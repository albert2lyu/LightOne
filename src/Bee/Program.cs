using System;
using System.Diagnostics;
using System.Linq;
using Bee.Yhd;
using Common.Logging;
using Quartz;
using Quartz.Impl;

namespace Bee {
    public class Program {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        private static void MyTest() {
            var sw = new Stopwatch();
            sw.Start();
            var ds = new YhdDataSource();
            var products = ds.ExtractProductsInCategory("5231")
                                .Distinct(new ProductProxyComparer());   // 因为抓到的数据可能重复，所以需要过滤掉重复数据，否则在多线程更新数据库的时候可能产生冲突
            var x = products.Where(p => p.Number == "8987215").Count();
            Console.WriteLine("download:" + sw.Elapsed);
            //new CategoryProductsProxy { CategoryId = ObjectId.Parse("10290784-6a45-408d-ab85-579fa914efc8"), Products = products }
            //    .SaveOrUpdate();
            //Console.WriteLine("save to db:" + sw.Elapsed);
            Console.ReadLine();
        }

        public static void Main() {
            //MyTest();

            //return;

            Logger.Info("启动");

            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            ScheduleJob(scheduler);
        }

        private static void ScheduleJob(IScheduler scheduler) {
            var jobType = typeof(YhdArchiveJob);
            var jobName = jobType.Name;
            var triggerName = jobType.Name;

            var job = new JobDetail(jobName, jobType);
            var trigger = TriggerUtils.MakeMinutelyTrigger(triggerName, 20, SimpleTrigger.RepeatIndefinitely);

            scheduler.ScheduleJob(job, trigger);
        }
    }
}