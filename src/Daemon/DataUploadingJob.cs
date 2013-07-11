using Business;
using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon {
    /// <summary>
    /// 数据上传作业
    /// </summary>
    [DisallowConcurrentExecution]
    class DataUploadingJob : IJob {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context) {
            var sw = new Stopwatch();
            sw.Start();
            Logger.Info("数据上传作业启动");

            var service = new DataUploadingService();
            service.Run();

            Logger.InfoFormat("数据上传作业完成，用时{0}", sw.Elapsed);
        }
    }
}
