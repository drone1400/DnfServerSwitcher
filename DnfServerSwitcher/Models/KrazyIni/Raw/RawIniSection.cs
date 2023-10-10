namespace DnfServerSwitcher.Models.KrazyIni.Raw;

public class RawIniSection : RawIniLine {
    public string Section { get; protected set; } = "";

    public RawIniSection(string raw, string section) : base(raw) {
        this.Section = section;
    }
}
