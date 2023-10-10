namespace DnfServerSwitcher.Models.KrazyIni.Data {
    public class IniValue : IniValueBase {
        public string Value { get; set; } = "";

        public IniValue() {
            this.Kind = IniValueKind.Simple;
        }
        
        public IniValue(string value) {
            this.Value = value;
            this.Kind = IniValueKind.Simple;
        }
    }
}
