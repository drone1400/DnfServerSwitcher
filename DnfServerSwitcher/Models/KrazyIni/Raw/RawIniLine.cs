namespace DnfServerSwitcher.Models.KrazyIni.Raw;

public abstract class RawIniLine {
    public string RawLine { get; protected set; } = "";

    public RawIniLine() {
    }
    public RawIniLine(string raw) {
        this.RawLine = raw;
    }
}
