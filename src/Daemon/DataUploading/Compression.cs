using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class Compression {
        public void Compress(string inputPath, string outputFilename) {
            var filename = Path.Combine(Environment.CurrentDirectory, "Lib", "7z.exe");
            var command = string.Format("a {0} {1}\\*", outputFilename, inputPath);

            var pr = new ProcessRunner();
            pr.Run(filename, command);
        }
    }
}
