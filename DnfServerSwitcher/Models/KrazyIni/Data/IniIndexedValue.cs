using System.Collections.Generic;
namespace DnfServerSwitcher.Models.KrazyIni.Data {
    public class IniIndexedValue : IniValueBase {
        public Dictionary<int, string> Values { get; private set; } = new Dictionary<int, string>();

        public IniIndexedValue() {
            this.Kind = IniValueKind.Indexed;
        }

        public IniIndexedValue(Dictionary<int, string> dictionary) {
            this.Kind = IniValueKind.Indexed;
            this.Values = dictionary;
        }

        public IniIndexedValue(string[] array) {
            this.Kind = IniValueKind.Indexed;
            for (int i = 0; i < array.Length; i++) {
                this.Values.Add(i,array[i]);
            }
        }
        
        public IniIndexedValue(List<string> list) {
            this.Kind = IniValueKind.Indexed;
            for (int i = 0; i < list.Count; i++) {
                this.Values.Add(i,list[i]);
            }
        }
    }
}
