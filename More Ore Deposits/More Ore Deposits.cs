using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using Jotunn.Configs;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace MoreOreDeposits
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class moreOreDeposits : BaseUnityPlugin
    {
        public const string PluginGUID = "com.bepinex.MoreOreDeposits";
        public const string PluginName = "More Ore Deposits";
        public const string PluginVersion = "0.0.1";

        // Use this class to add your own localization to the game
        // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

        private AssetBundle goldAssetBundle;
        private GameObject goldDepositPrefab;

        private AssetBundle ironAssetBundle;
        private GameObject ironDepositPrefab;

        private AssetBundle silverAssetBundle;
        private GameObject silverDepositPrefab;

        private AssetBundle blackmetalAssetBundle;
        private GameObject blackmetalDepositPrefab;

        private void Awake()
        {
            // Jotunn comes with its own Logger class to provide a consistent Log style for all mods using it
            Jotunn.Logger.LogInfo("More Ore Deposits has Loaded");

            // Subscribe to the OnVanillaPrefabsAvailable event
            PrefabManager.OnVanillaPrefabsAvailable += OnPrefabsAvailable;

        }

        private void OnPrefabsAvailable()
        {

            // Load assets and add vegetation here
            LoadAssets();
            AddVegetation();

            // Unsubscribe if you only want to execute this once
            PrefabManager.OnVanillaPrefabsAvailable -= OnPrefabsAvailable;
        }

        private void LoadAssets()
        {
            goldAssetBundle = AssetUtils.LoadAssetBundleFromResources("gold_bundle");
            goldDepositPrefab = goldAssetBundle?.LoadAsset<GameObject>("gold_deposit_prefab");

            ironAssetBundle = AssetUtils.LoadAssetBundleFromResources("iron_bundle");
            ironDepositPrefab = ironAssetBundle?.LoadAsset<GameObject>("iron_deposit_prefab");

            silverAssetBundle = AssetUtils.LoadAssetBundleFromResources("silver_bundle");
            silverDepositPrefab = silverAssetBundle?.LoadAsset<GameObject>("silver_deposit_prefab");

            blackmetalAssetBundle = AssetUtils.LoadAssetBundleFromResources("blackmetal_bundle");
            blackmetalDepositPrefab = blackmetalAssetBundle?.LoadAsset<GameObject>("blackmetal_deposit_prefab");

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
        }

        private void CheckAssetBundle(AssetBundle bundle, string bundleName)
        {
            if (bundle == null)
            {
                Jotunn.Logger.LogError($"Failed to load the {bundleName} asset bundle.");
            }
        }

        // Define the vegetation configuration
        VegetationConfig goldDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Swamp | Heightmap.Biome.BlackForest | Heightmap.Biome.Mistlands,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,

        };

        // Define the vegetation configuration
        VegetationConfig ironDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Swamp,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,

        };

        // Define the vegetation configuration
        VegetationConfig silverDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Mountain,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,

        };

        // Define the vegetation configuration
        VegetationConfig blackmetalDepositConfig = new VegetationConfig
        {
            Biome = Heightmap.Biome.Plains,
            BlockCheck = true,
            Min = 0,
            Max = 2,
            GroundOffset = -0.3f,

        };

        private void AddVegetation()
        {
            // Ensure all prefabs are loaded
            if (goldDepositPrefab == null || ironDepositPrefab == null || silverDepositPrefab == null || blackmetalDepositPrefab == null)
            {
                Jotunn.Logger.LogError("One or more deposit prefabs are not loaded.");
                return;
            }

            ConfigureDestructible(goldDepositPrefab, 1, 30f);
            ConfigureDestructible(ironDepositPrefab, 1, 30f);
            ConfigureDestructible(silverDepositPrefab, 2, 30f);
            ConfigureDestructible(blackmetalDepositPrefab, 3, 30f);

            ConfigureDropOnDestroyed(goldDepositPrefab, "Coins", 10, 25);
            ConfigureDropOnDestroyed(ironDepositPrefab, "IronOre", 1, 2);
            ConfigureDropOnDestroyed(silverDepositPrefab, "SilverOre", 1, 2);
            ConfigureDropOnDestroyed(blackmetalDepositPrefab, "BlackMetalScrap", 1, 2);

            //ConfigureHoverText(goldDepositPrefab, "$piece_deposit_gold");
            //ConfigureHoverText(ironDepositPrefab, "$piece_deposit_iron");
            //ConfigureHoverText(silverDepositPrefab, "$piece_deposit_small_silver");
            //ConfigureHoverText(blackmetalDepositPrefab, "$piece_deposit_blackmetal");


            // Assuming oakStumpPrefab is loaded and oakStumpConfig is correctly set up
            CustomVegetation goldDepositVegetation = new CustomVegetation(goldDepositPrefab, false, goldDepositConfig);
            CustomVegetation ironDepositVegetation = new CustomVegetation(ironDepositPrefab, false, ironDepositConfig);
            CustomVegetation silverDepositVegetation = new CustomVegetation(silverDepositPrefab, false, silverDepositConfig);
            CustomVegetation blackmetalDepositVegetation = new CustomVegetation(blackmetalDepositPrefab, false, blackmetalDepositConfig);

            ZoneManager.Instance.AddCustomVegetation(goldDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(ironDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(silverDepositVegetation);
            ZoneManager.Instance.AddCustomVegetation(blackmetalDepositVegetation);
        }

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

        private void ConfigureDropOnDestroyed(GameObject prefab, string itemName, int minStack, int maxStack)
        {
            var dropOnDestroyed = prefab.GetComponent<DropOnDestroyed>() ?? prefab.AddComponent<DropOnDestroyed>();
                dropOnDestroyed.m_dropWhenDestroyed.m_drops = new List<DropTable.DropData>
            {
                new DropTable.DropData
                {
                    m_item = PrefabManager.Instance.GetPrefab(itemName),
                    m_stackMin = minStack,
                    m_stackMax = maxStack,
                    m_weight = 1f
                },
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

    }
    
}