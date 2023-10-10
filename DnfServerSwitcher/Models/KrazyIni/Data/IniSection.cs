using System;
using System.Collections.Generic;
namespace DnfServerSwitcher.Models.KrazyIni.Data;

public class IniSection {

    public string Section { get; private set; } = "";
    public List<string> Comments { get; private set; } = new List<string>();

    public Dictionary<string, IniKey> KeyDictionary { get; private set; } = new Dictionary<string, IniKey>();

    public IniSection(string section) {
        this.Section = section;
    }

    public bool ContainsKey(string key) {
        return this.KeyDictionary.ContainsKey(key);
    }

    public IniKey this[string key] {
        get => this.KeyDictionary[key];
        set {
            this.KeyDictionary[key] = value;
        }
    }

    public string ToString(string lineTermination) {
        return $"[{this.Section}]{lineTermination}";
    }
    
    public override string ToString() {
        return this.ToString(Environment.NewLine);
    }
}
