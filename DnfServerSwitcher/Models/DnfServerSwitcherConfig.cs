using System;
using System.IO;
using System.Text;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.KrazyIni.Data;
using DnfServerSwitcher.Models.KrazyIni.Raw;
using DnfServerSwitcher.Models.Trace;

namespace DnfServerSwitcher.Models {
    public class DnfServerSwitcherConfig {
        public const string INI_FILE_NAME = "DnfServerSwitcher.ini";
        public string Dnf2011ExePath { get; set; } = "";
        public string Dnf2011SystemIniPath { get; set; } = "";

        public void SaveToIni() {
            try {
                IniDocument data = new IniDocument();
                data["Files"] = new IniSection("Files");
                data["Files"]["Dnf2011ExePath"] = new IniKey("Dnf2011ExePath", this.Dnf2011ExePath);
                data["Files"]["Dnf2011SystemIniPath"] = new IniKey("Dnf2011SystemIniPath", this.Dnf2011SystemIniPath);

                string iniPath = Path.Combine(LocationHelper.AppBaseDirectory, INI_FILE_NAME);
                data.WriteToFile(iniPath, Encoding.UTF8);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "This is awkward, an error has occured trying to save the app config ini...", ex);
            }
        }

        public bool LoadFromIni() {
            try {
                string iniPath = Path.Combine(LocationHelper.AppBaseDirectory, INI_FILE_NAME);

                if (File.Exists(iniPath) == false) {
                    this.Dnf2011ExePath = "";
                    this.Dnf2011SystemIniPath = "";
                    return false;
                }
                try {
                    RawIniDocument rawDoc = new RawIniDocument();
                    rawDoc.ParseFile(iniPath, Encoding.UTF8);
                    IniDocument doc = new IniDocument(rawDoc);
                    this.Dnf2011ExePath = doc["Files"]["Dnf2011ExePath"].GetSimpleValue();
                    this.Dnf2011SystemIniPath = doc["Files"]["Dnf2011SystemIniPath"].GetSimpleValue();
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
