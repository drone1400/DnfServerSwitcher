using System;
namespace DnfServerSwitcher.Models {
    public static class ExceptionHelper {
        public static string GetExceptionAsString(Exception ex) {
            string str = "";

            str += "Exception: " + ex.ToString() + Environment.NewLine;

            while (ex.InnerException != null) {
                ex = ex.InnerException;

                str += "Inner Exception: " + ex.ToString() + Environment.NewLine;
            }

            return str;
        }
    }
}
