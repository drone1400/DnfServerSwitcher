using System;
namespace DnfServerSwitcher.Models.Trace {
    [Flags]
    public enum MyTraceLevel {
        Critical = 1,
        Error = 2,
        Warning = 4,
        Information = 8,
        Verbose = 16,
    }
}
