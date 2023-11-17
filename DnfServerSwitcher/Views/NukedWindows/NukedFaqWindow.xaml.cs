using System.Windows;
using DukNuk.Wpf.Controls;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedFaqWindow : NukWindow {
        public NukedFaqWindow() {
            InitializeComponent();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}

