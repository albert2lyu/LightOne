using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class DatabaseDump {
        public void DumpDatabase(string database, string outputFolder, TimeSpan timeAgo) {
            var filename = Path.Combine(Environment.CurrentDirectory, "Lib", "mongodump.exe");

            foreach (var arg in BuildMongodumpArguments(timeAgo)) {
                var command = string.Format("-db {0} -o {1} {2}", database, outputFolder, arg);

                var pr = new ProcessRunner();
                pr.Run(filename, command);
            }
        }

        private IEnumerable<string> BuildMongodumpArguments(TimeSpan timeAgo) {
            var epoch = new DateTime(1970, 1, 1).ToLocalTime();
            var startTime = (long)DateTime.Now.Add(timeAgo.Negate()).Subtract(epoch).TotalMilliseconds;
            yield return string.Format("-c products -q \"{{UpdateTime: {{$gt: new Date({0})}}}}\"", startTime);

            yield return "-c categories";

            yield return "-c ratio_rankings";
        }
    }
}
