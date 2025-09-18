using System;
using System.IO;

namespace TSqlFormatter.SSMS
{
    internal static class EarlyInitializer
    {
        static EarlyInitializer()
        {
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TSqlFormatter_ModuleInit.txt");
                var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MODULE INITIALIZED - TSqlFormatter.SSMS assembly loaded into process: {System.Diagnostics.Process.GetCurrentProcess().ProcessName} (PID: {System.Diagnostics.Process.GetCurrentProcess().Id})\r\n";
                File.AppendAllText(logPath, message);

                // Also try Windows Event Log
                try
                {
                    System.Diagnostics.EventLog.WriteEntry("Application",
                        $"TSqlFormatter.SSMS MODULE INITIALIZED in {System.Diagnostics.Process.GetCurrentProcess().ProcessName}",
                        System.Diagnostics.EventLogEntryType.Information);
                }
                catch { }
            }
            catch (Exception ex)
            {
                try
                {
                    var errorPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TSqlFormatter_ERROR.txt");
                    File.AppendAllText(errorPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MODULE INIT ERROR: {ex}\r\n");
                }
                catch { }
            }
        }

        public static void ForceInit()
        {
            // This method exists just to force the static constructor to run
        }
    }
}