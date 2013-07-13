using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    class DatabaseExport {
        public void ExportDatabase(string database, string outputFolder, TimeSpan timeAgo) {
            var filename = Path.Combine(Environment.CurrentDirectory, "Lib", "mongoexport.exe");

            foreach (var arg in BuildMongodumpArguments(timeAgo)) {
                var outputFile = Path.Combine(outputFolder, arg.Item1 + ".json");
                var command = string.Format("-db {0} -c {1} -o \"{2}\"", database, arg.Item1, outputFile);
                if (!string.IsNullOrWhiteSpace(arg.Item2))
                    command += string.Format(" -q {0}", arg.Item2);

                var pr = new ProcessRunner();
                pr.Run(filename, command);
            }
        }

        private IEnumerable<Tuple<string, string>> BuildMongodumpArguments(TimeSpan timeAgo) {
            var epoch = new DateTime(1970, 1, 1).ToLocalTime();
            var startTime = (long)DateTime.Now.Add(timeAgo.Negate()).Subtract(epoch).TotalMilliseconds;
            yield return new Tuple<string, string>("products", string.Format("\"{{UpdateTime: {{$gt: new Date({0})}}}}\"", startTime));

            yield return new Tuple<string, string>("categories", null);

            yield return new Tuple<string, string>("ratio_rankings", null);
        }
    }
}
