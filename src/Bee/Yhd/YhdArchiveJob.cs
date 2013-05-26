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
    class YhdArchiveJob : IStatefulJob {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(JobExecutionContext context) {
            // 先清空所有分类，然后再保存抓取到的分类，这样可以更新分类的变化
            //YhdCategory.DeleteAll();
            Logger.Info("开始抓取一号店数据");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var downloadedCategories = YhdDataSource.ExtractCategories().ToList();
            Logger.Info(string.Format("下载到{0}个分类", downloadedCategories.Count));
            
            // 保存分类数据并返回需要继续处理的分类
            var upsertCategoriesTask = ServerProxy.UpsertCategoriesAsync(downloadedCategories);
            upsertCategoriesTask.Wait();
            var needProcessCategories = upsertCategoriesTask.Result;
            Logger.Info(string.Format("需要处理{0}个分类", needProcessCategories.Count()));

            var taskLock = new SemaphoreSlim(initialCount: 10);
            var tasks = needProcessCategories.Select(async (category, index) => {
                await taskLock.WaitAsync();
                try {
                    var result = await ProcessCategoryAsync(category);
                    //■◆▲●□◇△○
                    Logger.Info(string.Join(" ", new[]{
                            string.Format("{0}/{1}", index + 1, needProcessCategories.Count()),
                            string.Format("[{0}]{1}", category.Number, category.Name),
                            string.Format("□{0} △{1}", result.Total, result.Changed)
                        }));
                }
                catch (Exception e) {
                    Logger.Error(string.Format("处理分类{0}{1}失败", category.Name, category.Number), e);
                }
                finally {
                    taskLock.Release();
                }
            });

            Task.WaitAll(tasks.ToArray());

            Logger.Info(string.Format("抓取一号店数据完成，用时{0:0.#}分", stopwatch.Elapsed.TotalMinutes));
        }

        private async Task<dynamic> ProcessCategoryAsync(Category category) {
            // 获取服务器上的产品信息签名
            var productSignatures = await ServerProxy.GetProductSignaturesByCategoryIdAsync(category.Id);

            // 从网站上抓取产品信息
            var downloadTask = await YhdDataSource.ExtractProductsInCategoryAsync(category.Number);
            // 因为抓到的数据可能重复，所以需要过滤掉重复数据，否则在多线程更新数据库的时候可能产生冲突
            var downloadProducts = downloadTask.Distinct(new ProductComparer());

            // 找到发生变化的产品
            var changedProducts = FindChangedProducts(downloadProducts, productSignatures).ToList();

            await ServerProxy.UpsertProductsAsync(category.Id, changedProducts);

            return new {
                Total = downloadProducts.Count(),
                Changed = changedProducts.Count
            };
        }

        private IEnumerable<Product> FindChangedProducts(IEnumerable<Product> products, IEnumerable<ProductSignature> existsProducts) {
            Func<string, string, string> createKey = (source, number) => {
                return source + "-" + number;
            };
            var existsProductsMap = existsProducts.GroupBy(p => createKey(p.Source, p.Number), p => p.Signature).ToDictionary(p => p.Key, p => p.First());
            
            return products.Where(p=> {
                var key = createKey(p.Source, p.Number);
                var signature = ProductSignature.Create(p).Signature;

                return !existsProductsMap.ContainsKey(key) || !Signature.IsMatch(existsProductsMap[key], signature);
            });
        }
    }
}
