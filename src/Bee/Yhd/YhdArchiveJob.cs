using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using System.Collections.Generic;
using Business;

namespace Bee.Yhd {
    [DisallowConcurrentExecution]
    class YhdArchiveJob : IJob {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CategoryArchiveService _CategoryArchiveService = new CategoryArchiveService();
        private readonly ProductArchiveService _ProductArchiveService = new ProductArchiveService();
        private readonly ProductRepo _ProductRepo = new ProductRepo();

        public void Execute(IJobExecutionContext context) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Logger.Info("开始抓取一号店数据");

            var downloadedCategories = new YhdCategoryExtractor().Extract().Result;
            var needProcessCategories = _CategoryArchiveService.Archive(downloadedCategories).OrderBy(c => c.ProductsUpdateTime);
            
            var taskLock = new SemaphoreSlim(initialCount: 1);
            var tasks = needProcessCategories.Select(async (category, index) => {
                await taskLock.WaitAsync();
                try {
                    var result = await ProcessCategoryAsync(category);
                    //■◆▲●□◇△○
                    Logger.Info(string.Join(" ", new[]{
                            string.Format("{0}", index + 1),
                            string.Format("[{0}]{1}", category.Number, category.Name),
                            string.Format("□{0} △{1}", result.Total, result.Changed)
                        }));
                }
                catch (Exception e) {
                    Logger.ErrorFormat("处理分类{0}{1}失败", e, category.Name, category.Number);
                }
                finally {
                    taskLock.Release();
                }
            });

            Task.WaitAll(tasks.ToArray());

            Logger.InfoFormat("抓取一号店数据完成，用时{0:0.#}分", stopwatch.Elapsed.TotalMinutes);
        }

        private async Task<dynamic> ProcessCategoryAsync(Category category) {
            // 从网站上抓取产品信息，因为抓到的数据可能重复，所以需要过滤掉重复数据，否则在多线程更新数据库的时候可能产生冲突
            var downloadProducts = await new YhdProductExtractor().ExtractProductsInCategoryAsync(category.Number);
            // 获取已经存在产品的信息签名
            var existingProducts = _ProductRepo.GetByCategoryId(category.Id)
                .Select(p => new ProductSignature { Source = p.Source, Number = p.Number, Signature = p.Signature });

            // 计算刚下载的产品的签名
            downloadProducts.AsParallel().ForAll(p => p.Signature = ProductSignature.ComputeSignature(p));

            // 找到签名发生变化的产品
            var changedProducts = FindChangedProducts(downloadProducts, existingProducts).ToList();

            //await ServerProxy.UpsertProductsAsync(category.Id, changedProducts);
            _ProductArchiveService.Archive(category.Id, changedProducts);

            return new {
                Total = downloadProducts.Count(),
                Changed = changedProducts.Count
            };
        }

        private IEnumerable<Product> FindChangedProducts(IEnumerable<Product> products, IEnumerable<ProductSignature> existsProducts) {
            Func<string, string, string> createKey = (source, number) => {
                return source + "-" + number;
            };
            var existsProductsMap = existsProducts.AsParallel().GroupBy(p => createKey(p.Source, p.Number), p => p.Signature).ToDictionary(p => p.Key, p => p.First());

            return products.AsParallel().Where(p => {
                var key = createKey(p.Source, p.Number);
                return !existsProductsMap.ContainsKey(key) || !Signature.IsMatch(existsProductsMap[key], p.Signature);
            });
        }
    }
}
