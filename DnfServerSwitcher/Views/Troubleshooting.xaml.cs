using System.Windows.Controls;
using System.Windows.Navigation;
namespace DnfServerSwitcher.Views {
    public partial class Troubleshooting : UserControl {
        public Troubleshooting() {
            InitializeComponent();
        }
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }
    }
}

