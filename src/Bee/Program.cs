using System;
using System.Diagnostics;
using System.Linq;
using Bee.Yhd;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System.Reflection;

namespace Bee {
    public class Program {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void MyTest() {
            var sw = new Stopwatch();
            sw.Start();
            var task = new YhdProductExtractor().ExtractProductsInCategoryAsync("28441");
            task.Wait();

            var products = task.Result
                                //.Distinct(new ProductComparer())   // 因为抓到的数据可能重复，所以需要过滤掉重复数据，否则在多线程更新数据库的时候可能产生冲突
                                .ToList();
            
            Console.WriteLine(products.Count);
            Console.WriteLine(sw.Elapsed);
            Console.ReadLine();
        }

        public static void Main() {
            //MyTest();
            //return;

            Logger.Info("启动");

            //new YhdArchiveJob().Execute(null);
            //Console.ReadKey(true);
            //return;

            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            ScheduleJob(scheduler);
        }

        private static void ScheduleJob(IScheduler scheduler) {
            const int INTERVAL_IN_SECONDS = 5;
            scheduler.ScheduleJob(
                JobBuilder.Create<YhdArchiveJob>().WithIdentity("YhdArchiveJob").Build(),
                TriggerBuilder.Create().WithIdentity("YhdArchiveTrigger").StartNow().WithSimpleSchedule(x => x.RepeatForever().WithIntervalInSeconds(INTERVAL_IN_SECONDS)).Build());
        }
    }
}