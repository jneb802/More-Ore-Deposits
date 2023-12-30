using BepInEx;
using BepInEx.Configuration;
using Jotunn.Configs;
using System;
using UnityEngine;
using Configuration;
using System.Linq;
using System.Collections.Generic;

namespace MoreOreDeposits
{
    public class OreDropConfig
    {
        public string OreName { get; set; }
        public ConfigEntry<int> DropMin { get; set; }
        public ConfigEntry<int> DropMax { get; set; }

        
        public static OreDropConfig GetFromProps(BaseUnityPlugin instance, string oreName, int defaultMin, int defaultMax)
        {
            OreDropConfig config = new OreDropConfig();
            config.OreName = oreName;
            config.DropMin = instance.Config.Bind("Server config", $"{oreName} Drop Min", defaultMin, new ConfigDescription($"Minimum amount of {oreName} dropped from {oreName} deposits"));
            config.DropMax = instance.Config.Bind("Server config", $"{oreName} Drop Max", defaultMax, new ConfigDescription($"Maximum amount of {oreName} dropped from {oreName} deposits"));

            return config;
        }

        private Action<object, EventArgs> _action = null;
        private void OnSettingChanged(object sender, EventArgs e)
        {
            if (_action != null)
            {
                _action(sender, e);
            }
        }
        public void AddSettingsChangedHandler(Action<object, EventArgs> action)
        {
            _action = action;
            DropMin.SettingChanged += OnSettingChanged;
            DropMax.SettingChanged += OnSettingChanged;
        }

        public void RemoveSettingsChangedHandler()
        {
            DropMin.SettingChanged -= OnSettingChanged;
            DropMax.SettingChanged -= OnSettingChanged;
            _action = null;
        }
    }
}
