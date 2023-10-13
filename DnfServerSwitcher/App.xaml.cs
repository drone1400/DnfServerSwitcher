using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DnfServerSwitcher.Models.Trace;
using DnfServerSwitcher.ViewModels;
using DnfServerSwitcher.Views;

namespace DnfServerSwitcher {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private MainWindow? _mainWindow;
        private MainViewModel? _mainVm;
        
        public App() {
            AppDomain.CurrentDomain.UnhandledException += this.UnhandledExceptionHandler;
        }
        
        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e) {
            if (!e.IsTerminating) {
                Glog.Message(MyTraceCategory.Global,"Encountered a global unhandled exception!", MyTraceLevel.Critical);
            } else {
                Glog.Message(MyTraceCategory.Global,"Encountered a global unhandled exception that is TERMINATING!", MyTraceLevel.Critical);
            }
            if (e.ExceptionObject is Exception ex) {
                Glog.Error(MyTraceCategory.Global, ex, MyTraceLevel.Critical);
            } else {
                Glog.Message(MyTraceCategory.Global, e.ExceptionObject.ToString(), MyTraceLevel.Critical);
            }
        }
        
        protected override void OnStartup(StartupEventArgs e) {
            this._mainVm = new MainViewModel();
            this._mainWindow = new MainWindow();
            this._mainWindow.Content = new MainView();
            this._mainWindow.DataContext = this._mainVm;
            
            this._mainWindow.Show();
            this._mainWindow.Closed += MainWindowOnClosed;
            
            base.OnStartup(e);
        }
        private void MainWindowOnClosed(object sender, EventArgs e) {
            this._mainVm?.MyCfg.SaveToIni();
            App.Current.Shutdown();
        }
    }
    

}
