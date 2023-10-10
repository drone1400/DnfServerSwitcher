using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DnfServerSwitcher.Models.KrazyIni.Data {
    public class IniKey {

        public List<string> Comments { get; private set; } = new List<string>();
        public string Key { get; private set; }
        public IniValueBase Value { get; private set; } = new IniValueEmpty();

        public IniKey(string key) {
            this.Key = key;
        }

        public IniKey(string key, string value) {
            this.Key = key;
            this.Value = new IniValue(value);
        }

        public bool IsEmpty() {
            return this.Value.Kind == IniValueKind.Empty;
        }

        public void SetEmptyValue() {
            this.Value = new IniValueEmpty();
        }
        
        // ------
        // Single string value
        public void SetSimpleValue(string value) {
            this.Value = new IniValue(value);
        }
        public string GetSimpleValue() {
            if (this.Value.Kind == IniValueKind.Simple) {
                return ((IniValue)this.Value).Value;
            }
            
            throw new InvalidOperationException($"Data stored in Key={this.Key} is of Kind={this.Value.Kind}! It can not be read with GetSimpleValue!");
        }

        // ------
        // List of string values
        public void SetListValue(List<string> value) {
            this.Value = new IniValueList(value);
        }
        public void SetListValue(string[] value) {
            this.Value = new IniValueList(value.ToList());
        }
        public List<string> AccessListValue() {
            if (this.Value.Kind == IniValueKind.List) {
                return ((IniValueList)this.Value).Values;
            }
            
            throw new InvalidOperationException($"Data stored in Key={this.Key} is of Kind={this.Value.Kind}! It can not be read with GetListValue!");
        }
        public string GetListValueFirst() {
            if (this.Value.Kind == IniValueKind.List) {
                return ((IniValueList)this.Value).Values.First();
            }
            
            throw new InvalidOperationException($"Data stored in Key={this.Key} is of Kind={this.Value.Kind}! It can not be read with GetListValueFirst!");
        }
        public string GetListValueLast() {
            if (this.Value.Kind == IniValueKind.List) {
                return ((IniValueList)this.Value).Values.Last();
            }
            
            throw new InvalidOperationException($"Data stored in Key={this.Key} is of Kind={this.Value.Kind}! It can not be read with GetListValueLast!");
        }

        // ------
        // Indexed values
        public void SetIndexedValue(string[] values) {
            this.Value = new IniIndexedValue(values);
        }
        public void SetIndexedValue(Dictionary<int,string> values) {
            this.Value = new IniIndexedValue(values);
        }
        public Dictionary<int,string> AccessIndexedValue() {
            if (this.Value.Kind == IniValueKind.Indexed) {
                return ((IniIndexedValue)this.Value).Values;
            }
            
            throw new InvalidOperationException($"Data stored in Key={this.Key} is of Kind={this.Value.Kind}! It can not be read with AccessIndexedValue!");
        }
        
        // ------
        // Indexed list of string values
        public void SetIndexedValueList(List<List<string>> values) {
            this.Value = new IniIndexedValueList(values);
        }
        public void SetIndexedValueList(List<string>[] values) {
            this.Value = new IniIndexedValueList(values);
        }
        public void SetIndexedValueList(Dictionary<int,List<string>> values) {
            this.Value = new IniIndexedValueList(values);
        }
        public Dictionary<int,List<string>> AccessIndexedValueList() {
            if (this.Value.Kind == IniValueKind.IndexedList) {
                return ((IniIndexedValueList)this.Value).Values;
            }
            throw new InvalidOperationException($"Data stored in Key={this.Key} is of Kind={this.Value.Kind}! It can not be read with AccessIndexedValueList!");
        }

        public override string ToString() {
            return this.ToString(Environment.NewLine);
        }
        
        public string ToString(string lineTermination) {
            switch (this.Value.Kind) {
                default:
                case IniValueKind.Empty:
                    {
                        return $"{this.Key}={lineTermination}";
                    }
                case IniValueKind.Simple:
                    {
                        return $"{this.Key}={this.GetSimpleValue()}{lineTermination}";
                    }
                case IniValueKind.List:
                    {
                        StringBuilder sb = new StringBuilder();
                        List<string> list = this.AccessListValue();
                        foreach (string s in list) {
                            sb.Append($"{this.Key}={s}{lineTermination}");
                        }
                        return sb.ToString();
                    }
                case IniValueKind.Indexed:
                    {
                        StringBuilder sb = new StringBuilder();
                        Dictionary<int,string> dictionary = this.AccessIndexedValue();
                        foreach (KeyValuePair<int,string> kvp in dictionary) {
                            sb.Append($"{this.Key}[{kvp.Key}]={kvp.Value}{lineTermination}");
                        }
                        return sb.ToString();
                    }
                case IniValueKind.IndexedList:
                    {
                        StringBuilder sb = new StringBuilder();
                        Dictionary<int,List<string>> dictionary = this.AccessIndexedValueList();
                        foreach (KeyValuePair<int,List<string>> kvp in dictionary) {
                            foreach (string s in kvp.Value) {
                                sb.Append($"{this.Key}[{kvp.Key}]={s}{lineTermination}");
                            }
                        }
                        return sb.ToString();
                    }
            }
        }
    }
}
