using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DnfServerSwitcher.Models;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.KrazyIni.Data;
using DnfServerSwitcher.Models.SteamApi;
using DnfServerSwitcher.Models.Trace;
using DnfServerSwitcher.Views;
using DukNuk.Wpf.Mvvm;
using Microsoft.Win32;
using Steamworks;

namespace DnfServerSwitcher.ViewModels {
    public class MainViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #region Commands

        
        public DukCommand CmdQuit { get; }
        public DukCommand CmdShowHelpFaq { get; }
        public DukCommand CmdShowHelpAbout { get; }
        public DukCommand CmdShowHelpLogWindow { get; }

        public NukCommand CmdLaunchNormal { get; }
        public NukCommand CmdLaunchDeprecated { get; }
        public DukCommand CmdBrowseExe { get; }
        public DukCommand CmdBrowseSystemIni { get; }
        public NukCommand CmdDeleteRemoteCacheVdf { get; }
        public DukCommand CmdAutoDetectInstallPaths { get; }

        public DukCommand CmdOpenDnfMapsWebsite { get; }
        public NukCommand CmdQuickPlayMap { get; }

        #endregion

        public MainViewModel() {
            this.AppVersion = ((Application.Current as App)?.AppVersion) ?? "V - Unknown?";
            
            // initialize commands...
            this.CmdQuit = new DukCommand(() => { (Application.Current as App)?.StartShutdown(); } ) ;
            this.CmdShowHelpFaq = new DukCommand(() => { (Application.Current as App)?.ShowHelpFaqWindow(); });
            this.CmdShowHelpAbout = new DukCommand(() => { (Application.Current as App)?.ShowHelpAboutWindow(); });
            this.CmdShowHelpLogWindow = new DukCommand(() => { (Application.Current as App)?.ShowLogWindow(); });
            
            this.CmdLaunchNormal = new NukCommand(this.LaunchDnf2011Normal) { CanExec = false };
            this.CmdLaunchDeprecated = new NukCommand(this.LaunchDnf2011Deprecated) { CanExec = false };
            this.CmdBrowseExe = new DukCommand(this.BrowseDnf2011Exe);
            this.CmdBrowseSystemIni = new DukCommand(this.BrowseDnf2011SystemIni);
            
            this.CmdOpenDnfMapsWebsite = new DukCommand(this.OpenDnfMapWebsite);
            this.CmdQuickPlayMap = new NukCommand(this.OpenDnfMap);
            
            this.CmdDeleteRemoteCacheVdf = new NukCommand(this.DeleteRemoteCacheVdf);
            this.CmdAutoDetectInstallPaths = new DukCommand(this.AutoDetectInstallPaths);


        }
        
        #region path settings

        public bool OpenLogWindowOnStartup {
            get => this._openLogWindowOnStartup;
            set => this.SetField(ref this._openLogWindowOnStartup, value);
        }
        private bool _openLogWindowOnStartup = false;

        public bool EnableSteamCloudSync {
            get => this._enableSteamCloudSync;
            set => this.SetField(ref this._enableSteamCloudSync, value);
        }
        private bool _enableSteamCloudSync = false;
        
        public string Dnf2011ExePath {
            get => this._dnf2011ExePath;
            set {
                this.SetField(ref this._dnf2011ExePath, value);
                // recalculate paths when property is set
                this.RefreshDnf2011Exe();
                this.RefreshDnf2011MapsDirectory();
                this.RefreshCommands();
            }
        }
        private string _dnf2011ExePath = ""; 

        public string Dnf2011ExeCommandLineArgs {
            get => this._dnf2011ExeCommandLineArgs;
            set => this.SetField(ref this._dnf2011ExeCommandLineArgs, value);
        }
        private string _dnf2011ExeCommandLineArgs = "";

        public string Dnf2011SystemIniPath {
            get => this._dnf2011SystemIniPath;
            set {
                this.SetField(ref this._dnf2011SystemIniPath, value);
                this.RefreshDnf2011SystemIni();;
                this.RefreshDnf2011UserIni();
                this.RefreshRemoteCacheVdf();
                this.RefreshCommands();
            }
        }
        private string _dnf2011SystemIniPath = "";

        public string Dnf2011MapsDirectory {
            get => this._dnf2011MapsDirectory;
            private set => this.SetField(ref this._dnf2011MapsDirectory, value);
        }
        private string _dnf2011MapsDirectory = "";

        public string Dnf2011UserIniPath {
            get => this._dnf2011UserIniPath;
            private set => this.SetField(ref this._dnf2011UserIniPath, value);
        }
        private string _dnf2011UserIniPath = "";
        
        public string Dnf2011RemoteCacheVdf {
            get => this._dnf2011RemoteCacheVdf;
            private set => this.SetField(ref this._dnf2011RemoteCacheVdf, value);
        }
        private string _dnf2011RemoteCacheVdf = "";

        public bool IsValidDnf2011Exe {
            get => this._isValidDnf2011Exe;
            private set => this.SetField(ref this._isValidDnf2011Exe, value);
        }
        private bool _isValidDnf2011Exe = false;
        
        public bool IsValidDnf2011MapsDirectory {
            get => this._isValidDnf2011MapsDirectory;
            private set => this.SetField(ref this._isValidDnf2011MapsDirectory, value);
        }
        private bool _isValidDnf2011MapsDirectory = false;
        
        public bool IsValidDnf2011SystemIni {
            get => this._isValidDnf2011SystemIni;
            private set => this.SetField(ref this._isValidDnf2011SystemIni, value);
        }
        private bool _isValidDnf2011SystemIni = false;
        
        public bool IsValidDnf2011UserIni {
            get => this._isValidDnf2011UserIni;
            private set => this.SetField(ref this._isValidDnf2011UserIni, value);
        }
        private bool _isValidDnf2011UserIni = false;
        
        public bool IsValidDnf2011RemoteCacheVdf {
            get => this._isValidDnf2011RemoteCacheVdf;
            private set => this.SetField(ref this._isValidDnf2011RemoteCacheVdf, value);
        }
        private bool _isValidDnf2011RemoteCacheVdf = false;
        
        
        
        
        #endregion
        
        #region UserIni settings

        public int UserFieldOfView {
            get => this._userFieldOfView;
            set => this.SetField(ref this._userFieldOfView, value);
        }
        private int _userFieldOfView = 0;
        
        public string DeprecatedPlayerName {
            get => this._deprecatedPlayerName;
            set => this.SetField(ref this._deprecatedPlayerName, value);
        }
        private string _deprecatedPlayerName = "";
        
        public Key DeprecatedPlayerNameHotkey {
            get => this._deprecatedPlayerNameHotkey;
            set => this.AttemptToSetHotkey(value);
        }
        private Key _deprecatedPlayerNameHotkey = Key.None;
        
        #endregion

        public string AppVersion {
            get => this._appVersion;
            private set => this.SetField(ref this._appVersion, value);
        }
        private string _appVersion = "";
        
        
        public SteamApiHelper? SteamApi {
            get => this._steamApi;
            set => this.SetField(ref this._steamApi, value);
        }
        private SteamApiHelper? _steamApi = null;
        
        private DnfHotkeyHelper _hotkeyHelper = new DnfHotkeyHelper();

        public List<GameDifficultySelection> AvailableDifficulties { get; } = new List<GameDifficultySelection>() {
            new GameDifficultySelection("Piece of Cake", 0),
            new GameDifficultySelection("Let's Rock", 1),
            new GameDifficultySelection("Come Get Some", 2),
            new GameDifficultySelection("Damn I'm Good", 3),
        };
        public int SelectedMapDifficulty {
            get => this._selectedMapDifficulty;
            set => this.SetField(ref this._selectedMapDifficulty, value);
        }
        private int _selectedMapDifficulty = 2;


        
        public void LoadFromConfig(DnfServerSwitcherConfig cfg) {
            this.EnableSteamCloudSync = cfg.EnableSystemIniSteamCloudSync;
            this.OpenLogWindowOnStartup = cfg.OpenLogWindowOnStartup;
            
            // path settings
            this.Dnf2011ExePath = cfg.Dnf2011ExePath;
            this.Dnf2011ExeCommandLineArgs = cfg.Dnf2011ExeCommandLineArgs;
            this.Dnf2011SystemIniPath = cfg.Dnf2011SystemIniPath;
           
            // user ini settings
            this.UserFieldOfView = cfg.UserFieldOfView;
            this.DeprecatedPlayerName = cfg.DeprecatedPlayerName;
            Key key = DnfHotkeyHelper.GetKeyFromIniString(cfg.DeprecatedPlayerNameHotkey);
            this.DeprecatedPlayerNameHotkey = key;
        }

        public void SaveToConfig(DnfServerSwitcherConfig cfg) {
            cfg.EnableSystemIniSteamCloudSync = this.EnableSteamCloudSync;
            cfg.OpenLogWindowOnStartup = this.OpenLogWindowOnStartup;
            
            // path settings
            cfg.Dnf2011ExePath = this.Dnf2011ExePath;
            cfg.Dnf2011ExeCommandLineArgs = this.Dnf2011ExeCommandLineArgs;
            cfg.Dnf2011SystemIniPath = this.Dnf2011SystemIniPath;
            
            // user ini settings
            cfg.UserFieldOfView = this.UserFieldOfView;
            cfg.DeprecatedPlayerName = this.DeprecatedPlayerName;
            string hotkey = DnfHotkeyHelper.GetKeyIniString(this.DeprecatedPlayerNameHotkey);
            cfg.DeprecatedPlayerNameHotkey = hotkey;
        }
        
        private void RefreshDnf2011Exe() {
            try {
                this.IsValidDnf2011Exe = File.Exists(this.Dnf2011ExePath);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
                this.IsValidDnf2011Exe = false;
            }
        }

        private void RefreshDnf2011MapsDirectory() {
            try {
                this.Dnf2011MapsDirectory = "";
                if (string.IsNullOrWhiteSpace(this.Dnf2011ExePath) == false) {
                    FileInfo finfo = new FileInfo(this.Dnf2011ExePath);
                    if (finfo.Directory?.Parent != null) {
                        this.Dnf2011MapsDirectory = Path.Combine(finfo.Directory.Parent.FullName, "Maps");
                    }
                }
                
                this.IsValidDnf2011MapsDirectory = Directory.Exists(this.Dnf2011MapsDirectory);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
                this.IsValidDnf2011MapsDirectory = false;
            }
        }
        
        private void RefreshDnf2011SystemIni() {
            try {
                this.IsValidDnf2011SystemIni = File.Exists(this.Dnf2011SystemIniPath);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
                this.IsValidDnf2011SystemIni = false;
            }
        }

        private void RefreshDnf2011UserIni() {
            try {
                this.Dnf2011UserIniPath = "";
                if (string.IsNullOrWhiteSpace(this.Dnf2011SystemIniPath) == false) {
                    FileInfo finfo = new FileInfo(this.Dnf2011SystemIniPath);
                    if (finfo.Directory != null) {
                        this.Dnf2011UserIniPath = Path.Combine( finfo.Directory.FullName, "user.ini");
                    }
                }

                this.IsValidDnf2011UserIni = File.Exists(this.Dnf2011UserIniPath);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
                this.IsValidDnf2011UserIni = false;
            }
        }

        private void RefreshRemoteCacheVdf() {
            try {

                this.IsValidDnf2011SystemIni = File.Exists(this.Dnf2011SystemIniPath);

                this.Dnf2011UserIniPath = "";
                this.Dnf2011RemoteCacheVdf = "";
                if (string.IsNullOrWhiteSpace(this.Dnf2011SystemIniPath) == false) {
                    FileInfo finfo = new FileInfo(this.Dnf2011SystemIniPath);
                    if (finfo.Directory != null) {
                        this.Dnf2011UserIniPath = Path.Combine( finfo.Directory.FullName, "user.ini");
                    }
                    if (finfo.Directory?.Parent != null) {
                        this.Dnf2011RemoteCacheVdf = Path.Combine(finfo.Directory.Parent.FullName, "remotecache.vdf");
                    }
                }

                this.IsValidDnf2011UserIni = File.Exists(this.Dnf2011UserIniPath);
                
                
                this.IsValidDnf2011RemoteCacheVdf = File.Exists(this.Dnf2011RemoteCacheVdf);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
                this.IsValidDnf2011SystemIni = false;
                this.IsValidDnf2011UserIni = false;
            }
        }

        private void RefreshCommands() {
            this.CmdLaunchNormal.CanExec = this.IsValidDnf2011Exe && this.IsValidDnf2011SystemIni;
            this.CmdLaunchDeprecated.CanExec = this.IsValidDnf2011Exe && this.IsValidDnf2011SystemIni;
            this.CmdQuickPlayMap.CanExec = this.IsValidDnf2011Exe && this.IsValidDnf2011MapsDirectory;
            this.CmdDeleteRemoteCacheVdf.CanExec = this.IsValidDnf2011RemoteCacheVdf;
        }
        
        #region Launch game
        
        private bool LaunchValidateContinue() {
            try {
                this.RefreshDnf2011Exe();
                this.RefreshDnf2011SystemIni();
                this.RefreshDnf2011UserIni();
                this.RefreshCommands();

                if (this.IsValidDnf2011Exe == false) {
                    MessageBox.Show(
                        "DukeForever.exe not found! Make sure the path",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (this.IsValidDnf2011SystemIni == false) {
                    if (MessageBox.Show(
                            "system.ini file not found! Attempt to launch game anyway?",
                            "Error", MessageBoxButton.YesNo, MessageBoxImage.Error)
                        == MessageBoxResult.No) 
                        return false;
                }
                
                if (this.IsValidDnf2011UserIni == false) {
                    if (MessageBox.Show(
                            "user.ini file not found! Attempt to launch game anyway?",
                            "Error", MessageBoxButton.YesNo, MessageBoxImage.Error)
                        == MessageBoxResult.No) 
                        return false;
                }

                return true;
            }  catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
                return false;
            }
        }

        private void LaunchDnf2011Normal() {
            try {
                if (!this.LaunchValidateContinue())
                    return;

                if (this.IsValidDnf2011SystemIni) {
                    if (!DnfSystemIniHelper.SetNormalMode(this.Dnf2011SystemIniPath))
                        return;
                }

                this.LaunchSyncSteamCloud();
                this.LaunchStartProcess();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }

        private void LaunchDnf2011Deprecated() {
            try {
                if (!this.LaunchValidateContinue())
                    return;

                if (this.IsValidDnf2011SystemIni) {
                    if (!DnfSystemIniHelper.SetDeprecatedMode(this.Dnf2011SystemIniPath))
                        return;
                }
                if (this.IsValidDnf2011UserIni) {
                    DnfUserIniHelper.SetUserIniData(this.Dnf2011UserIniPath,
                        this.DeprecatedPlayerNameHotkey,
                        this.DeprecatedPlayerName,
                        this.UserFieldOfView);
                }

                DnfSystemIniHelper.TryCopyDeprecatedFiles(this.Dnf2011ExePath, true);

                this.LaunchSyncSteamCloud();
                this.LaunchStartProcess();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }

        private void LaunchStartProcess() {
            Glog.Message(MyTraceCategory.General, $"Starting DNF2011 at path={this.Dnf2011ExePath}");
            Glog.Message(MyTraceCategory.General, $"Command line args={this.Dnf2011ExeCommandLineArgs}");
            Process.Start(this.Dnf2011ExePath, this.Dnf2011ExeCommandLineArgs);
        } 

        private void LaunchSyncSteamCloud() {
            if (this.EnableSteamCloudSync == false || this.SteamApi == null)
                return;
            
            if (this.IsValidDnf2011SystemIni) {
                Glog.Message(MyTraceCategory.General, $"Attempting to write system.ini to Steam Remote Storage...");
                if (this.SteamApi.WriteFileToRemote(this.Dnf2011SystemIniPath, "system.ini") != true) {
                    MessageBox.Show("An error has occurred trying to write the system.ini file to Steam Remote Storage... See log file for more info.", "Steam Remote Storage Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (this.IsValidDnf2011UserIni) {
                Glog.Message(MyTraceCategory.General, $"Attempting to write system.ini to Steam Remote Storage...");
                if (this.SteamApi.WriteFileToRemote(this.Dnf2011UserIniPath, "user.ini") != true) {
                    MessageBox.Show("An error has occurred trying to write the system.ini file to Steam Remote Storage... See log file for more info.", "Steam Remote Storage Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
        #endregion
        
        #region File browse
        
        private void BrowseDnf2011Exe() {
            try {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "DNF Executable|DukeForever.exe|Executable|*.exe";
                if (string.IsNullOrWhiteSpace(this.Dnf2011ExePath) == false) {
                    FileInfo finfo = new FileInfo(this.Dnf2011ExePath);
                    if (finfo.Directory?.Exists == true) {
                        ofd.InitialDirectory = finfo.Directory.FullName;
                    }
                }
                if (ofd.ShowDialog() == true) {
                    this.Dnf2011ExePath = ofd.FileName;
                }
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }

        private void BrowseDnf2011SystemIni() {
            try {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "DNF System.ini|system.ini|INI file|*.ini";
                if (string.IsNullOrWhiteSpace(this.Dnf2011SystemIniPath) == false) {
                    FileInfo finfo = new FileInfo(this.Dnf2011SystemIniPath);
                    if (finfo.Directory?.Exists == true) {
                        ofd.InitialDirectory = finfo.Directory.FullName;
                    }
                }
                if (ofd.ShowDialog() == true) {
                    this.Dnf2011SystemIniPath = ofd.FileName;
                }
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }
        
        #endregion

        #region RemoteCacheVdf

        private void DeleteRemoteCacheVdf() {
            try {
                this.RefreshDnf2011Exe();
                this.RefreshRemoteCacheVdf();

                if (this.IsValidDnf2011RemoteCacheVdf == false)
                    return;
                string path = this.Dnf2011RemoteCacheVdf;
                
                Glog.Message(MyTraceCategory.General, $"Found remotecache.vdf file at path={path}");
                File.Delete(path);
                Glog.Message(MyTraceCategory.General, $"Deleted remotecache.vdf file at path={path}");
                
                this.RefreshRemoteCacheVdf();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }
        
        #endregion

        #region Single Player Map
        
        private void OpenDnfMapWebsite() {
            try {
                Uri dnfmaps = new Uri(@"https://dnfmaps.com/dnf-2011/", UriKind.Absolute);
                Process.Start(dnfmaps.AbsoluteUri);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }

        private void OpenDnfMap() {
            try {
                this.RefreshDnf2011Exe();
                this.RefreshDnf2011MapsDirectory();

                if (this.IsValidDnf2011MapsDirectory == false)
                    return;

                string mapDir = this._dnf2011MapsDirectory;
                
                Glog.Message(MyTraceCategory.General, $"Found map folder at path={mapDir}");

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = mapDir;
                ofd.Filter = "DNF map file|*.dnf";
                if (ofd.ShowDialog() == true) {
                    FileInfo finfo = new FileInfo(ofd.FileName);

                    Glog.Message(MyTraceCategory.General, $"User selected map file at path={finfo.FullName}");

                    string mapName = Path.GetFileNameWithoutExtension(finfo.Name);
                    string args = $"{mapName}?Difficulty={this.SelectedMapDifficulty}";

                    Glog.Message(MyTraceCategory.General, $"Starting DNF2011 with arguments={args}");
                    Process.Start(this.Dnf2011ExePath, args);
                }
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }
        
        #endregion

        private void AutoDetectInstallPaths() {
            try {
                Dnf2011Finder df = new Dnf2011Finder();
                df.FindPaths(this._steamApi);

                if (string.IsNullOrWhiteSpace(df.Dnf2011Exe) == false) {
                    this.Dnf2011ExePath = df.Dnf2011Exe;
                }
                if (string.IsNullOrWhiteSpace(df.Dnf2011SystemIni) == false) {
                    this.Dnf2011SystemIniPath = df.Dnf2011SystemIni;
                }
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
            }
        }

        private void AttemptToSetHotkey(Key hotkey) {
            
            this.RefreshDnf2011UserIni();
            if (this.IsValidDnf2011UserIni) {
                if (!this._hotkeyHelper.RefreshBindingsFromUserIni(this.Dnf2011UserIniPath)) {
                    MessageBox.Show($"Could not parse user.ini to validate hotkey!", "Hotkey error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int vk = DnfHotkeyHelper.Key2VirtualKey(hotkey);
                if (this._hotkeyHelper.CurrentBindings.TryGetValue(vk, out string binding)) {
                    if (binding.StartsWith("name") == false) {
                        MessageBox.Show($"Hotkey is already bound to \"{binding}\", choose a different hotkey!", "Hotkey error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                // finally set hotkey..
                this.SetField(ref this._deprecatedPlayerNameHotkey, hotkey, nameof(this.DeprecatedPlayerNameHotkey));
            }
        }
    }
}
