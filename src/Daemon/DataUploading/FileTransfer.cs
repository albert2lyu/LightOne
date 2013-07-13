using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class FileTransfer {
        public void Transfer(string password, string localPath, string remotePath) {
            var filename = Path.Combine(Environment.CurrentDirectory, "Lib", "pscp.exe");
            var command = string.Format("-pw {0} \"{1}\" {2}", password, localPath, remotePath);

            var pr = new ProcessRunner();
            pr.Run(filename, command);
        }
    }
}
