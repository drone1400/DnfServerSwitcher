namespace DnfServerSwitcher.Models.KrazyIni.Raw;

public class RawIniComment : RawIniLine {
    public string Comment { get; protected set; } = "";

    public RawIniComment(string raw, string comment) : base(raw) {
        this.Comment = comment;
    }
}
