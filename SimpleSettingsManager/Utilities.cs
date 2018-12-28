using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleSettingsManager
{
    public class Utilities
    {
        public static string GetVersion()
        {
            string[] ver = (typeof(SimpleSettingsManager.SSM).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version).Split('.');
            return "v" + ver[0] + "." + ver[1] + "." + ver[2];
        }
    }

    internal class IntUtilities
    {
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }



        public static Int64 GetUnixTimestamp()
        {
            return (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static string SqlEscape(string usString)
        {
            if (usString == null)
                return null;

            return Regex.Replace(usString, @"[\r\n\x00\x1a\\'""]", @"\$0");
        }

        public static bool isSQLiteDB(string path)
        {
            byte[] bytes = new byte[17];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Read(bytes, 0, 16);
            }
            string chkStr = ASCIIEncoding.ASCII.GetString(bytes);
            return chkStr.Contains("SQLite format");
        }
    }
}
