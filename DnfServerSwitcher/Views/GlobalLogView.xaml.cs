using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DnfServerSwitcher.Models.Trace;
namespace DnfServerSwitcher.Views {
    public partial class GlobalLogView : UserControl, IMyTraceListener {
        public GlobalLogView() {
            InitializeComponent();
        }

        public void StartListening() {
            MyTrace.Global.AddListener(this);
        }

        public void StopListening() {
            MyTrace.Global.RemoveListener(this);
        }


        public MyTraceLevel Levels { get; } = MyTraceLevel.Critical | MyTraceLevel.Error | MyTraceLevel.Warning | MyTraceLevel.Information | MyTraceLevel.Verbose;

        public bool ShowLevels { get; } = true;
        public bool IsClosed { get; } = false;

        public void WriteMessage(DateTime timestamp, string category, string message, MyTraceLevel level) {
            if (this.ShowLevels) {
                this.TheTextBox.AppendText(MyTrace.FormatLine(timestamp, category, message, level) + Environment.NewLine);
            } else {
                this.TheTextBox.AppendText(MyTrace.FormatLine(timestamp, category, message) + Environment.NewLine);
            }
        }

        public void WriteMessage(DateTime timestamp, string category, List<string> messages, MyTraceLevel level) {
            foreach (string message in messages) {
                if (this.ShowLevels) {
                    this.TheTextBox.AppendText(MyTrace.FormatLine(timestamp, category, message, level) + Environment.NewLine);
                } else {
                    this.TheTextBox.AppendText(MyTrace.FormatLine(timestamp, category, message) + Environment.NewLine);
                }
            }
        }
        
        public void Close() {
            // nothing to do...
        }
        public void Flush() {
            // nothing to do...
        }
    }
}

