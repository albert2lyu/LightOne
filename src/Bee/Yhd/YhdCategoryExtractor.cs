using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Logging;
using HtmlAgilityPack;

namespace Bee.Yhd {
    class YhdCategoryExtractor {
        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<CategoryProxy> ExtractCategories() {
            var html = DownloadHtmlFromServer();
            var categories = ParseCategoriesFromHtml(html);
            return categories;
        }

        private HtmlDocument DownloadHtmlFromServer(int retryTimes = 0) {
            const string ALL_PRODUCT_URL = @"http://www.yihaodian.com/product/listAll.do";
            try {
                var webClient = new HtmlWeb();
                return webClient.Load(ALL_PRODUCT_URL);
            }
            catch (Exception e) {
                Logger.Warn("下载分类信息异常", e);
                if (retryTimes < 5) {
                    retryTimes++;
                    Logger.Info(string.Format("30秒后第{0}次重试", retryTimes));
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    return DownloadHtmlFromServer(retryTimes);
                }
                else
                    throw;
            }
        }

        private IEnumerable<CategoryProxy> ParseCategoriesFromHtml(HtmlDocument html) {
            //Logger.Info(html.DocumentNode.OuterHtml);
            // 已服务方式运行，需要安装TMG代理
            var sort = 0;
            foreach (var outerNode in html.DocumentNode.SelectNodes(@"//div[@class='alonesort']")) {
                var first = ParseCategories(".//h3/a", outerNode, null, 1).FirstOrDefault();
                var firstCategory = first.Key;
                if (firstCategory == null)
                    throw new ParseException("没有找到一级分类");
                firstCategory.Sort = sort++;
                yield return firstCategory;

                // 解析二级分类
                foreach (var second in ParseCategories(".//dt/a", outerNode, firstCategory, 2)) {
                    var secondCategory = second.Key;
                    var secondCategoryNode = second.Value;
                    secondCategory.Sort = sort++;
                    yield return secondCategory;

                    // 解析三级分类
                    foreach (var third in ParseCategories("../..//dd//a", secondCategoryNode, secondCategory, 3)) {
                        var thirdCategory = third.Key;
                        thirdCategory.Sort = sort++;
                        yield return thirdCategory;
                    }
                }
            }
        }

        /// <summary>
        /// 解析分类
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="outerNode"></param>
        /// <param name="parentCategory"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<CategoryProxy, HtmlNode>> ParseCategories(string xpath, HtmlNode outerNode, CategoryProxy parentCategory, int level) {
            var nodes = outerNode.SelectNodes(xpath);
            if (nodes == null)
                yield break;
            foreach (var node in nodes) {
                var category = ParseCategoryFromANode(node);
                category.Level = level;
                if (parentCategory != null && !string.IsNullOrWhiteSpace(parentCategory.Number))
                    category.ParentNumber = parentCategory.Number;
                yield return new KeyValuePair<CategoryProxy, HtmlNode>(category, node);
            }
        }

        /// <summary>
        /// 从url中解析分类编号
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ParseCategoryNumberFromUrl(string url) {
            var pattern = @"\/c(\d+)\-";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = regex.Match(url);
            if (!m.Success)
                throw new ParseException("从url:'{0}'中解析分类number失败，exp pattern:'{1}'", url, pattern);

            var number = m.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(number))
                throw new ParseException("从url:'{0}'中解析分类number失败，exp pattern:'{1}'", url, pattern);

            return number;
        }

        private CategoryProxy ParseCategoryFromANode(HtmlNode aNode) {
            var url = aNode.GetAttributeValue("href", string.Empty);
            var name = aNode.InnerText;
            var number = ParseCategoryNumberFromUrl(url);
            return new CategoryProxy { Number = number, Url = url, Name = name, Source = "yhd" };
        }
    }
}
