using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DnfServerSwitcher.Models;
using DnfServerSwitcher.Models.Trace;
using DnfServerSwitcher.ViewModels;
using DnfServerSwitcher.Views;
using DnfServerSwitcher.Views.Windows;

namespace DnfServerSwitcher {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public string AppBaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        

        private NukedMainWindow? _normalWindowNuked;
        private MainWindow? _normalWindow;
        
        private MainViewModel? _mainVm;
        private MyTraceListenerLogger _logger;

        
        
        public App() {
            string? processFile = Process.GetCurrentProcess().MainModule?.FileName;
            
            if (processFile == null) {
                NullReferenceException ex = new NullReferenceException("Could not determine current process start location...");
                Glog.Error(MyTraceCategory.General, ex);
            } else {
                FileInfo finfo = new FileInfo(processFile);

                if (finfo.DirectoryName == null) {
                    NullReferenceException ex = new NullReferenceException("Could not determine current process start location...");
                    Glog.Error(MyTraceCategory.General, ex);
                } else {
                    this.AppBaseDirectory = finfo.DirectoryName;
                }
            }
            
            this._logger = new MyTraceListenerLogger(this.AppBaseDirectory, "DnfSS_ErrorLog", false) {
                FlushAfterEachMessage = true,
                MaxTraceLevel = MyTraceLevel.Error,
            };
            
            MyTrace.Global.Listeners.Add(this._logger);
            
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
            if (this._mainVm.MyCfg.Theme == MyThemes.DookieNookie2001.ToString()) {
                this._normalWindowNuked = new NukedMainWindow();
                this._normalWindowNuked.DataContext = this._mainVm;
                this._normalWindowNuked.Show();
                this._normalWindowNuked.Closed += this.NormalWindowNukedOnClosed;
            } else {
                this._normalWindow = new MainWindow();
                this._normalWindow.DataContext = this._mainVm;
                this._normalWindow.Show();
                this._normalWindow.Closed += this.NormalWindowNukedOnClosed;
            }
            
            
            
            base.OnStartup(e);
        }
        private void NormalWindowNukedOnClosed(object sender, EventArgs e) {
            this._mainVm?.MyCfg.SaveToIni();
            this._logger.Flush();
            this._logger.Close();
            App.Current.Shutdown();
        }

        public void ShowTroubleshootingWindow() {
            if (this._mainVm?.MyCfg.Theme == MyThemes.DookieNookie2001.ToString()) {
                NukedTroubleshootingWindow trouble = new NukedTroubleshootingWindow();
                trouble.ShowDialog();
            } else {
                TroubleshootingWindow trouble = new TroubleshootingWindow();
                trouble.ShowDialog();
            }
        }
    }
    

}
