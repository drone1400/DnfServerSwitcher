using System.Collections.Generic;
namespace DnfServerSwitcher.Models.KrazyIni.Data {
    public class IniIndexedValueList : IniValueBase {
        public Dictionary<int, List<string>> Values { get; private set; } = new Dictionary<int, List<string>>();

        public IniIndexedValueList() {
            this.Kind = IniValueKind.IndexedList;
        }
        
        public IniIndexedValueList(Dictionary<int, List<string>> dictionary) {
            this.Kind = IniValueKind.IndexedList;
            this.Values = dictionary;
        }
        
        public IniIndexedValueList(List<string>[] arrayOfLists) {
            this.Kind = IniValueKind.IndexedList;
            for (int i = 0; i < arrayOfLists.Length; i++) {
                this.Values.Add(i, arrayOfLists[i]);
            }
        }
        
        public IniIndexedValueList(List<List<string>> listOfLists) {
            this.Kind = IniValueKind.IndexedList;
            for (int i = 0; i < listOfLists.Count; i++) {
                this.Values.Add(i, listOfLists[i]);
            }
        }
    }
}
