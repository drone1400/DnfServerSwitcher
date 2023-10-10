namespace DnfServerSwitcher.Models.KrazyIni.Raw;

public class RawIniKeyValue : RawIniLine {

    public string Key { get; protected set; } = "";
    public string Value { get; protected set; } = "";

    public RawIniKeyValue(string raw, string key, string value) : base(raw){
        this.Key = key;
        this.Value = value;
    }
}
