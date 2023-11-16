using System.Windows;
using System.Windows.Input;
namespace DnfServerSwitcher.Views.Windows {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            this.InitializeComponent();
        }
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.F7) {
                (Application.Current as App)?.ThemeToggleOnOff();
                e.Handled = true;
            } else if (e.Key == Key.F8) {
                (Application.Current as App)?.ThemeSelectNext();
                e.Handled = true;
            }
        }
    }
}
