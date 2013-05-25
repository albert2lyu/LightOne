using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;

namespace Bee.Yhd {
    class YhdArchiveJob : IStatefulJob {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(JobExecutionContext context) {
            // 先清空所有分类，然后再保存抓取到的分类，这样可以更新分类的变化
            //YhdCategory.DeleteAll();
            Logger.Info("开始抓取一号店数据");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var ds = new YhdDataSource();
            var downloadedCategories = ds.ExtractCategories().ToList();
            Logger.Info(string.Format("下载到{0}个分类", downloadedCategories.Count));
            
            // 保存分类数据并返回需要继续处理的分类
            var needProcessCategories = ServerProxy.UpsertCategories(downloadedCategories);
            Logger.Info(string.Format("需要处理{0}个分类", needProcessCategories.Count()));
            
            var index = 0;
            Parallel.ForEach(needProcessCategories,
                new ParallelOptions { MaxDegreeOfParallelism = 1 },// 并发处理每个分类，这样可以大大加快处理速度
                (category) => {
                    try {
                        Logger.Info(string.Format("{3}/{4} {0}[{1}]{2}",
                            string.Join("", Enumerable.Repeat(".", category.Level - 1)),
                            category.Number,
                            category.Name,
                            Interlocked.Increment(ref index), needProcessCategories.Count()));

                        var products = ds.ExtractProductsInCategory(category.Number)
                            .Distinct(new ProductProxyComparer());   // 因为抓到的数据可能重复，所以需要过滤掉重复数据，否则在多线程更新数据库的时候可能产生冲突

                        new CategoryProductsProxy { CategoryId = category.Id, Products = products }
                            .Upsert();
                    }
                    catch (Exception e) {
                        Logger.Error(string.Format("处理分类{0}{1}失败", category.Name, category.Number), e);
                    }
                });

            Logger.Info(string.Format("抓取一号店数据完成，用时{0:0.#}分", stopwatch.Elapsed.TotalMinutes));
        }
    }
}
