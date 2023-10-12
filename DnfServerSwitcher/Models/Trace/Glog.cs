using System;
using System.Collections.Generic;
namespace DnfServerSwitcher.Models.Trace {
    public static class Glog {
         
        public static void Message(MyTraceCategory category, string message, MyTraceLevel level = MyTraceLevel.Information) {
            MyTrace.Global.WriteMessage(category, message, level);
        }

        public static void Message(string category, string message, MyTraceLevel level = MyTraceLevel.Information) {
            MyTrace.Global.WriteMessage(category, message, level);
        }

        public static void Message(MyTraceCategory category, List<string> messages, MyTraceLevel level = MyTraceLevel.Information) {
            MyTrace.Global.WriteMessage(category, messages, level);
        }

        public static void Message(string category, List<string> messages, MyTraceLevel level = MyTraceLevel.Information) {
            MyTrace.Global.WriteMessage(category, messages, level);
        }

        public static void Error(MyTraceCategory category, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            MyTrace.Global.WriteException(category, ex, level);
        }

        public static void Error(string category, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            MyTrace.Global.WriteException(category, ex, level);
        }
        
        public static void Error(MyTraceCategory category, string message, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            MyTrace.Global.WriteException(category, message, ex, level);
        }

        public static void Error(string category, string message, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            MyTrace.Global.WriteException(category, message, ex, level);
        }
    }
}
