using System;
using System.IO;
using DnfServerSwitcher.Models.Trace;
using Steamworks;
namespace DnfServerSwitcher.Models.SteamApi {
    public class SteamApiHelper {

        //private Dictionary<string,SteamRemoteFileInfo> _remoteFiles = new Dictionary<string, SteamRemoteFileInfo>();

        private bool _isClientInitialized = false;

        private void CheckInitializeClient() {
            if (this._isClientInitialized) return;
            SteamClient.Init(App.DNF2011_STEAMAPPID);
            this._isClientInitialized = true;
            Glog.Message(MyTraceCategory.Steamworks, $"Initialized steamworks client for AppId={App.DNF2011_STEAMAPPID}");
        }

        public bool ReadAllFiles() {
            try {
                this.CheckInitializeClient();

                foreach ( var file in SteamRemoteStorage.Files ) {
                    int fileSize = SteamRemoteStorage.FileSize(file);
                    DateTime fileTime = SteamRemoteStorage.FileTime(file);

                    SteamRemoteFileInfo info = new SteamRemoteFileInfo() {
                        FileName = file,
                        Size = fileSize,
                        Time = fileTime,
                    };

                    //this._remoteFiles.Add(file, info);

                    Glog.Message(MyTraceCategory.Steamworks, $"Found Steam Remote file: name=\"{file}\", size={fileSize}, time={fileTime.ToString("O")}");
                }
                return true;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Steamworks, ex);
                return false;
            }
        }

        public string GetUserId32() {
            try {
                this.CheckInitializeClient();

                SteamId id = SteamClient.SteamId;
                return id.AccountId.ToString();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Steamworks, ex);
                return "";
            }
        }

        public string GetUserId64() {
            try {
                this.CheckInitializeClient();

                SteamId id = SteamClient.SteamId;
                return id.ToString();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Steamworks, ex);
                return "";
            }
        }

        public string GetDnfInstallPath() {
            try {
                this.CheckInitializeClient();

                return SteamApps.AppInstallDir(App.DNF2011_STEAMAPPID);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Steamworks, ex);
                return "";
            }
        }

        public bool WriteSystemIni(string localFilePath) {
            try {
                this.CheckInitializeClient();

                byte[] systemIniBytes = File.ReadAllBytes(localFilePath);
                Glog.Message(MyTraceCategory.Steamworks, $"Read {systemIniBytes.Length} bytes from local system.ini file at path={localFilePath}");

                SteamRemoteStorage.FileWrite("system.ini", systemIniBytes);
                Glog.Message(MyTraceCategory.Steamworks, $"Wrote system.ini file to Steam Remote Storage!");

                return true;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Steamworks, ex);
                return false;
            }
        }

        public void Shutdown() {
            try {
                this._isClientInitialized = false;
                SteamClient.Shutdown();
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Steamworks, ex);
            }
        }
    }
}
