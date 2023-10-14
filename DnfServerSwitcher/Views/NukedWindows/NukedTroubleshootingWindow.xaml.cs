using System.Windows;
using DnfServerSwitcher.Themes;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedTroubleshootingWindow : NukedWindow {
        public NukedTroubleshootingWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

