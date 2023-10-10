namespace DnfServerSwitcher.Models.KrazyIni.Raw;

public class RawIniKeyIndexValue : RawIniLine {

    public string Key { get; protected set; } = "";
    public int Index { get; protected set; } = 0;
    public string Value { get; protected set; } = "";

    public RawIniKeyIndexValue(string raw, string key, int index, string value) : base(raw){
        this.Key = key;
        this.Index = index;
        this.Value = value;
    }
}
