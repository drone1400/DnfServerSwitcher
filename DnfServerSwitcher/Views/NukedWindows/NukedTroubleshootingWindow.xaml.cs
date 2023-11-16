using System.Windows;
using DukNuk.Wpf.Controls;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedTroubleshootingWindow : NukWindow {
        public NukedTroubleshootingWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

