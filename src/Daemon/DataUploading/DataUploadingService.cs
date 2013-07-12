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

        public DataUploadingService() {
            _DatabaseDumpService = new DatabaseDumpService();
            _CompressionService = new CompressionService();
        }

        public void Run(string database) {
            var tmpFolder = Path.Combine(Environment.CurrentDirectory, "tmp");
            _DatabaseDumpService.DumpDatabase(database, tmpFolder, TimeSpan.FromHours(1));

            var compressedFilename = Path.Combine(tmpFolder, string.Format("{0}-dump.7z", database));
            _CompressionService.Compress(Path.Combine(tmpFolder, database), compressedFilename);
        }

        
    }
}
