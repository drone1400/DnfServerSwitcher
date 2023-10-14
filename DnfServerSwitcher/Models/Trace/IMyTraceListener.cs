using System;
using System.Collections.Generic;
namespace DnfServerSwitcher.Models.Trace {
    public interface IMyTraceListener {
        string Name { get; }
        MyTraceLevel Levels { get; }
        bool IsClosed { get; }
        void WriteMessage(DateTime timestamp, string category, string message, MyTraceLevel level);

        void WriteMessage(DateTime timestamp, string category, List<string> messages, MyTraceLevel level);
        void Close();
        void Flush();
    }
}
