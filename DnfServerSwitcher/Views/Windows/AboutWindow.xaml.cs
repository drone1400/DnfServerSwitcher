using System.Windows;
namespace DnfServerSwitcher.Views.Windows {
    public partial class AboutWindow : Window {
        public AboutWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

