using System;
using System.IO;
using System.Windows.Controls;
using NLog;
using Torch;
using Torch.API;
using Torch.API.Plugins;


namespace DeSafeZoneProtection
{
    public class DeSafeZoneProtectionPlugin : TorchPluginBase, IWpfPlugin, ITorchPlugin, IDisposable
    {
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private MainWindow _control;
        public SafeZonesCache Cache;
        public Persistent<DeSafeZoneConfig> ConfigPersistent;
        public DeSafeZoneConfig Config => ConfigPersistent?.Data;

        public static DeSafeZoneProtectionPlugin Instance { get; protected set; }

        public UserControl GetControl() => _control ?? (UserControl)(_control = new MainWindow(this));

        public void LoadConfig()
        {
            string path = Path.Combine(StoragePath, "DeSafeZoneProtection.cfg");
            if (ConfigPersistent?.Data != null)
                ConfigPersistent = Persistent<DeSafeZoneConfig>.Load(path, true);
        }

        public void SetupConfig()
        {
            string path = Path.Combine(StoragePath, "DeSafeZoneProtection.cfg");
            try
            {
                ConfigPersistent = Persistent<DeSafeZoneConfig>.Load(path, true);
            }
            catch (Exception ex)
            {
                Log.Warn(ex);
            }
            if (ConfigPersistent?.Data != null)
                return;

            Log.Info("Create Default Config, because none was found!");
            ConfigPersistent = new Persistent<DeSafeZoneConfig>(path, new DeSafeZoneConfig());
            ConfigPersistent.Save(null);
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            Instance = this;

            SetupConfig();

            if (Config.Enabled)
            {
                Cache = new SafeZonesCache("DeSCache.cache");
                Cache.Load();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (!Config.Enabled)
                return;

            Cache.Dispose();
        }
    }
}
