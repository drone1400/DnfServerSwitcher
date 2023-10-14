using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.KrazyIni.Data;
using DnfServerSwitcher.Models.KrazyIni.Raw;
using DnfServerSwitcher.Models.Trace;

namespace DnfServerSwitcher.Models {
    public class DnfServerSwitcherConfig : INotifyPropertyChanged{
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            Glog.Message(MyTraceCategory.Config, $"Config changed: {propertyName}={value}");
            this.OnPropertyChanged(propertyName);
            return true;
        }
        
        
        public const string INI_FILE_NAME = "DnfServerSwitcher.ini";
        
        
        public string Dnf2011ExePath {
            get => this._dnf2011ExePath;
            set => this.SetField(ref this._dnf2011ExePath, value);
        }
        private string _dnf2011ExePath = ""; 
        
        public string Dnf2011ExeCommandLineArgs { 
            get => this._dnf2011ExeCommandLineArgs;
            set => this.SetField(ref this._dnf2011ExeCommandLineArgs, value);
        }
        private string _dnf2011ExeCommandLineArgs = "";
        
        public string Dnf2011SystemIniPath { 
            get => this._dnf2011SystemIniPath;
            set => this.SetField(ref this._dnf2011SystemIniPath, value);
        }
        private string _dnf2011SystemIniPath = "";

        public bool EnableSystemIniSteamCloudSync { 
            get => this._enableSystemIniSteamCloudSync;
            set => this.SetField(ref this._enableSystemIniSteamCloudSync, value);
        }
        private bool _enableSystemIniSteamCloudSync = false;
        public bool OpenLogWindowOnStartup { 
            get => this._openLogWindowOnStartup;
            set => this.SetField(ref this._openLogWindowOnStartup, value);
        }
        private bool _openLogWindowOnStartup = false;

        public string Theme { 
            get => this._theme;
            set => this.SetField(ref this._theme, value);
        }
        private string _theme = MyThemes.DookieNookie2001.ToString();

        public void SaveToIni() {
            try {
                IniDocument data = new IniDocument();
                data["Files"]["Dnf2011SystemIniPath"].SetSimpleValue(this.Dnf2011SystemIniPath);
                data["Files"]["Dnf2011ExePath"].SetSimpleValue(this.Dnf2011ExePath);
                data["Files"]["Dnf2011ExeCommandLineArgs"].SetSimpleValue(this.Dnf2011ExeCommandLineArgs);
                data["Features"]["EnableSystemIniSteamCloudSync"].SetSimpleValue(this.EnableSystemIniSteamCloudSync ? "true" : "false");
                data["Features"]["OpenLogWindowOnStartup"].SetSimpleValue(this.OpenLogWindowOnStartup ? "true" : "false");
                data["Theme"]["ThemeName"].SetSimpleValue(this.Theme);

                string iniPath = Path.Combine(((App)Application.Current).AppBaseDirectory, INI_FILE_NAME);
                data.WriteToFile(iniPath, Encoding.UTF8);
                Glog.Message(MyTraceCategory.Config, $"Done writing config file to path={iniPath}");
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "This is awkward, an error has occured trying to save the app config ini...", ex);
            }
        }

        public bool LoadFromIni() {
            try {
                string iniPath = Path.Combine(((App)Application.Current).AppBaseDirectory, INI_FILE_NAME);

                if (File.Exists(iniPath) == false) {
                    this.Dnf2011ExePath = "";
                    this.Dnf2011SystemIniPath = "";
                    return false;
                }
                try {
                    RawIniDocument rawDoc = new RawIniDocument();
                    rawDoc.ParseFile(iniPath, Encoding.UTF8);
                    IniDocument doc = new IniDocument(rawDoc);
                    this.Dnf2011SystemIniPath = doc["Files"]["Dnf2011SystemIniPath"].GetSimpleValue();
                    this.Dnf2011ExePath = doc["Files"]["Dnf2011ExePath"].GetSimpleValue();
                    this.Dnf2011ExeCommandLineArgs = doc["Files"]["Dnf2011ExeCommandLineArgs"].GetSimpleValue();
                    
                    string enableCloudSync = doc["Features"]["EnableSystemIniSteamCloudSync"].GetSimpleValue().ToLowerInvariant();
                    this.EnableSystemIniSteamCloudSync = enableCloudSync == "true" || enableCloudSync == "1";
                    
                    string openLogWindow = doc["Features"]["OpenLogWindowOnStartup"].GetSimpleValue().ToLowerInvariant();
                    this.OpenLogWindowOnStartup = openLogWindow == "true" || openLogWindow == "1";
                    
                    string theme = doc["Theme"]["ThemeName"].GetSimpleValue();
                    if (theme == MyThemes.DookieNookie2001.ToString()) {
                        this.Theme = theme;
                    } else {
                        this.Theme = MyThemes.Default.ToString();
                    }
                    
                    Glog.Message(MyTraceCategory.Config, $"Done reading config file from path={iniPath}");
                } catch (Exception) {
                    this.Dnf2011ExePath = "";
                    this.Dnf2011SystemIniPath = "";
                    return false;
                }
                return true;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "This is awkward, an error has occured trying to load the app config ini...", ex);
                return false;
            }
        }
    }
}
