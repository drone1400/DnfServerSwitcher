using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using DnfServerSwitcher.Models;
using DnfServerSwitcher.Models.Trace;
using DnfServerSwitcher.ViewModels;
using DnfServerSwitcher.Views;
using DnfServerSwitcher.Views.NukedWindows;
using DnfServerSwitcher.Views.Windows;
using DukNuk.Wpf.Helpers;

namespace DnfServerSwitcher {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public const int DNF2011_STEAMAPPID = 57900;

        public string AppBaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public string AppVersion { get; } = "";


        private Window? _mainWindow;
        private Window? _logWindow;
        private Window? _faqWindow;
        private Window? _aboutWindow;
        private bool _inhibitShutdown = false;

        private DnfServerSwitcherConfig _myCfg = new DnfServerSwitcherConfig();
        private MainViewModel? _mainVm;
        private MyTraceListenerFileLogger _fileLogger ;
        private bool _themeLoaded = false;
        
        
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
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.AppVersion = $"V{version}";
            
            this._fileLogger = new MyTraceListenerFileLogger(this.AppBaseDirectory, "DnfSS_ErrorLog", false) {
                FlushAfterEachMessage = true,
                MaxTraceLevel = MyTraceLevel.Error,
            };
            
            MyTrace.Global.AddListener(this._fileLogger);
            
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
            // load config from file
            this.LoadConfig();
            
            // load theme
            this.LoadTheme(defaultToNormalWpf:true);
            
            // initialize view model
            this._mainVm = new MainViewModel();
            this._mainVm.InitializeConfig(this._myCfg);
            
            // show windows
            if (this._myCfg.OpenLogWindowOnStartup) {
                this.ShowLogWindow();
            }
            this.ShowMainWindow();
            
            base.OnStartup(e);
        }

        private void LoadConfig() {
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

        private void LoadTheme(bool defaultToNormalWpf) {
            try {
                string[] themes = ThemeManager.Default.GetAvailableThemeColors();

                if (themes.Contains(this._myCfg.Theme) == false) {
                    this._myCfg.Theme = defaultToNormalWpf ? "" : themes.First();
                }

                if (this._myCfg.IsDefaultWpfTheme == false) {
                    ThemeManager.Default.InitializeAppResources(this._myCfg.Theme);
                    this._themeLoaded = true;
                }
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Error loading theme...", ex);
            }
        }

        private void UnloadTheme() {
            try {
                ThemeManager.Default.FreeFromAppResources();
                this._themeLoaded = false;
             } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Error unloading theme...", ex);
            }
        }
        
        private void MainWindowOnClosed(object sender, EventArgs e) {
            this._mainWindow = null;
            
            if (this._inhibitShutdown) return;
            
            Glog.Message(MyTraceCategory.General,"Main window closed! Application shutting down...");
            this.StartShutdown();
        }

        public void StartShutdown() {
            this._mainVm?.MyCfg.SaveToIni();
            this._fileLogger.Flush();
            this._fileLogger.Close();
            this._logWindow?.Close();
            this.Shutdown();
        }
        
        #region showing windows
        
        private void ShowLogWindow() {
            if (this._logWindow != null) return;
            
            this._logWindow = this._myCfg.IsDefaultWpfTheme ? new LogWindow() : new NukedLogWindow();
            this._logWindow.Closed += ( sender,  args) => {
                this._logWindow = null;
            };
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

        private void ShowMainWindow() {
            if (this._mainWindow != null) return;
            
            Glog.Message(MyTraceCategory.General,"Initializing main window...");

            this._mainWindow = this._myCfg.IsDefaultWpfTheme ? new MainWindow() : new NukedMainWindow();
            this._mainWindow.DataContext = this._mainVm;
            this._mainWindow.Closed += this.MainWindowOnClosed;
            this._mainWindow.Show();
        }

        public void ShowHelpFaqWindow() {
            if (this._faqWindow != null) return;
            
            this._faqWindow = this._myCfg.IsDefaultWpfTheme ? new FaqWindow() : new NukedFaqWindow();
            this._faqWindow.Closed += ( sender,  args) => {
                this._faqWindow = null;
            };
            this._faqWindow.ShowDialog();
        }
        
        public void ShowHelpAboutWindow() {
            if (this._aboutWindow != null) return;
            
            this._aboutWindow = this._myCfg.IsDefaultWpfTheme ? new AboutWindow() : new NukedAboutWindow();
            this._aboutWindow.DataContext = this._mainVm;
            this._aboutWindow.Closed += ( sender,  args) => {
                this._aboutWindow = null;
            };
            this._aboutWindow.ShowDialog();
        }
        
        #endregion

        public void ThemeSelectNext() {
            string crt = ThemeManager.Default.CurrentTheme;
            bool found = false;
            
            string[] themes = ThemeManager.Default.GetAvailableThemeColors();
            
            for (int i = 0; i < themes.Length; i++) {
                if (themes[i] == crt) {
                    int index = i + 1;
                    if (index >= themes.Length) index = 0;
                    crt = themes[index];
                    found = true;
                    break;
                }
            }

            if (found == false) {
                crt = themes[0];
            }
            this._myCfg.Theme = crt;
            ThemeManager.Default.SetTheme(crt);
        }

        public void ThemeToggleOnOff() {
            try {
                this._inhibitShutdown = true;
                bool showLog = this._logWindow != null;

                this._faqWindow?.Close();
                this._aboutWindow?.Close();
                this._logWindow?.Close();
                this._mainWindow?.Close();

                if (this._themeLoaded) {
                    this.UnloadTheme();
                    this._myCfg.Theme = "";
                } else {
                    this._myCfg.Theme = ThemeManager.Default.GetAvailableThemeColors().First();
                    this.LoadTheme(defaultToNormalWpf:false);
                }
                
                if (showLog) 
                    this.ShowLogWindow();
                this.ShowMainWindow();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Error toggling theme mode..", ex);
            } finally {
                this._inhibitShutdown = false;
            }
        }
    }
    

}
