using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SimpleSettingsManager
{
    public class Utilities
    {
        public static bool IsFileSQLiteDB(string settingsPath)
        {
            byte[] bytes = new byte[17];
            using (FileStream fs = new FileStream(settingsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Read(bytes, 0, 16);
            }
            string chkStr = ASCIIEncoding.ASCII.GetString(bytes);
            return chkStr.Contains("SQLite format");
        }

        public static bool IsFileXML(string settingsPath)
        {
            XElement xml;

            try
            {
                xml = XElement.Load(settingsPath);
                return true;
            }
            catch
            {
                return false;
            }
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
    }
}
