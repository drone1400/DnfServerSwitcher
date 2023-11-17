using System.Windows.Controls;
using System.Windows.Navigation;
namespace DnfServerSwitcher.Views {
    public partial class HelpAbout : UserControl {
        public HelpAbout() {
            InitializeComponent();
        }
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }
    }
}

