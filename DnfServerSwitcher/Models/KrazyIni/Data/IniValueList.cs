using System.Collections.Generic;
namespace DnfServerSwitcher.Models.KrazyIni.Data {
    public class IniValueList : IniValueBase {
        public List<string> Values { get; private set; } = new List<string>();

        public IniValueList() {
            this.Kind = IniValueKind.List;
        }

        public IniValueList(List<string> list) {
            this.Kind = IniValueKind.List;
            this.Values = list;
        }
    }
}
