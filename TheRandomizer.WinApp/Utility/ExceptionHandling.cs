using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace TheRandomizer.WinApp.Utility
{
    static class ExceptionHandling 
    {

        public static bool EnableDebugLogging { get; set; } = false;
        public static bool EnableInfoLogging { get; set; } = false;
        public static bool EnableErrorLogging { get; set; } = false;
        public static bool EnableWarningLogging { get; set; } = false;

        public static Logger Logger { get; set; } = LogManager.GetLogger("TheRandomizer");

        public static void LogException(Exception ex)
        {
            if (EnableErrorLogging) Logger.Log(LogLevel.Error, ex);
        }

        public static void LogInfo(string message)
        {
            if (EnableInfoLogging) Logger.Log(LogLevel.Info, message);
        }

        public static void LogDebug(string message)
        {
            if (EnableDebugLogging) Logger.Log(LogLevel.Debug, message);
        }
    }
}
