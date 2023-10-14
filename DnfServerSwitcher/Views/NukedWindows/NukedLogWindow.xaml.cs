using System;
using System.Windows;
using DnfServerSwitcher.Themes;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedLogWindow : NukedWindow {
        public NukedLogWindow() {
            InitializeComponent();
            this.TheLogView.StartListening();
            this.Closed += this.OnClosed;
        }
        private void OnClosed(object sender, EventArgs e) {
            this.TheLogView.StopListening();
        }
    }
}

