using System.Windows;
namespace DnfServerSwitcher.Views.Windows {
    public partial class FaqWindow : Window {
        public FaqWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

