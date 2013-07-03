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
    [DisallowConcurrentExecution]
    class RatioRankingJob : IJob {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context) {
            var sw = new Stopwatch();
            sw.Start();
            Logger.Info("RatioRankingJob启动");

            var service = new RatioRankingService();
            service.RankAllCategories();

            Logger.InfoFormat("RatioRankingJob完成，用时{0}", sw.Elapsed);
        }
    }
}
