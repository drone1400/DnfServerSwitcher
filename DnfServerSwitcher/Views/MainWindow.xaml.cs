using System.Windows;
using DnfServerSwitcher.ViewModels;
namespace DnfServerSwitcher.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
