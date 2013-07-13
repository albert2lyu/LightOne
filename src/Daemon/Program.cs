using Common.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daemon {
    class Program {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args) {
            Logger.Info("Daemon启动");

            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();
            
            ScheduleRatioRankingJob(scheduler);
            ScheduleDataUploadingJob(scheduler);
        }

        private static void ScheduleRatioRankingJob(IScheduler scheduler) {
            const int INTERVAL_IN_MINUTES = 15;
            scheduler.ScheduleJob(
                JobBuilder.Create<RatioRankingJob>().WithIdentity("RatioRankingJob").Build(),
                TriggerBuilder.Create().WithIdentity("RatioRankingTrigger").StartNow().WithSimpleSchedule(x => x.RepeatForever().WithIntervalInMinutes(INTERVAL_IN_MINUTES)).Build());
        }

        private static void ScheduleDataUploadingJob(IScheduler scheduler) {
            const int INTERVAL_IN_MINUTES = 30;
            scheduler.ScheduleJob(
                JobBuilder.Create<DataUploadingJob>().WithIdentity("DataUploadingJob").Build(),
                TriggerBuilder.Create().WithIdentity("DataUploadingTrigger").StartNow().WithSimpleSchedule(x => x.RepeatForever().WithIntervalInMinutes(INTERVAL_IN_MINUTES)).Build());
        }
    }
}
