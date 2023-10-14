using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Microsoft.Win32;

namespace DnfServerSwitcher.ViewModels {
    public class MainViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public DnfServerSwitcherConfig MyCfg { get => this._myCfg; }
        private DnfServerSwitcherConfig _myCfg = new DnfServerSwitcherConfig();
        
        public bool EnableSystemIniSteamCloudSync {
            get => this._myCfg.EnableSystemIniSteamCloudSync;
            set {
                if (this._myCfg.EnableSystemIniSteamCloudSync != value) {
                    this._myCfg.EnableSystemIniSteamCloudSync = value;
                }
            }
        }

        public string Dnf2011ExePath {
            get => this._myCfg.Dnf2011ExePath;
            set {
                if (this._myCfg.Dnf2011ExePath != value) {
                    this._myCfg.Dnf2011ExePath = value;
                    this.OnPropertyChanged();
                    this._cmdLaunchNormal?.RefreshCanExecute();
                    this._cmdLaunchDeprecated?.RefreshCanExecute();
                    this._cmdQuickOpenMap?.RefreshCanExecute();
                }
            }
        }
        
        public string Dnf2011ExeCommandLineArgs {
            get => this._myCfg.Dnf2011ExeCommandLineArgs;
            set {
                if (this._myCfg.Dnf2011ExeCommandLineArgs != value) {
                    this._myCfg.Dnf2011ExeCommandLineArgs = value;
                    this.OnPropertyChanged();
                    //this._cmdLaunchNormal?.RefreshCanExecute();
                    //this._cmdLaunchDeprecated?.RefreshCanExecute();
                }
            }
        }

        public string Dnf2011SystemIniPath {
            get => this._myCfg.Dnf2011SystemIniPath;
            set {
                if (this._myCfg.Dnf2011SystemIniPath != value) {
                    this._myCfg.Dnf2011SystemIniPath = value;
                    this.OnPropertyChanged();
                    this._cmdLaunchNormal?.RefreshCanExecute();
                    this._cmdLaunchDeprecated?.RefreshCanExecute();
                    this._cmdDeleteRemoteCacheVdf?.RefreshCanExecute();
                }
            }
        }

        public ICommand CmdLaunchNormal => this._cmdLaunchNormal ??= new MuhCommand(this.LaunchDnf2011Normal, this.CanLaunch);
        private MuhCommand? _cmdLaunchNormal;

        public ICommand CmdLaunchDeprecated => this._cmdLaunchDeprecated ??= new MuhCommand(this.LaunchDnf2011Deprecated, this.CanLaunch);
        private MuhCommand? _cmdLaunchDeprecated;

        public ICommand CmdBrowseExe => this._cmdBrowseExe ??= new MuhCommand(this.BrowseDnf2011Exe, () => true);
        private MuhCommand? _cmdBrowseExe;

        public ICommand CmdBrowseSystemIni => this._cmdBrowseSystemIni ??= new MuhCommand(this.BrowseDnf2011SystemIni, () => true);
        private MuhCommand? _cmdBrowseSystemIni;
        
        public ICommand CmdShowTroubleshootingInfo => this._cmdShowTroubleshootingInfo ??= new MuhCommand(this.ShowTroubleshootingInfoWindow, () => true);
        private MuhCommand? _cmdShowTroubleshootingInfo;
        
        public ICommand CmdDeleteRemoteCacheVdf => this._cmdDeleteRemoteCacheVdf ??= new MuhCommand(this.DeleteRemoteCacheVdf, this.CanDeleteRemoteCacheVdf);
        private MuhCommand? _cmdDeleteRemoteCacheVdf;
        
        public ICommand CmdOpenDnfMapsWebsite => this._cmdOpenDnfMapsWebsite ??= new MuhCommand(this.OpenDnfMapWebsite, () => true);
        private MuhCommand? _cmdOpenDnfMapsWebsite;
        
        public ICommand CmdQuickPlayMap => this._cmdQuickOpenMap ??= new MuhCommand(this.OpenDnfMap, this.CanOpenDnfMap);
        private MuhCommand? _cmdQuickOpenMap;

        private SteamApiHelper _steamHelper = new SteamApiHelper();
        
        public MainViewModel() {}

        public void InitializeConfig(DnfServerSwitcherConfig cfg) {
            this._myCfg = cfg;
            this._cmdLaunchNormal?.RefreshCanExecute();
            this._cmdLaunchDeprecated?.RefreshCanExecute();
            this._cmdBrowseExe?.RefreshCanExecute();
            this._cmdBrowseSystemIni?.RefreshCanExecute();
            this._cmdShowTroubleshootingInfo?.RefreshCanExecute();
            this._cmdDeleteRemoteCacheVdf?.RefreshCanExecute();
            this._cmdOpenDnfMapsWebsite?.RefreshCanExecute();
            this._cmdQuickOpenMap?.RefreshCanExecute();
        }

        private bool CanLaunch() {
            if (!File.Exists(this.Dnf2011ExePath) ||
                !File.Exists(this.Dnf2011SystemIniPath)) return false;
            return true;
        }

        private bool CheckExeExists() {
            if (File.Exists(this.Dnf2011ExePath) == false) {
                MessageBox.Show(
                    "DukeForever.exe not found! Make sure the path",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private bool? CheckSystemIniExists() {
            if (File.Exists(this.Dnf2011SystemIniPath) == false) {
                if (
                    MessageBox.Show(
                        "system.ini file not found! Attempt to launch game anyway?",
                        "Error",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error)
                    == MessageBoxResult.No) return false;
                return null;
            }
            return true;
        }

        private void LaunchDnf2011Normal() {
            if (!this.CheckExeExists()) return;

            if (this.CheckSystemIniExists() == true) {
                IniDocument? data = DnfIniParseHelper.ParseDnf2011SystemIni(this.Dnf2011SystemIniPath);
                if (data == null) {
                    MessageBox.Show("An error has occurred parsing the System.ini file, Duke will not be launched...",  "ERROR parsing System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                Glog.Message(MyTraceCategory.General, $"Successfully parsed system.ini at path={this.Dnf2011SystemIniPath}");
                
                data["Engine.Engine"]["NetworkDevice"].SetSimpleValue("Engine.AgentNetDriver");
                // set net speeds and client rates to recommended values
                data["Engine.Player"]["ConfiguredInternetSpeed"].SetSimpleValue("20000");
                data["Engine.Player"]["ConfiguredLanSpeed"].SetSimpleValue("20000");
                data["Engine.TcpNetDriver"]["MaxClientRate"].SetSimpleValue("20000");
                data["Engine.AgentNetDriver"]["MaxClientRate"].SetSimpleValue("20000");
                
                Glog.Message(MyTraceCategory.General, new List<string>() {
                    "--- Updated system.ini values are ---",
                    "[Engine.Engine]NetworkDevice=" + data["Engine.Engine"]["NetworkDevice"].GetSimpleValue(),
                    "[Engine.Player]ConfiguredInternetSpeed=" + data["Engine.Player"]["ConfiguredInternetSpeed"].GetSimpleValue(),
                    "[Engine.Player]ConfiguredLanSpeed=" + data["Engine.Player"]["ConfiguredLanSpeed"].GetSimpleValue(),
                    "[Engine.TcpNetDriver]MaxClientRate=" + data["Engine.TcpNetDriver"]["MaxClientRate"].GetSimpleValue(),
                    "[Engine.AgentNetDriver]MaxClientRate=" + data["Engine.AgentNetDriver"]["MaxClientRate"].GetSimpleValue(),
                    "--- end of updated system.ini values ---",
                });
                
                if (!DnfIniParseHelper.WriteDnf2011SystemIni(this.Dnf2011SystemIniPath, data)) {
                    MessageBox.Show("An error has occurred saving the System.ini file, Duke will not be launched...",  "ERROR saving System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                Glog.Message(MyTraceCategory.General, $"Successfully wrote updated system.ini at path={this.Dnf2011SystemIniPath}");
            }

            this.DoSteamThingy();
            
            Glog.Message(MyTraceCategory.General, $"Starting DNF2011 at path={this.Dnf2011ExePath}");
            Glog.Message(MyTraceCategory.General, $"Command line args={this.Dnf2011ExeCommandLineArgs}");
            Process.Start(this.Dnf2011ExePath, this.Dnf2011ExeCommandLineArgs);
        }

        private void LaunchDnf2011Deprecated() {
            if (!this.CheckExeExists()) return;

            if (this.CheckSystemIniExists() == true) {
                IniDocument? data = DnfIniParseHelper.ParseDnf2011SystemIni(this.Dnf2011SystemIniPath);
                if (data == null) {
                    MessageBox.Show("An error has occurred parsing the System.ini file, Duke will not be launched...",  "ERROR parsing System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                Glog.Message(MyTraceCategory.General, $"Successfully parsed system.ini at path={this.Dnf2011SystemIniPath}");
                
                data["Engine.Engine"]["NetworkDevice"].SetSimpleValue("Engine.TCPNetDriver");
                // set net speeds and client rates to recommended values
                data["Engine.Player"]["ConfiguredInternetSpeed"].SetSimpleValue("20000");
                data["Engine.Player"]["ConfiguredLanSpeed"].SetSimpleValue("20000");
                data["Engine.TcpNetDriver"]["MaxClientRate"].SetSimpleValue("20000");
                data["Engine.AgentNetDriver"]["MaxClientRate"].SetSimpleValue("20000");
                
                Glog.Message(MyTraceCategory.General, new List<string>() {
                    "--- Updated system.ini values are ---",
                    "[Engine.Engine]NetworkDevice=" + data["Engine.Engine"]["NetworkDevice"].GetSimpleValue(),
                    "[Engine.Player]ConfiguredInternetSpeed=" + data["Engine.Player"]["ConfiguredInternetSpeed"].GetSimpleValue(),
                    "[Engine.Player]ConfiguredLanSpeed=" + data["Engine.Player"]["ConfiguredLanSpeed"].GetSimpleValue(),
                    "[Engine.TcpNetDriver]MaxClientRate=" + data["Engine.TcpNetDriver"]["MaxClientRate"].GetSimpleValue(),
                    "[Engine.AgentNetDriver]MaxClientRate=" + data["Engine.AgentNetDriver"]["MaxClientRate"].GetSimpleValue(),
                    "--- end of updated system.ini values ---",
                });
                
                if (!DnfIniParseHelper.WriteDnf2011SystemIni(this.Dnf2011SystemIniPath, data)) {
                    MessageBox.Show("An error has occurred saving the System.ini file, Duke will not be launched...",  "ERROR saving System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                Glog.Message(MyTraceCategory.General, $"Successfully wrote updated system.ini at path={this.Dnf2011SystemIniPath}");
            }

            this.TryCopyDeprecatedFiles(true);
            
            this.DoSteamThingy();
            
            Glog.Message(MyTraceCategory.General, $"Starting DNF2011 at path={this.Dnf2011ExePath}");
            Glog.Message(MyTraceCategory.General, $"Command line args={this.Dnf2011ExeCommandLineArgs}");
            Process.Start(this.Dnf2011ExePath, this.Dnf2011ExeCommandLineArgs);

        }
        
        private static string GetSha256(string filePath) {
            if (!File.Exists(filePath)) return "";
            using (SHA256 cks = SHA256.Create()) {
                byte[] cksBytes = cks.ComputeHash(File.ReadAllBytes(filePath));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cksBytes.Length; i++) {
                    sb.Append(cksBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private void TryCopyDeprecatedFiles(bool verifyChecksumIfExist) {
            try {
                string fileNameMod = "dnWindow.u";
                string filePathMod = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ThirdParty", fileNameMod);
                // check mod file exists
                if (!File.Exists(filePathMod)) {
                    Glog.Message(MyTraceCategory.General, $"Local copy of dnWindow.u not found in ThirdParty folder at path={filePathMod}");
                    return;
                }
                FileInfo finfo = new FileInfo(this.Dnf2011ExePath);

                // check DNF directory exists
                if (finfo.Directory?.Exists != true) {
                    Glog.Message(MyTraceCategory.General, $"DNF2011 System directory not found?!...");
                    return;
                }
                
                FileInfo finfo2 = new FileInfo(Path.Combine(finfo.Directory.FullName, fileNameMod));

                // check if the mod file already exists in the DNF directory
                if (finfo2.Exists) {
                    Glog.Message(MyTraceCategory.General, $"The file dnWindow.u already exists in DNF2011 System folder at path={finfo2.FullName}");
                    
                    if (!verifyChecksumIfExist) return;

                    // verify file checksum
                    string cks = GetSha256(finfo.FullName);
                    string cks2 = GetSha256(finfo2.FullName);
                    
                    Glog.Message(MyTraceCategory.General, $"App's dnWindow.u SHA256 checksum={cks}");
                    Glog.Message(MyTraceCategory.General, $"DNF2011's dnWindow.u SHA256 checksum={cks2}");
                    
                    if (string.IsNullOrWhiteSpace(cks) == false &&
                        string.IsNullOrWhiteSpace(cks2) == false &&
                        cks == cks2) {
                        // checksums match, exit
                        return;
                    }
                    
                    Glog.Message(MyTraceCategory.General, $"DNF2011's dnWindow.u and App's dnWindow.u have different checksums! Replacing DNF2011's version with the App's version...");
                }
                File.Copy(filePathMod, finfo2.FullName, overwrite: true);
                
                Glog.Message(MyTraceCategory.General, $"The file dnWindow.u was copied over into DNF2011's System fodler at path={finfo2.FullName}");
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
            }
        }
        
        
        private void DoSteamThingy() {
            if (this.EnableSystemIniSteamCloudSync) {
                Glog.Message(MyTraceCategory.General, $"Attempting to write system.ini to Steam Remote Storage...");
                
                // NOTE: don't really need this, was mostly for my own debug purposes...
                // if (!this._steamHelper.ReadAllFiles()) {
                //     MessageBox.Show("An error has occurred trying to read Steam Remote Storage files for Duke Nukem Forever 2011... See log file for more info.", "Steam Remote Storage Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //     return;
                // }
                
                if (!this._steamHelper.WriteSystemIni(this.Dnf2011SystemIniPath)) {
                    MessageBox.Show("An error has occurred trying to write the system.ini file to Steam Remote Storage... See log file for more info.", "Steam Remote Storage Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        public string GetRemoteCachePath() {
            if (File.Exists(this.Dnf2011SystemIniPath) == false) return "";
            FileInfo finfo = new FileInfo(this.Dnf2011SystemIniPath);
            if (finfo.Directory?.Parent == null) return "";
            string rcvdf = Path.Combine(finfo.Directory.Parent.FullName, "remotecache.vdf");
            
            Glog.Message(MyTraceCategory.General, $"Found remotecache.vdf file at path={rcvdf}");
            return rcvdf;
        }
        
        public bool CanDeleteRemoteCacheVdf() {
            string path = this.GetRemoteCachePath();
            return string.IsNullOrWhiteSpace(path) == false && File.Exists(path);
        }

        private void DeleteRemoteCacheVdf() {
            string path = this.GetRemoteCachePath();
            if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false) return;
            File.Delete(path);
            Glog.Message(MyTraceCategory.General, $"Deleted remotecache.vdf file at path={path}");
            this._cmdDeleteRemoteCacheVdf?.RefreshCanExecute();
        }

        private void BrowseDnf2011Exe() {
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
        }

        private void BrowseDnf2011SystemIni() {
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
        }

        private void ShowTroubleshootingInfoWindow() {
            if (Application.Current is App app) {
                app.ShowTroubleshootingWindow();
            }
        }
        
        private void OpenDnfMapWebsite() {
            Uri dnfmaps = new Uri(@"https://dnfmaps.com/dnf-2011/", UriKind.Absolute);
            Process.Start(dnfmaps.AbsoluteUri);
        }

        public string GetMapFolder() {
            if (File.Exists(this.Dnf2011ExePath) == false) return "";
            FileInfo finfo = new FileInfo(this.Dnf2011ExePath);
            if (finfo.Directory?.Parent == null) return "";
            string path = Path.Combine(finfo.Directory.Parent.FullName, "Maps");
            
            Glog.Message(MyTraceCategory.General, $"Found map folder at path={path}");
            return path;
        }
        
        private bool CanOpenDnfMap() {
            string mapFolder = this.GetMapFolder();
            return string.IsNullOrWhiteSpace(mapFolder) == false && Directory.Exists(mapFolder);
        }
        
        private void OpenDnfMap() {
            string mapFolder = this.GetMapFolder();
            if (string.IsNullOrWhiteSpace(mapFolder) || Directory.Exists(mapFolder) == false) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = mapFolder;
            ofd.Filter = "DNF map file|*.dnf";
            if (ofd.ShowDialog() == true) {
                FileInfo finfo = new FileInfo(ofd.FileName);
                
                Glog.Message(MyTraceCategory.General, $"User selected map file at path={finfo.FullName}");
                
                string mapName = Path.GetFileNameWithoutExtension(finfo.Name);
                
                Glog.Message(MyTraceCategory.General, $"Starting DNF2011 with arguments={mapName}");
                Process.Start(this.Dnf2011ExePath, mapName);
            }
        }
    }
}
