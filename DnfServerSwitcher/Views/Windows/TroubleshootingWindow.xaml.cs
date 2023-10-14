using System.Windows;
namespace DnfServerSwitcher.Views.Windows {
    public partial class TroubleshootingWindow : Window {
        public TroubleshootingWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

