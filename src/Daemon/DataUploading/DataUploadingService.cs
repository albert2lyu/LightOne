using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon.DataUploading {
    public class DataUploadingService {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly DatabaseExport _DatabaseExport;
        private readonly Compression _CompressionService;
        private readonly FileTransfer _FileTransferService;

        public DataUploadingService() {
            _DatabaseExport = new DatabaseExport();
            _CompressionService = new Compression();
            _FileTransferService = new FileTransfer();
        }

        public void Run(string database) {
            var tmpFolder = Path.Combine(Environment.CurrentDirectory, "tmp");

            var exportFolder = Path.Combine(tmpFolder, database);
            if (!Directory.Exists(exportFolder))
                Directory.CreateDirectory(exportFolder);
            _DatabaseExport.ExportDatabase(database, exportFolder, TimeSpan.FromMinutes(35));

            var compressedFilename = Path.Combine(tmpFolder, string.Format("{0}-dump-{1:MMddHHmm}.7z", database, DateTime.Now));
            _CompressionService.Compress(exportFolder, compressedFilename);

            var remotePath = ConfigurationManager.AppSettings["uploadRemotePath"];
            var password = ConfigurationManager.AppSettings["uploadPassword"];
            _FileTransferService.Transfer(password, compressedFilename, remotePath);
        }
    }
}
