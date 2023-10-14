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
using DnfServerSwitcher.Views.NukedWindows;
using DnfServerSwitcher.Views.Windows;

namespace DnfServerSwitcher {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public const int DNF2011_STEAMAPPID = 57900;

        public string AppBaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        

        private Window? _mainWindow;
        private Window? _logWindow;

        private DnfServerSwitcherConfig _myCfg = new DnfServerSwitcherConfig();
        private MainViewModel? _mainVm;
        private MyTraceListenerFileLogger _fileLogger;
        
        
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
            
            this._fileLogger = new MyTraceListenerFileLogger(this.AppBaseDirectory, "DnfSS_ErrorLog", false) {
                FlushAfterEachMessage = true,
                MaxTraceLevel = MyTraceLevel.Error,
            };
            
            MyTrace.Global.AddListener(this._fileLogger);
            
            this.InitializeConfig();
            
            AppDomain.CurrentDomain.UnhandledException += this.UnhandledExceptionHandler;
        }

        private void InitializeConfig() {
            if (!this._myCfg.LoadFromIni() ||
                string.IsNullOrWhiteSpace(this._myCfg.Dnf2011ExePath) ||
                string.IsNullOrWhiteSpace(this._myCfg.Dnf2011SystemIniPath)) {
                // could not load config file.. try to auto detect paths!
                Dnf2011Finder df = new Dnf2011Finder();
                df.FindPaths();
                
                if (string.IsNullOrWhiteSpace(df.Dnf2011Exe) ||
                    df.Dnf2011SystemIni.Count == 0) {
                    MessageBox.Show("Could not locate DNF files, please manually set the correct paths for the Duke Nukem Forever exe and the System.ini file!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (string.IsNullOrWhiteSpace(df.Dnf2011Exe) == false) {
                    this._myCfg.Dnf2011ExePath = df.Dnf2011Exe;
                }
                if (df.Dnf2011SystemIni.Count > 0) {
                    if (df.Dnf2011SystemIni.Count > 1) {
                        MessageBox.Show("Multiple Steam users with different System.ini files detected! Defaulting to first user... Please manually select the correct System.ini file if you are using a different user...", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    // TODO... give some user prompt to select what user's data to use?...
                    // for now just default to first value i guess...
                    this._myCfg.Dnf2011SystemIniPath = df.Dnf2011SystemIni.First().Value;
                }
                
                this._myCfg.SaveToIni();
            }
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
            if (this._myCfg.OpenLogWindowOnStartup) {
                if (this._myCfg.Theme == MyThemes.DookieNookie2001.ToString()) {
                    this._logWindow = new NukedLogWindow();
                } else {
                    this._logWindow = new LogWindow();
                }
                this._logWindow.Show();
                Glog.Message(MyTraceCategory.Config,"Log Window Initialized...");
                
                Glog.Message(MyTraceCategory.Config,new List<string>() {
                    "--- Current configuration settings ---",
                    "Dnf2011SystemIniPath="+ this._myCfg.Dnf2011SystemIniPath,
                    "Dnf2011ExePath="+ this._myCfg.Dnf2011ExePath,
                    "Dnf2011ExeCommandLineArgs="+ this._myCfg.Dnf2011ExeCommandLineArgs,
                    "EnableSystemIniSteamCloudSync="+ this._myCfg.EnableSystemIniSteamCloudSync,
                    "OpenLogWindowOnStartup="+ this._myCfg.OpenLogWindowOnStartup,
                    "Theme="+ this._myCfg.Theme,
                    "--- End of current configuration settings ---",
                });
            }
            
            Glog.Message(MyTraceCategory.General,"Initializing main window...");
            
            this._mainVm = new MainViewModel();
            this._mainVm.InitializeConfig(this._myCfg);
            
            if (this._myCfg.Theme == MyThemes.DookieNookie2001.ToString()) {
                this._mainWindow = new NukedMainWindow();
            } else {
                this._mainWindow = new MainWindow();
            }
            
            this._mainWindow.DataContext = this._mainVm;
            this._mainWindow.Show();
            this._mainWindow.Closed += this.MainWindowOnClosed;
            
            base.OnStartup(e);
        }
        private void MainWindowOnClosed(object sender, EventArgs e) {
            Glog.Message(MyTraceCategory.General,"Main window closed! Application shutting down...");
            
            this._mainVm?.MyCfg.SaveToIni();
            this._fileLogger.Flush();
            this._fileLogger.Close();
            this._logWindow?.Close();
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
