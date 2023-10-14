using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace DnfServerSwitcher.Models.Trace {
    public class MyTraceListenerFileLogger : MyTraceListenerBase {
        public bool FlushAfterEachMessage { get; set; } = false;
        public MyTraceLevel MaxTraceLevel { get; set; } = MyTraceLevel.Error;
        public bool PeriodicAutoFlush { get; }
        public string LogFolder { get; }

        private FileStream? _logStream = null;
        private StreamWriter? _streamWriter = null;

        public MyTraceListenerFileLogger(string logFolder, string name, bool autoFlush = false) : base(name) {
            this.PeriodicAutoFlush = autoFlush;
            this.LogFolder = logFolder;
            
            if (autoFlush) {
                Thread logFlusherThread = new Thread(this.LogFlushLoop) {
                    Name = "MyTraceListenerLogger - Log Flusher Thread", 
                };
                logFlusherThread.Start();
            }
        }

        private void Initialize() {
            string fileName = this.Name + "_" + DateTime.Now.ToString(MyTrace.FileTimestamp) + ".log";
            string path = Path.Combine(this.LogFolder, fileName);
            FileInfo finfo = new FileInfo(path);
            this._logStream = new FileStream(finfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
            this._streamWriter = new StreamWriter(this._logStream);
        }

        private void LogFlushLoop() {
            while (true) {
                if (this.IsClosed) return;
                
                this._streamWriter?.Flush();
                this._logStream?.Flush();

                Thread.Sleep(5000);
            }
        }

        public override void Close() {
            base.Close();

            this._streamWriter?.Close();
            this._logStream?.Close();
        }

        public override void Flush() {
            if (this.IsClosed) { return; }

            this._streamWriter?.Flush();
            this._logStream?.Flush();
        }

        public override void WriteMessage(DateTime timestamp, string category, string message, MyTraceLevel level) {
            if (level <= this.MaxTraceLevel) {
                if (this._streamWriter == null) {
                    this.Initialize();
                }
                
                if (this.ShowLevels) {
                    this._streamWriter?.WriteLine(MyTrace.FormatLine(timestamp, category, message, level));
                } else {
                    this._streamWriter?.WriteLine(MyTrace.FormatLine(timestamp, category, message));
                }
                if (this.FlushAfterEachMessage) {
                    this.Flush();
                }
            }
        }

        public override void WriteMessage(DateTime timestamp, string category, List<string> messages, MyTraceLevel level) {
            if (level <= this.MaxTraceLevel) {
                if (this._streamWriter == null) {
                    this.Initialize();
                }
                
                foreach (string message in messages) {
                    if (this.ShowLevels) {
                        this._streamWriter?.WriteLine(MyTrace.FormatLine(timestamp, category, message, level));
                    } else {
                        this._streamWriter?.WriteLine(MyTrace.FormatLine(timestamp, category, message));
                    }
                }
                
                if (this.FlushAfterEachMessage) {
                    this.Flush();
                }
            }
        }

    }
}
