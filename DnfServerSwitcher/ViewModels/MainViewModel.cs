﻿using System;
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

        private DnfServerSwitcherConfig _myCfg = new DnfServerSwitcherConfig();

        public string Dnf2011ExePath {
            get => this._myCfg.Dnf2011ExePath;
            set {
                if (this._myCfg.Dnf2011ExePath != value) {
                    this._myCfg.Dnf2011ExePath = value;
                    this.OnPropertyChanged();
                    this._cmdLaunchNormal?.RefreshCanExecute();
                    this._cmdLaunchDeprecated?.RefreshCanExecute();
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

        public MainViewModel() {
            this.Initialize();
        }

        private void Initialize() {
            if (!this._myCfg.LoadFromIni()) {
                // could not load config file.. try to auto detect paths!
                Dnf2011Finder df = new Dnf2011Finder();
                df.FindPaths();
                if (string.IsNullOrWhiteSpace(df.Dnf2011Exe) == false) {
                    this._myCfg.Dnf2011ExePath = df.Dnf2011Exe;
                }
                if (df.Dnf2011SystemIni.Count > 0) {
                    // TODO... give some user prompt to select what user's data to use?...
                    // for now just default to first value i guess...
                    this._myCfg.Dnf2011SystemIniPath = df.Dnf2011SystemIni.First().Value;
                }
                this._myCfg.SaveToIni();
            }

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
            try {
                if (!this.CheckExeExists()) return;

                if (this.CheckSystemIniExists() == true) {
                    IniDocument data = DnfIniParseHelper.ParseDnf2011SystemIni(this.Dnf2011SystemIniPath);
                    data["Engine.Engine"]["NetworkDevice"] = new IniKey("NetworkDevice", "Engine.AgentNetDriver");
                    // set net speeds and client rates to recommended values
                    data["Engine.Player"]["ConfiguredInternetSpeed"] = new IniKey("ConfiguredInternetSpeed", "20000");
                    data["Engine.Player"]["ConfiguredLanSpeed"] = new IniKey("ConfiguredLanSpeed", "20000");
                    data["Engine.TcpNetDriver"]["MaxClientRate"] = new IniKey("MaxClientRate", "20000");
                    data["Engine.AgentNetDriver"]["MaxClientRate"] = new IniKey("MaxClientRate", "20000");
                    DnfIniParseHelper.WriteDnf2011SystemIni(this.Dnf2011SystemIniPath, data);
                }

                Process.Start(this.Dnf2011ExePath);
            } catch (Exception ex) {
                MessageBox.Show("An error has occurred..." + Environment.NewLine + Environment.NewLine +
                    ExceptionHelper.GetExceptionAsString(ex), "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static string GetSha256(string filePath) {
            if (!File.Exists(filePath)) return "";
            using (SHA256 cks = SHA256.Create())
            {
                byte[] cksBytes = cks.ComputeHash(File.ReadAllBytes(filePath));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < cksBytes.Length; i++)
                {
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
                if (!File.Exists(filePathMod)) return;
                FileInfo finfo = new FileInfo(this.Dnf2011ExePath);
                
                // check DNF directory exists
                if (finfo.Directory?.Exists != true) return;
                FileInfo finfo2 = new FileInfo(Path.Combine(finfo.Directory.FullName, fileNameMod));
                
                // check if the mod file already exists in the DNF directory
                if (finfo2.Exists) {
                    if (!verifyChecksumIfExist) return;
                    
                    // verify file checksum
                    string cks = GetSha256(finfo.FullName);
                    string cks2 = GetSha256(finfo2.FullName);
                    if (string.IsNullOrWhiteSpace(cks) == false &&
                        string.IsNullOrWhiteSpace(cks2) == false &&
                        cks == cks2) {
                        // checksums match, exit
                        return;
                    }
                }
                File.Copy(filePathMod, finfo2.FullName, overwrite:true);
            } catch (Exception) {
                // TODO do something with the exception?...
            }
        }

        private void LaunchDnf2011Deprecated() {
            try {
                if (!this.CheckExeExists()) return;

                if (this.CheckSystemIniExists() == true) {
                    IniDocument data = DnfIniParseHelper.ParseDnf2011SystemIni(this.Dnf2011SystemIniPath);
                    data["Engine.Engine"]["NetworkDevice"] = new IniKey("NetworkDevice", "Engine.TCPNetDriver");
                    // set net speeds and client rates to recommended values
                    data["Engine.Player"]["ConfiguredInternetSpeed"] = new IniKey("ConfiguredInternetSpeed", "20000");
                    data["Engine.Player"]["ConfiguredLanSpeed"] = new IniKey("ConfiguredLanSpeed", "20000");
                    data["Engine.TcpNetDriver"]["MaxClientRate"] = new IniKey("MaxClientRate", "20000");
                    data["Engine.AgentNetDriver"]["MaxClientRate"] = new IniKey("MaxClientRate", "20000");
                    DnfIniParseHelper.WriteDnf2011SystemIni(this.Dnf2011SystemIniPath, data);
                }

                this.TryCopyDeprecatedFiles(true);
                
                Process.Start(this.Dnf2011ExePath);
            } catch (Exception ex) {
                MessageBox.Show("An error has occurred..." + Environment.NewLine + Environment.NewLine +
                    ExceptionHelper.GetExceptionAsString(ex), "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

    }
}
