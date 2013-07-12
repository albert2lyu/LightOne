using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    public class DataUploadingService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DatabaseDumpService _DatabaseDumpService;
        private readonly CompressionService _CompressionService;

        public DataUploadingService(string mongodumpFilename, string zFilename) {
            _DatabaseDumpService = new DatabaseDumpService(mongodumpFilename);
            _CompressionService = new CompressionService(zFilename);
        }

        public void Run(string database) {
            var dumpFolder = Path.Combine(Environment.CurrentDirectory, "upload");
            _DatabaseDumpService.DumpDatabase(database, dumpFolder, TimeSpan.FromHours(1));

            var compressedFilename = Path.Combine(dumpFolder, string.Format("{0}-dump.7z", database));
            _CompressionService.Compress(dumpFolder, compressedFilename);
        }

        
    }
}
