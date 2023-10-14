using System;
using System.Windows;
namespace DnfServerSwitcher.Views.Windows {
    public partial class LogWindow : Window {
        public LogWindow() {
            InitializeComponent();
            this.TheLogView.StartListening();
            this.Closed += this.OnClosed;
        }
        private void OnClosed(object sender, EventArgs e) {
            this.TheLogView.StopListening();
        }
    }
}

