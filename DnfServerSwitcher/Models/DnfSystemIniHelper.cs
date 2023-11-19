using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.Trace;
namespace DnfServerSwitcher.Models {
    public static class DnfSystemIniHelper {
        public static string GetSha256(string filePath) {
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

        public static void TryCopyDeprecatedFiles(string dnfExePath, bool verifyChecksumIfExist) {
            try {
                string fileNameMod = "dnWindow.u";
                string filePathMod = Path.Combine((Application.Current as App)!.AppBaseDirectory, "ThirdParty", fileNameMod);
                // check mod file exists
                if (!File.Exists(filePathMod)) {
                    Glog.Message(MyTraceCategory.General, $"Local copy of dnWindow.u not found in ThirdParty folder at path={filePathMod}");
                    return;
                }
                FileInfo finfo = new FileInfo(dnfExePath);

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


        public static bool SetDeprecatedMode(string systemIni) {
            IniDocument? data = DnfIniParseHelper.ParseDnf2011Ini(systemIni);
            if (data == null) {
                MessageBox.Show("An error has occurred parsing the System.ini file!...",  "ERROR parsing System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Glog.Message(MyTraceCategory.General, $"Successfully parsed system.ini at path={systemIni}");

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

            if (!DnfIniParseHelper.WriteDnf2011Ini(systemIni, data)) {
                MessageBox.Show("An error has occurred saving the System.ini file!...",  "ERROR saving System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Glog.Message(MyTraceCategory.General, $"Successfully wrote updated system.ini at path={systemIni}");
            return true;
        }

        public static bool SetNormalMode(string systemIni) {
            IniDocument? data = DnfIniParseHelper.ParseDnf2011Ini(systemIni);
            if (data == null) {
                MessageBox.Show("An error has occurred parsing the System.ini file!...",  "ERROR parsing System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Glog.Message(MyTraceCategory.General, $"Successfully parsed system.ini at path={systemIni}");

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

            if (!DnfIniParseHelper.WriteDnf2011Ini(systemIni, data)) {
                MessageBox.Show("An error has occurred saving the System.ini file!...",  "ERROR saving System.ini!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            Glog.Message(MyTraceCategory.General, $"Successfully wrote updated system.ini at path={systemIni}");
            return true;
        }
    }
}
