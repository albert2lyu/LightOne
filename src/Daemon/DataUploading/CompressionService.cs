using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class CompressionService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Compress(string inputPath, string outputFilename) {
            var filename = Path.Combine(Environment.CurrentDirectory, "Lib", "7z.exe");
            var command = string.Format("a {0} {1}\\*", outputFilename, inputPath);
            var info = new ProcessStartInfo(filename, command);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            var process = Process.Start(info);

            var reader = process.StandardOutput;
            while (!reader.EndOfStream) {
                Logger.Debug(reader.ReadLine());
            }
        }
    }
}
