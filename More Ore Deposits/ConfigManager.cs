using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Configuration
{
    //public interface IPlugin
    //{
    //    ConfigFile Config { get; }
    //}

    public class AcceptableValueConfigNote : AcceptableValueBase
    {
        public virtual string Note { get; }

        public AcceptableValueConfigNote(string note) : base(typeof(string))
        {
            if (string.IsNullOrEmpty(note))
            {
                throw new ArgumentException("A string with atleast 1 character is needed", "Note");
            }
            this.Note = note;
        }

        // passthrough overrides
        public override object Clamp(object value) { return value; }
        public override bool IsValid(object value) { return !string.IsNullOrEmpty(value as string); }

        public override string ToDescriptionString()
        {
            return "# Note: " + Note;
        }
    }

    // use bepinex ConfigEntry settings
    public static class ConfigHelper
    {
        public static ConfigurationManagerAttributes GetAdminOnlyFlag()
        {
            return new ConfigurationManagerAttributes { IsAdminOnly = true };
        }

        public static ConfigurationManagerAttributes GetTags(Action<ConfigEntryBase> action)
        {
            return new ConfigurationManagerAttributes() { CustomDrawer = action };
        }

        public static ConfigurationManagerAttributes GetTags()
        {
            return new ConfigurationManagerAttributes();
        }

        public static ConfigEntry<T> Config<T>(this IPlugin instance, string group, string name, T value, ConfigDescription description)
        {
            return instance.Config.Bind(group, name, value, description);
        }

        public static ConfigEntry<T> Config<T>(this IPlugin instance, string group, string name, T value, string description) => Config(instance, group, name, value, new ConfigDescription(description, null, GetAdminOnlyFlag()));
    }

    public static class RequirementsEntry
    {
        public static RequirementConfig[] Deserialize(string reqs)
        {
            return reqs.Split(',').Select(r =>
            {
                string[] parts = r.Split(':');
                return new RequirementConfig
                {
                    Item = parts[0],
                    Amount = parts.Length > 1 && int.TryParse(parts[1], out int amount) ? amount : 1,
                    AmountPerLevel = parts.Length > 2 && int.TryParse(parts[2], out int apl) ? apl : 0,
                    Recover = true // defaulting to true
                };
            }).ToArray();
        }

        public static string Serialize(RequirementConfig[] reqs)
        {
            return string.Join(",", reqs.Select(r =>
                r.AmountPerLevel > 0 ?
                    $"{r.Item}:{r.Amount}:{r.AmountPerLevel}" :
                    $"{r.Item}:{r.Amount}"));
        }
    }

    public static class SharedDrawers
    {
        private static BaseUnityPlugin configManager = null;
        private static BaseUnityPlugin GetConfigManager()
        {
            if (SharedDrawers.configManager == null)
            {
                PluginInfo configManagerInfo;
                if (Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.configurationmanager", out configManagerInfo) && configManagerInfo.Instance)
                {
                    SharedDrawers.configManager = configManagerInfo.Instance;
                }
            }

            return SharedDrawers.configManager;
        }

        public static int GetRightColumnWidth()
        {
            int result = 130;
            BaseUnityPlugin configManager = GetConfigManager();
            if (configManager != null)
            {
                PropertyInfo pi = configManager?.GetType().GetProperty("RightColumnWidth", BindingFlags.Instance | BindingFlags.NonPublic);
                if (pi != null)
                {
                    result = (int)pi.GetValue(configManager);
                }
            }

            return result;
        }

        public static void ReloadConfigDisplay()
        {
            BaseUnityPlugin configManager = GetConfigManager();
            if (configManager != null)
            {
                configManager.GetType().GetMethod("BuildSettingList").Invoke(configManager, Array.Empty<object>());
            }
        }

        public static Action<ConfigEntryBase> DrawReqConfigTable(bool hasUpgrades = false)
        {
            return cfg =>
            {
                List<RequirementConfig> newReqs = new List<RequirementConfig>();
                bool wasUpdated = false;

                int RightColumnWidth = GetRightColumnWidth();

                GUILayout.BeginVertical();

                List<RequirementConfig> reqs = RequirementsEntry.Deserialize((string)cfg.BoxedValue).ToList();

                foreach (var req in reqs)
                {
                    GUILayout.BeginHorizontal();

                    string newItem = GUILayout.TextField(req.Item, new GUIStyle(GUI.skin.textField) { fixedWidth = RightColumnWidth - 33 - (hasUpgrades ? 37 : 0) - 21 - 21 - 12 });
                    string prefabName = string.IsNullOrEmpty(newItem) ? req.Item : newItem;
                    wasUpdated = wasUpdated || prefabName != req.Item;


                    int amount = req.Amount;
                    if (int.TryParse(GUILayout.TextField(amount.ToString(), new GUIStyle(GUI.skin.textField) { fixedWidth = 33 }), out int newAmount) && newAmount != amount)
                    {
                        amount = newAmount;
                        wasUpdated = true;
                    }

                    int amountPerLvl = req.AmountPerLevel;
                    if (hasUpgrades)
                    {
                        if (int.TryParse(GUILayout.TextField(amountPerLvl.ToString(), new GUIStyle(GUI.skin.textField) { fixedWidth = 33 }), out int newAmountPerLvl) && newAmountPerLvl != amountPerLvl)
                        {
                            amountPerLvl = newAmountPerLvl;
                            wasUpdated = true;
                        }
                    }

                    if (GUILayout.Button("x", new GUIStyle(GUI.skin.button) { fixedWidth = 21 }))
                    {
                        wasUpdated = true;
                    }
                    else
                    {
                        newReqs.Add(new RequirementConfig { Item = prefabName, Amount = amount, AmountPerLevel = amountPerLvl });
                    }

                    if (GUILayout.Button("+", new GUIStyle(GUI.skin.button) { fixedWidth = 21 }))
                    {
                        wasUpdated = true;
                        newReqs.Add(new RequirementConfig { Item = "<Prefab Name>", Amount = 1 });
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                if (wasUpdated)
                {
                    cfg.BoxedValue = RequirementsEntry.Serialize(newReqs.ToArray());
                }
            };
        }
    }
}
