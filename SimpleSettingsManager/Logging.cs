using System;
using System.IO;

namespace SimpleSettingsManager
{
    internal class Logging
    {
        private const string _logPath = "SSM.log";

        public static void Log(string log, Severity severity = Severity.INFO)
        {
            if (SSM.GetVerboseLogging())
            {
                string formattedLog;
                switch (severity)
                {
                    case Severity.DEBUG:
                        {
                            formattedLog = String.Format("[DEBUG] {0}", log);
                            break;
                        }
                    case Severity.WARN:
                        {
                            formattedLog = String.Format("[WARN] {0}", log);
                            break;
                        }
                    case Severity.ERROR:
                        {
                            formattedLog = String.Format("[ERROR] {0}", log);
                            break;
                        }
                    default:
                        {
                            formattedLog = String.Format("[INFO] {0}", log);
                            break;
                        }
                }
                Console.WriteLine(formattedLog);
                if (SSM.GetFileLogging()) LogToFile(formattedLog);
            }
            else if (severity > 0)
            {
                Console.WriteLine(log);
                if (SSM.GetFileLogging()) LogToFile(log);
            }
        }

        private static void LogToFile(string log)
        {
            File.AppendAllText(_logPath, log + Environment.NewLine);
        }
    }

    internal enum Severity
    {
        DEBUG,
        INFO,
        WARN,
        ERROR
    };
}
