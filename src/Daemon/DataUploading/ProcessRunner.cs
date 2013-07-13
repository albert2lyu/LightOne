using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class ProcessRunner {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(string filename, string command) {
            var info = new ProcessStartInfo(filename, command);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            var process = Process.Start(info);

            WriteLog(process.StandardOutput, Logger.Debug);
            WriteLog(process.StandardError, Logger.Error);
        }

        private void WriteLog(StreamReader reader, Action<string> logAction) {
            while (!reader.EndOfStream) {
                logAction.Invoke(reader.ReadLine());
            }
        }
    }
}
