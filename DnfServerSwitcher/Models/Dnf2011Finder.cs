using System;
using System.Collections.Generic;
using System.IO;
using DnfServerSwitcher.Models.Trace;
using Microsoft.Win32;
namespace DnfServerSwitcher.Models {
    public class Dnf2011Finder {
        public string Dnf2011InstallPath { get; private set; } = "";
        public string SteamInstallPath { get; private set; } = "";
        public string Dnf2011Exe { get; private set; } = "";
        public Dictionary<string, string> Dnf2011SystemIni { get; private set; } = new Dictionary<string, string>();

        public void FindPaths() {
            try {
                this.FindWindowsDnfInstallPaths();
                this.FindWindowsSteamInstallPaths();

                if (Directory.Exists(this.Dnf2011InstallPath) == false) {
                    this.Dnf2011InstallPath = "";
                }
                if (Directory.Exists(this.SteamInstallPath) == false) {
                    this.SteamInstallPath = "";
                }

                if (string.IsNullOrWhiteSpace(this.Dnf2011InstallPath) &&
                    string.IsNullOrWhiteSpace(this.SteamInstallPath) == false) {
                    // could not find DNF install path but found Steam install path...
                    // fall back to default path inside the steam folder...
                    this.Dnf2011InstallPath = Path.Combine(this.SteamInstallPath, @"steamapps\common\Duke Nukem Forever");
                }

                this.Dnf2011Exe = string.IsNullOrWhiteSpace(this.Dnf2011InstallPath)
                    ? "" : Path.Combine(this.Dnf2011InstallPath, @"System\DukeForever.exe");

                if (File.Exists(this.Dnf2011Exe) == false) {
                    this.Dnf2011Exe = "";
                }

                this.FindWindowsDnfConfigFiles();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
            }
        }

        private void FindWindowsDnfConfigFiles() {
            if (string.IsNullOrWhiteSpace(this.SteamInstallPath)) {
                this.Dnf2011SystemIni.Clear();
                return;
            }

            string userData = Path.Combine(this.SteamInstallPath, "userdata");
            string[] userIds = Directory.GetDirectories(userData);

            foreach (string userId in userIds) {
                string path = Path.Combine(userData, userId, @"57900\remote");
                if (Directory.Exists(path)) {
                    this.Dnf2011SystemIni.Add(userId, Path.Combine(path,"system.ini"));
                }
            } 
        }
        
        private void FindWindowsSteamInstallPaths() {
            RegistryKey? hklm = null;
            RegistryKey? key = null;
            try {
                this.SteamInstallPath = "";
                
                // on a 64 bit system, make sure to look into HKLM\Wow6432Node\SOFTWARE since Steam is 32 bit
                // on a 32 bit system, just use default location
                hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, 
                    Environment.Is64BitOperatingSystem ? RegistryView.Registry32 : RegistryView.Default);  
                key = hklm.OpenSubKey(@"SOFTWARE\Valve\Steam");
                if (key == null && Environment.Is64BitOperatingSystem)  {
                    hklm.Dispose();
                    // fallback to the normal 64 bit HKLM\SOFTWARE in case in the future Steam is 64 bit?...
                    hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    key = hklm.OpenSubKey(@"SOFTWARE\Valve\Steam");
                }
                if (key == null) {
                    return;
                }

                object? pathObj = key.GetValue("InstallPath");

                if (pathObj is not string pathStr)
                    return;

                this.SteamInstallPath = pathStr;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Unexpected error locating Steam install path! Unable to automatically determine install paths...", ex);
                this.SteamInstallPath = "";
            } finally {
                key?.Dispose();
                hklm?.Dispose();
            }
        }
        
        private void FindWindowsDnfInstallPaths() {
            RegistryKey? hklm = null;
            RegistryKey? key = null;
            try {
                this.Dnf2011InstallPath = "";

                // TODO: not sure if this is correct for 32 bit windows? need to test...
                hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                    Environment.Is64BitOperatingSystem
                        ? RegistryView.Registry64 // on a 64 bit system, make sure to look in HKLM\SOFTWARE and not HKLM\Wow6432Node\SOFTWARE
                        : RegistryView.Default);  // on a 32 bit system, just use default location
                key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 57900");
                if (key == null)
                    return;

                object? pathObj = key.GetValue("InstallLocation");

                if (pathObj is not string pathStr)
                    return;

                this.Dnf2011InstallPath = pathStr;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Unexpected error locating DNF2011 install path! Unable to automatically determine install paths...", ex);
                this.Dnf2011InstallPath = "";
            } finally {
                key?.Dispose();
                hklm?.Dispose();
            }
        }
    }
}
