using System.Windows;
using DukNuk.Wpf.Controls;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedAboutWindow : NukWindow {
        public NukedAboutWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

