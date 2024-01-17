using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Jotunn.Configs;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx.Configuration;
using Configuration;

namespace MoreOreDeposits
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class MoreOreDeposits : BaseUnityPlugin, Configuration.IPlugin
    {
        #region Plugin Info
        public const string PluginGUID = "com.bepinex.MoreOreDeposits";
        public const string PluginName = "More Ore Deposits";
        public const string PluginVersion = "1.3.1";
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("More Ore Deposits has Loaded");

            // Subscribe to the OnVanillaPrefabsAvailable event
            PrefabManager.OnVanillaPrefabsAvailable += OnPrefabsAvailable;

            // Apply Harmony patches
            var harmony = new Harmony(PluginGUID);
            harmony.PatchAll();

            SynchronizationManager.OnConfigurationSynchronized += OnConfigurationSynchronized;
            Config.ConfigReloaded += OnConfigReloaded;

            var _ = new ConfigWatcher(this);

            InitializeOreConfigs();

        }

        private void OnPrefabsAvailable()
        {

            // Load assets and add vegetation here
            LoadAssets();
            CreateGoldOre();
            AddVegetation();
            AddlocalizationsEnglish();
            JSONS();

            // Unsubscribe if you only want to execute this once
            PrefabManager.OnVanillaPrefabsAvailable -= OnPrefabsAvailable;
        }
        #endregion

        #region Localizations
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private void AddlocalizationsEnglish()
        {
            Localization = LocalizationManager.Instance.GetLocalization();
            Localization.AddTranslation("English", new Dictionary<string, string>
            {
              { "GoldOre_warp", "Gold ore" },
              { "GoldOre_desc_warp", "Unrefined gold. Use a smelter to refine into gold coins." },
              { "GoldDeposit_warp", "Gold" },
              { "IronDeposit_warp", "Iron" },
              { "SilverDepositSmall_warp", "Silver" },
              { "BlackmetalDeposit_warp", "Blackmetal" },
              { "CoalDeposit_warp", "Coal rock"}
            });
        }

        private void JSONS()
        {
            if (translationBundle == null)
            {
                return;
            }

            TextAsset[] textAssets = translationBundle.LoadAllAssets<TextAsset>();

            foreach (var textAsset in textAssets)
            {
                var lang = textAsset.name.Replace("_MoreOreDeposits", "");
                Localization.AddJsonFile(lang, textAsset.text);
            }
        }
        #endregion

        #region Configuration
        ConfigFile Configuration.IPlugin.Config => this.Config;
        private bool settingsUpdated = false;

        private OreDropConfig goldOreConfig;
        private OreDropConfig ironOreConfig;
        private OreDropConfig silverOreConfig;
        private OreDropConfig blackmetalOreConfig;
        private OreDropConfig coalOreConfig;

        private void InitializeOreConfigs()
        {

            // Initialize ore configurations
            goldOreConfig = OreDropConfig.GetFromProps(this, "GoldOre", 1, 2);
            ironOreConfig = OreDropConfig.GetFromProps(this, "IronScrap", 2, 3);
            silverOreConfig = OreDropConfig.GetFromProps(this, "SilverOre", 2, 3);
            blackmetalOreConfig = OreDropConfig.GetFromProps(this, "BlackMetalScrap", 2, 3);
            coalOreConfig = OreDropConfig.GetFromProps(this, "Coal", 2, 3);

            // Optionally add handlers for settings changes
            goldOreConfig.AddSettingsChangedHandler(OnSettingsChanged);
            ironOreConfig.AddSettingsChangedHandler(OnSettingsChanged);
            silverOreConfig.AddSettingsChangedHandler(OnSettingsChanged);
            blackmetalOreConfig.AddSettingsChangedHandler(OnSettingsChanged);
            coalOreConfig.AddSettingsChangedHandler(OnSettingsChanged);
        }

        private void UpdateFeatures()
        {
            settingsUpdated = false;
            ConfigureDropOnDestroyed(goldDepositPrefab, goldOreConfig);
            ConfigureDropOnDestroyed(ironDepositPrefab, ironOreConfig);
            ConfigureDropOnDestroyed(silverDepositPrefab, silverOreConfig);
            ConfigureDropOnDestroyed(blackmetalDepositPrefab, blackmetalOreConfig);
            ConfigureDropOnDestroyed(coalDepositPrefab, coalOreConfig);
        }

        private void OnSettingsChanged(object sender, System.EventArgs e)
        {
            settingsUpdated = true;
        }

        private void OnConfigReloaded(object sender, System.EventArgs e)
        {
            if (settingsUpdated)
            {
                UpdateFeatures();
            }
        }

        private void OnConfigurationSynchronized(object sender, ConfigurationSynchronizationEventArgs e)
        {
            UpdateFeatures();
        }

        private DropTable.DropData getDropDataFromEntry(OreDropConfig entry)
        {
            return new DropTable.DropData
            {
                m_item = PrefabManager.Instance.GetPrefab(entry.OreName),
                m_stackMin = entry.DropMin.Value,
                m_stackMax = entry.DropMax.Value,
                m_weight = 1f // Assuming a fixed weight, adjust as necessary
            };
        }
        #endregion

        #region Asset Bundles and Prefabs
        private AssetBundle goldAssetBundle;
        private GameObject goldDepositPrefab;
        private GameObject goldOrePrefab;

        private AssetBundle ironAssetBundle;
        private GameObject ironDepositPrefab;

        private AssetBundle silverAssetBundle;
        private GameObject silverDepositPrefab;

        private AssetBundle blackmetalAssetBundle;
        private GameObject blackmetalDepositPrefab;
        
        private AssetBundle coalAssetBundle;
        private GameObject coalDepositPrefab;

        private AssetBundle translationBundle;

        private void LoadAssets()
        {
            goldAssetBundle = AssetUtils.LoadAssetBundleFromResources("gold_bundle");
            goldDepositPrefab = goldAssetBundle?.LoadAsset<GameObject>("MineRock_gold");
            goldOrePrefab = goldAssetBundle?.LoadAsset<GameObject>("GoldOre");

            ironAssetBundle = AssetUtils.LoadAssetBundleFromResources("iron_bundle");
            ironDepositPrefab = ironAssetBundle?.LoadAsset<GameObject>("MineRock_iron");

            silverAssetBundle = AssetUtils.LoadAssetBundleFromResources("silver_bundle");
            silverDepositPrefab = silverAssetBundle?.LoadAsset<GameObject>("MineRock_silver_small");

            blackmetalAssetBundle = AssetUtils.LoadAssetBundleFromResources("blackmetal_bundle");
            blackmetalDepositPrefab = blackmetalAssetBundle?.LoadAsset<GameObject>("MineRock_blackmetal");
            
            coalAssetBundle = AssetUtils.LoadAssetBundleFromResources("coal_bundle");
            coalDepositPrefab = coalAssetBundle?.LoadAsset<GameObject>("MineRock_coal");

            translationBundle = AssetUtils.LoadAssetBundleFromResources("translations_bundle");

            LogResourceNamesAndCheckErrors();
        }

        private void LogResourceNamesAndCheckErrors()
        {
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            Jotunn.Logger.LogInfo($"Embedded resources: {string.Join(", ", resourceNames)}");

            CheckAssetBundle(goldAssetBundle, "gold");
            CheckAssetBundle(ironAssetBundle, "iron");
            CheckAssetBundle(silverAssetBundle, "silver");
            CheckAssetBundle(blackmetalAssetBundle, "blackmetal");
            CheckAssetBundle(coalAssetBundle, "coal");
        }

        private void CheckAssetBundle(AssetBundle bundle, string bundleName)
        {
            if (bundle == null)
            {
                Jotunn.Logger.LogError($"Failed to load the {bundleName} asset bundle.");
            }
        }
        #endregion

        #region ItemManager
        private void CreateGoldOre()
        {

            var goldOreItem = new CustomItem(goldOrePrefab, false);
            ItemManager.Instance.AddItem(goldOreItem);

            var goldOreSmelterConfig = new SmelterConversionConfig();
            goldOreSmelterConfig.FromItem = "GoldOre";
            goldOreSmelterConfig.ToItem = "Coins";
            goldOreSmelterConfig.Station = Smelters.Smelter;
            ItemManager.Instance.AddItemConversion(new CustomItemConversion(goldOreSmelterConfig));

            ConfigureGoldOreAutoPickup("GoldOre");

        }

        private void ConfigureGoldOreAutoPickup(string itemName)
        {
            // Get the prefab for the gold ore item
            GameObject itemPrefab = PrefabManager.Instance.GetPrefab(itemName);
            if (itemPrefab == null)
            {
                Debug.LogError($"Prefab '{itemName}' not found.");
                return;
            }

            // Get the existing ItemDrop component from the prefab
            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            if (itemDrop == null)
            {
                Debug.LogError($"ItemDrop component not found on prefab '{itemName}'.");
                return;
            }

            // Set the item to auto-pickup
            itemDrop.m_autoPickup = true;
        }
        #endregion

        #region VegetationConfigs
        // Define the vegetation configuration
        VegetationConfig goldDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.BlackForest,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,
            ScaleMin = 295,
            ScaleMax = 296,
            MinAltitude = 0f,

        };

        // Define the vegetation configuration
        VegetationConfig ironDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Swamp,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,
            ScaleMin = 295,
            ScaleMax = 296,
            MinAltitude = 0f,

        };

        // Define the vegetation configuration
        VegetationConfig silverDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Mountain,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,
            ScaleMin = 295,
            ScaleMax = 296,
            MinAltitude = 0f,

        };

        // Define the vegetation configuration
        VegetationConfig blackmetalDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Plains,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,
            ScaleMin = 295,
            ScaleMax = 296,
            MinAltitude = 0f,

        };
        
        // Define the vegetation configuration
        VegetationConfig coalDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Swamp,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,
            ScaleMin = 300,
            ScaleMax = 300,
            MinAltitude = 0,

        };
        #endregion

        #region ZoneManager
        private void AddVegetation()
        {
            // Ensure all prefabs are loaded
            if (goldDepositPrefab == null || ironDepositPrefab == null || silverDepositPrefab == null || blackmetalDepositPrefab == null || coalDepositPrefab == null)
            {
                Jotunn.Logger.LogError("One or more deposit prefabs are not loaded.");
                return;
            }

            ConfigureDestructible(goldDepositPrefab, 0, 30f);
            ConfigureDestructible(ironDepositPrefab, 1, 30f);
            ConfigureDestructible(silverDepositPrefab, 2, 30f);
            ConfigureDestructible(blackmetalDepositPrefab, 2, 30f);
            ConfigureDestructible(coalDepositPrefab, 0, 30f);

            ConfigureDropOnDestroyed(goldDepositPrefab, goldOreConfig);
            ConfigureDropOnDestroyed(ironDepositPrefab, ironOreConfig);
            ConfigureDropOnDestroyed(silverDepositPrefab, silverOreConfig);
            ConfigureDropOnDestroyed(blackmetalDepositPrefab, blackmetalOreConfig);
            ConfigureDropOnDestroyed(coalDepositPrefab, coalOreConfig);

            ConfigureHoverText(goldDepositPrefab, "$GoldDeposit_warp");
            ConfigureHoverText(ironDepositPrefab, "$IronDeposit_warp");
            ConfigureHoverText(silverDepositPrefab, "$SilverDepositSmall_warp");
            ConfigureHoverText(blackmetalDepositPrefab, "$BlackmetalDeposit_warp");
            ConfigureHoverText(coalDepositPrefab, "$CoalDeposit_warp");

            CustomVegetation goldDepositVegetation = new CustomVegetation(goldDepositPrefab, false, goldDepositConfig);
            CustomVegetation ironDepositVegetation = new CustomVegetation(ironDepositPrefab, false, ironDepositConfig);
            CustomVegetation silverDepositVegetation = new CustomVegetation(silverDepositPrefab, false, silverDepositConfig);
            CustomVegetation blackmetalDepositVegetation = new CustomVegetation(blackmetalDepositPrefab, false, blackmetalDepositConfig);
            CustomVegetation coalDepositVegetation = new CustomVegetation(coalDepositPrefab, false, coalDepositConfig);

            ZoneManager.Instance.AddCustomVegetation(goldDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(ironDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(silverDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(blackmetalDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(coalDepositVegetation);
        }
        #endregion

        #region Unity Scripts
        private void ConfigureDestructible(GameObject prefab, int minToolTier, float health)
        {
            var destructible = prefab.GetComponent<Destructible>() ?? prefab.AddComponent<Destructible>();
            destructible.m_minToolTier = minToolTier;
            destructible.m_health = health;

            // Set up destroyed and hit effects
            GameObject destroyedEffectPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_RockDestroyed");
            GameObject destroyedSoundPrefab = PrefabManager.Cache.GetPrefab<GameObject>("sfx_rock_destroyed");
            GameObject hitEffectPrefab = PrefabManager.Cache.GetPrefab<GameObject>("vfx_RockHit");
            GameObject hitSoundPrefab = PrefabManager.Cache.GetPrefab<GameObject>("sfx_rock_hit");

            destructible.m_destroyedEffect.m_effectPrefabs = new EffectList.EffectData[]
            {
                new EffectList.EffectData { m_prefab = destroyedEffectPrefab },
                new EffectList.EffectData { m_prefab = destroyedSoundPrefab }
            };

            destructible.m_hitEffect.m_effectPrefabs = new EffectList.EffectData[]
            {
                new EffectList.EffectData { m_prefab = hitEffectPrefab },
                new EffectList.EffectData { m_prefab = hitSoundPrefab }
            };
        }

        private void ConfigureDropOnDestroyed(GameObject prefab, OreDropConfig oreConfig)
        {
            var dropOnDestroyed = prefab.GetComponent<DropOnDestroyed>() ?? prefab.AddComponent<DropOnDestroyed>();
            dropOnDestroyed.m_dropWhenDestroyed.m_drops = new List<DropTable.DropData>
                {
                    getDropDataFromEntry(oreConfig)
                };
        }

        private void ConfigureHoverText(GameObject prefab, string hoverText)
        {
            if (prefab == null)
            {
                Jotunn.Logger.LogError("Prefab is null. Cannot add HoverText.");
                return;
            }

            // Check if HoverText component already exists, if not, add it
            HoverText hoverTextComponent = prefab.GetComponent<HoverText>();
            if (hoverTextComponent == null)
            {
                hoverTextComponent = prefab.AddComponent<HoverText>();
            }

            // Set the hover text
            hoverTextComponent.m_text = hoverText;
        }


        #endregion

    }

    #region Harmony Patches
    // Harmony patch class
    [HarmonyPatch(typeof(Smelter), "Spawn")]
    public static class SmelterProduceMore
    {
        public static void Prefix(Smelter __instance, string ore, ref int stack)
        {
            if (!__instance) return;
            if (ore == "GoldOre") // Make sure this matches the exact name of your ore item
            {
                stack *= 10; // Multiply the stack by 10 for gold ore
            }
        }
    }
    #endregion

}