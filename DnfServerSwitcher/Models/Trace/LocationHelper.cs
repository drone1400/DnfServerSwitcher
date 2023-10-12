using System;
using System.Diagnostics;
using System.IO;
namespace DnfServerSwitcher.Models.Trace {
    public class LocationHelper {
        public static string AppBaseDirectory {
            get;
        }

        static LocationHelper() {
            // initialize base directory somehow...
            //_AppBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //_AppBaseDirectory = AppContext.BaseDirectory;
            
            string? processFile = Process.GetCurrentProcess().MainModule?.FileName;
            
            if (processFile == null) {
                NullReferenceException ex = new NullReferenceException("Could not determine current process start location...");
                Glog.Error(MyTraceCategory.General, ex);
                throw ex;
            }
            
            FileInfo finfo = new FileInfo(processFile);

            if (finfo.DirectoryName == null) {
                NullReferenceException ex = new NullReferenceException("Could not determine current process start location...");
                Glog.Error(MyTraceCategory.General, ex);
                throw ex;
            }
            AppBaseDirectory = finfo.DirectoryName;
        }
        
        public static string LogsDirectory {
            get {
                string path = Path.Combine(AppBaseDirectory, "logs");
                if (Directory.Exists(path) == false) {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
    }
}
