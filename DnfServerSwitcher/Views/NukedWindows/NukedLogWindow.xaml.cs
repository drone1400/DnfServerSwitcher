using System;
using DukNuk.Wpf.Controls;
namespace DnfServerSwitcher.Views.NukedWindows {
    public partial class NukedLogWindow : NukWindow {
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

