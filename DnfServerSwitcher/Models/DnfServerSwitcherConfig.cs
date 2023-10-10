using System;
using System.IO;
using System.Text;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.KrazyIni.Data;
using DnfServerSwitcher.Models.KrazyIni.Raw;

namespace DnfServerSwitcher.Models {
    public class DnfServerSwitcherConfig {
        public const string INI_FILE_NAME = "DnfServerSwitcher.ini";
        public string Dnf2011ExePath { get; set; } = "";
        public string Dnf2011SystemIniPath { get; set; } = "";

        public void SaveToIni() {
            IniDocument data = new IniDocument();
            data["Files"] = new IniSection("Files");
            data["Files"]["Dnf2011ExePath"] = new IniKey("Dnf2011ExePath",this.Dnf2011ExePath);
            data["Files"]["Dnf2011SystemIniPath"] = new IniKey("Dnf2011SystemIniPath",this.Dnf2011SystemIniPath);

            data.WriteToFile(INI_FILE_NAME, Encoding.UTF8);
        }

        public bool LoadFromIni() {
            if (File.Exists(INI_FILE_NAME) == false) {
                this.Dnf2011ExePath = "";
                this.Dnf2011SystemIniPath = "";
                return false;
            }
            try {
                RawIniDocument rawDoc = new RawIniDocument();
                rawDoc.ParseFile(INI_FILE_NAME, Encoding.UTF8);
                IniDocument doc = new IniDocument(rawDoc);
                this.Dnf2011ExePath = doc["Files"]["Dnf2011ExePath"].GetSimpleValue();
                this.Dnf2011SystemIniPath = doc["Files"]["Dnf2011SystemIniPath"].GetSimpleValue();
            } catch (Exception) {
                this.Dnf2011ExePath = "";
                this.Dnf2011SystemIniPath = "";
                return false;
            }
            return true;
        }
    }
}
