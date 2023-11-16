using System;
namespace DnfServerSwitcher.Models.SteamApi {
    public record SteamRemoteFileInfo {
        public string FileName { get; set; } = "";
        public int Size { get; set; }
        public DateTime Time { get; set; }
    }
}
