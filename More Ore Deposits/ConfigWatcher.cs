using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Managers;
using System;
using System.IO;
using System.Reflection;

namespace Configuration
{
    public interface IPlugin
    {
        ConfigFile Config { get; }
    }

    public class ConfigWatcher
    {
        private BaseUnityPlugin configurationManager;
        private IPlugin plugin;

        public ConfigWatcher(IPlugin plugin)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            this.plugin = plugin;
            CheckForConfigManager();
        }

        private void InitializeConfigWatcher()
        {
            string file = Path.GetFileName(plugin.Config.ConfigFilePath);
            string path = Path.GetDirectoryName(plugin.Config.ConfigFilePath);
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(path, file);
            fileSystemWatcher.Changed += this.OnConfigFileChangedCreatedOrRenamed;
            fileSystemWatcher.Created += this.OnConfigFileChangedCreatedOrRenamed;
            fileSystemWatcher.Renamed += new RenamedEventHandler(this.OnConfigFileChangedCreatedOrRenamed);
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            fileSystemWatcher.EnableRaisingEvents = true;

            Jotunn.Logger.LogDebug("File system config watcher initialized.");
        }

        private void CheckForConfigManager()
        {
            if (GUIManager.IsHeadless())
            {
                InitializeConfigWatcher();
            }
            else
            {
                PluginInfo configManagerInfo;
                if (Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.configurationmanager", out configManagerInfo) && configManagerInfo.Instance)
                {
                    this.configurationManager = configManagerInfo.Instance;
                    Jotunn.Logger.LogDebug("Configuration manager found, hooking DisplayingWindowChanged");
                    EventInfo eventinfo = this.configurationManager.GetType().GetEvent("DisplayingWindowChanged");
                    if (eventinfo != null)
                    {
                        Action<object, object> local = new Action<object, object>(this.OnConfigManagerDisplayingWindowChanged);
                        Delegate converted = Delegate.CreateDelegate(eventinfo.EventHandlerType, local.Target, local.Method);
                        eventinfo.AddEventHandler(this.configurationManager, converted);
                    }
                }
                else
                {
                    InitializeConfigWatcher();
                }
            }
        }

        private void OnConfigFileChangedCreatedOrRenamed(object sender, FileSystemEventArgs e)
        {
            string path = plugin.Config.ConfigFilePath;

            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                plugin.Config.SaveOnConfigSet = false;
                plugin.Config.Reload();
                plugin.Config.SaveOnConfigSet = true;
            }
            catch
            {
                Jotunn.Logger.LogError("There was an issue with your " + Path.GetFileName(path) + " file.");
                Jotunn.Logger.LogError("Please check the format and spelling.");
                return;
            }
        }

        private void OnConfigManagerDisplayingWindowChanged(object sender, object e)
        {
            //Jotunn.Logger.LogDebug("OnConfigManagerDisplayingWindowChanged recieved.");
            PropertyInfo pi = this.configurationManager.GetType().GetProperty("DisplayingWindow");
            bool cmActive = (bool)pi.GetValue(this.configurationManager, null);

            if (!cmActive)
            {
                plugin.Config.SaveOnConfigSet = false;
                plugin.Config.Reload();
                plugin.Config.SaveOnConfigSet = true;
            }
        }
    }
}
