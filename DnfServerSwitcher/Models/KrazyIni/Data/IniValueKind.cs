namespace DnfServerSwitcher.Models.KrazyIni.Data {
    public enum IniValueKind {
        Empty,              // no value stored
        Simple,             // a simple string value
        List,               // a list of string values
        Indexed,            // a number of string values, each with a unique integer index
        IndexedList,        // a number of lists of string values, each list with a unique integer index
    }
}
