using System.Windows;
using System.Windows.Input;
using DukNuk.Wpf.Controls;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedMainWindow : NukWindow {
        public NukedMainWindow() {
            InitializeComponent();
        }
        private void NukedMainWindow_OnKeyDown(object sender, KeyEventArgs e) {
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

