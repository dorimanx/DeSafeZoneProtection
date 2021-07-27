using System.Windows;
using System.Windows.Controls;

namespace DeSafeZoneProtection
{
    public partial class MainWindow : UserControl
    {
        public DeSafeZoneProtectionPlugin Plugin { get; }

        private MainWindow() => InitializeComponent();

        public MainWindow(DeSafeZoneProtectionPlugin plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;
            distTextBox.Text = plugin.Config.Distance.ToString("0");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (float.TryParse(((TextBox)sender).Text, out float distance))
            {
                Plugin.Config.Distance = distance;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Plugin.LoadConfig();
            Plugin.ConfigPersistent.Save(null);
            Plugin.SetupConfig();
        }
    }
}
