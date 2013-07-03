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
            return;
            
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();
            
            ScheduleRatioRankingJob(scheduler);
        }

        private static void ScheduleRatioRankingJob(IScheduler scheduler) {
            const int INTERVAL_IN_MINUTES = 5;
            scheduler.ScheduleJob(
                JobBuilder.Create<RatioRankingJob>().WithIdentity("RatioRankingJob").Build(),
                TriggerBuilder.Create().WithIdentity("RatioRankingTrigger").StartNow().WithSimpleSchedule(x => x.RepeatForever().WithIntervalInMinutes(INTERVAL_IN_MINUTES)).Build());
        }
    }
}
