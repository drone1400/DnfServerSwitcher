using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DnfServerSwitcher.Models.SteamApi;
using DnfServerSwitcher.Models.Trace;
using Microsoft.Win32;
using Steamworks;
namespace DnfServerSwitcher.Models {
    public class Dnf2011Finder {
        public string Dnf2011InstallPath { get; private set; } = "";
        public string SteamInstallPath { get; private set; } = "";
        public string Dnf2011Exe { get; private set; } = "";
        public string Dnf2011SystemIni { get; private set; } = "";

        public const string DNF_SUBFOLDER_GAME_EXE = @"System\DukeForever.exe";
        public const string DNF_SUBFOLDER_GAME = @"steamapps\common\Duke Nukem Forever";
        public const string DNF_SUBFOLDER_REMOTE = @"57900\remote";

        public void FindPaths(SteamApiHelper? helper = null) {
            try {
                // try to find steam from system registry...
                this.SteamInstallPath = FindSteamInstallPathsFromWindowsRegistry();
                if (Directory.Exists(this.SteamInstallPath) == false) {
                    this.SteamInstallPath = "";
                }
                
                // try to find DNF from windows registry
                this.Dnf2011InstallPath = FindDnfInstallPathsFromWindowsRegistry();
                this.Dnf2011Exe = Path.Combine(this.Dnf2011InstallPath, Dnf2011Finder.DNF_SUBFOLDER_GAME_EXE);
                if (Directory.Exists(this.Dnf2011InstallPath) == false || File.Exists(this.Dnf2011Exe) == false) {
                    this.Dnf2011InstallPath = "";
                    this.Dnf2011Exe = "";
                }

                // fallback to default Steam install folder...
                if ((string.IsNullOrWhiteSpace(this.Dnf2011InstallPath) || string.IsNullOrWhiteSpace(this.Dnf2011Exe))&&
                    string.IsNullOrWhiteSpace(this.SteamInstallPath) == false) {
                    // could not find DNF install path but found Steam install path...
                    // fall back to default path inside the steam folder...
                    this.Dnf2011InstallPath = Path.Combine(this.SteamInstallPath, Dnf2011Finder.DNF_SUBFOLDER_GAME);
                    this.Dnf2011Exe = Path.Combine(this.Dnf2011InstallPath, Dnf2011Finder.DNF_SUBFOLDER_GAME_EXE);
                }
                if (Directory.Exists(this.Dnf2011InstallPath) == false || File.Exists(this.Dnf2011Exe) == false) {
                    this.Dnf2011InstallPath = "";
                    this.Dnf2011Exe = "";
                }
                
                // fallback to SteamAPI
                if (helper != null && (string.IsNullOrWhiteSpace(this.Dnf2011InstallPath) || string.IsNullOrWhiteSpace(this.Dnf2011Exe)))
                {
                    try {
                        this.Dnf2011InstallPath = helper.GetDnfInstallPath();
                        this.Dnf2011Exe = Path.Combine(this.Dnf2011InstallPath, Dnf2011Finder.DNF_SUBFOLDER_GAME_EXE);
                        if (Directory.Exists(this.Dnf2011InstallPath) == false || File.Exists(this.Dnf2011Exe) == false) {
                            this.Dnf2011InstallPath = "";
                            this.Dnf2011Exe = "";
                        }
                    } catch (Exception ex) {
                        Glog.Error(MyTraceCategory.General, "Unexpected error locating DNF2011 install path from SteamAPI!", ex);
                        this.Dnf2011InstallPath = "";
                        this.Dnf2011Exe = "";
                    }
                }

                if (helper != null) {
                    try {
                        string id = helper.GetUserId32();
                        string systemIni = Path.Combine(this.SteamInstallPath, "userdata", id, @"57900\remote\system.ini");
                        if (File.Exists(systemIni)) {
                            this.Dnf2011SystemIni = systemIni;
                        }
                    } catch (Exception ex) {
                        Glog.Error(MyTraceCategory.General, "Unexpected error locating DNF2011 install path from SteamAPI!", ex);
                        this.Dnf2011SystemIni = "";
                    }
                }

                if (string.IsNullOrWhiteSpace(this.Dnf2011SystemIni)) {
                    Dictionary<string, string> inis = FindDnfSystemIniFilesFromSteam(this.SteamInstallPath);
                    // just get first result for now..
                    this.Dnf2011SystemIni = inis.First().Value;
                }


            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, ex);
            }
        }

        private static Dictionary<string,string> FindDnfSystemIniFilesFromSteam(string steamBase) {
            Dictionary<string, string> systemInis = new Dictionary<string, string>();
            
            try {
                string userData = Path.Combine(steamBase, "userdata");
                string[] userIds = Directory.GetDirectories(userData);

                foreach (string userId in userIds) {
                    string path = Path.Combine(userData, userId, @"57900\remote");
                    if (Directory.Exists(path)) {
                        systemInis.Add(userId, Path.Combine(path, "system.ini"));
                    }
                }

                return systemInis;
            }  catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Unexpected error locating system.ini!", ex);
                return systemInis;
            }
        }
        
        private static string FindSteamInstallPathsFromWindowsRegistry() {
            RegistryKey? hklm = null;
            RegistryKey? key = null;
            try {
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

                object? pathObj = key?.GetValue("InstallPath");

                if (pathObj is string pathStr)
                    return pathStr;

                return "";
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Unexpected error locating Steam install path from windows registry!", ex);
                return "";
            } finally {
                key?.Dispose();
                hklm?.Dispose();
            }
        }
        
        private static string FindDnfInstallPathsFromWindowsRegistry() {
            RegistryKey? hklm = null;
            RegistryKey? key = null;
            try {
                // TODO: not sure if this is correct for 32 bit windows? need to test...
                hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                    Environment.Is64BitOperatingSystem
                        ? RegistryView.Registry64 // on a 64 bit system, make sure to look in HKLM\SOFTWARE and not HKLM\Wow6432Node\SOFTWARE
                        : RegistryView.Default);  // on a 32 bit system, just use default location
                key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 57900");
                
                object? pathObj = key?.GetValue("InstallLocation");

                if (pathObj is string pathStr)
                    return pathStr;

                return "";
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Unexpected error locating DNF2011 install path from windows registry!", ex);
                return "";
            } finally {
                key?.Dispose();
                hklm?.Dispose();
            }
        }
    }
}
