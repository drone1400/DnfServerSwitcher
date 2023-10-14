using System;
using System.Collections.Generic;
using System.Text;
namespace DnfServerSwitcher.Models.Trace {

    public class MyTrace {
        public static MyTrace Global = new MyTrace("Global");

        public static readonly string TimestampFormat = "HH:mm:ss.ffffff: ";
        public static readonly string FileTimestamp = "yyyy-MM-dd_HH-mm-ss";

        private List<IMyTraceListener> _listeners = new List<IMyTraceListener>();
        private readonly object _syncRoot = new object();

        public string Name { get; private set; }

        public MyTrace(string name) {
            this.Name = name;
        }

        public void AddListener(IMyTraceListener listener) {
            lock (this._syncRoot) {
                if (this._listeners.Contains(listener) == false) {
                    this._listeners.Add(listener);
                }
            }
        }

        public void RemoveListener(IMyTraceListener listener) {
            lock (this._syncRoot) {
                if (this._listeners.Contains(listener)) {
                    this._listeners.Remove(listener);
                }
            }
        }

        public void Close() {
            lock (this._syncRoot) {
                foreach (IMyTraceListener listener in this._listeners) {
                    listener.Close();
                }
                this._listeners.Clear();
            }
        }

        public void Flush() {
            lock (this._syncRoot) {
                foreach (IMyTraceListener listener in this._listeners) {
                    listener.Flush();
                }
            }
        }

        public void WriteMessage(MyTraceCategory category, string message, MyTraceLevel level = MyTraceLevel.Information) {
            DateTime now = DateTime.Now;
            lock (this._syncRoot) {
                foreach (IMyTraceListener listener in this._listeners) {
                    if (!listener.IsClosed && listener.Levels.HasFlag(level)) {
                        listener.WriteMessage(now, category.ToString(), message, level);
                    }
                }
            }
        }

        public void WriteMessage(string category, string message, MyTraceLevel level = MyTraceLevel.Information) {
            DateTime now = DateTime.Now;
            lock (this._syncRoot) {
                foreach (IMyTraceListener listener in this._listeners) {
                    if (!listener.IsClosed && listener.Levels.HasFlag(level)) {
                        listener.WriteMessage(now, category, message, level);
                    }
                }
            }
        }

        public void WriteMessage(MyTraceCategory category, List<string> messages, MyTraceLevel level = MyTraceLevel.Information) {
            DateTime now = DateTime.Now;
            lock (this._syncRoot) {
                foreach (IMyTraceListener listener in this._listeners) {
                    if (!listener.IsClosed && listener.Levels.HasFlag(level)) {
                        listener.WriteMessage(now, category.ToString(), messages, level);
                    }
                }
            }
        }

        public void WriteMessage(string category, List<string> messages, MyTraceLevel level = MyTraceLevel.Information) {
            DateTime now = DateTime.Now;
            lock (this._syncRoot) {
                foreach (IMyTraceListener listener in this._listeners) {
                    if (!listener.IsClosed && listener.Levels.HasFlag(level)) {
                        listener.WriteMessage(now, category, messages, level);
                    }
                }
            }
        }

        public void WriteException(MyTraceCategory category, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            this.WriteMessage(category, GetExceptionStringAsList(ex), level);
        }
        
        public void WriteException(MyTraceCategory category, string message, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            this.WriteMessage(category, message, level);
            this.WriteMessage(category, GetExceptionStringAsList(ex), level);
        }

        public void WriteException(string category, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            this.WriteMessage(category, GetExceptionStringAsList(ex), level);
        }
        
        public void WriteException(string category, string message, Exception ex, MyTraceLevel level = MyTraceLevel.Error) {
            this.WriteMessage(category, message, level);
            this.WriteMessage(category, GetExceptionStringAsList(ex), level);
        }

        public static string GetExceptionMessages(Exception? ex) {
            StringBuilder str = new StringBuilder();

            while (ex != null) {
                str.AppendLine(ex.ToString());
                ex = ex.InnerException;
            }

            return str.ToString();
        }

        public static List<string> GetExceptionStringAsList(Exception? ex) {
            List<string> messages = new List<string>();

            while (ex != null) {
                messages.Add(ex.ToString());
                ex = ex.InnerException;
            }

            return messages;
        }

        public static string FormatLine(DateTime timestamp, string category, string message) {
            return $"{timestamp.ToString(MyTrace.TimestampFormat)} [{category}] {message}";
        }

        public static string FormatLine(DateTime timestamp, string category, string message, MyTraceLevel level) {
            return $"{timestamp.ToString(MyTrace.TimestampFormat)} [{category}] [{level}] {message}";
        }
    }
}
