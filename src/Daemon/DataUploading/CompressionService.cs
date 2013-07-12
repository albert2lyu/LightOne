using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class CompressionService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _7zFilename;

        public CompressionService(string zFilename) {
            if (string.IsNullOrWhiteSpace(zFilename))
                throw new ArgumentException("7z.exe路径不能为空");
            _7zFilename = zFilename;
        }

        public void Compress(string inputPath, string outputFilename) {
            var command = string.Format("a {0} {1}", outputFilename, inputPath);
            var info = new ProcessStartInfo(_7zFilename, command);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;

            var process = Process.Start(info);

            var output = process.StandardOutput.ReadToEnd();
            Logger.Debug(output);
        }
    }
}
