using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PhoenixLoader
{
    class ErrorWriter
    {
        public static void writeError(String error)
        {
            string downloadLocation = System.AppDomain.CurrentDomain.FriendlyName;
            downloadLocation = downloadLocation.Replace("vshost.", "");
            String path = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(downloadLocation, "errors.log");
            File.AppendAllText(path, error + "\n");
        }
    }
}
