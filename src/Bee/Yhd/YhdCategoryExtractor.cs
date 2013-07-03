using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Logging;
using HtmlAgilityPack;
using Business;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Bee.Yhd {
    class YhdCategoryExtractor {
        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<IEnumerable<Category>> ExtractCategories() {
            var html = await DownloadHtmlFromServer();
            var categories = ParseCategoriesFromHtml(html);
            return categories;
        }

        private async Task<HtmlDocument> DownloadHtmlFromServer() {
            const string ALL_PRODUCT_URL = @"http://www.yihaodian.com/marketing/allproduct.html";

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })) {
                var responseContent = await client.GetStringAsync(ALL_PRODUCT_URL);

                var doc = new HtmlDocument();
                doc.LoadHtml(responseContent);
                return doc;
            }
        }

        private IEnumerable<Category> ParseCategoriesFromHtml(HtmlDocument html) {
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
        private IEnumerable<KeyValuePair<Category, HtmlNode>> ParseCategories(string xpath, HtmlNode outerNode, Category parentCategory, int level) {
            var nodes = outerNode.SelectNodes(xpath);
            if (nodes == null)
                yield break;
            foreach (var node in nodes) {
                var category = ParseCategoryFromANode(node);
                category.Level = level;
                if (parentCategory != null && !string.IsNullOrWhiteSpace(parentCategory.Number))
                    category.ParentNumber = parentCategory.Number;
                yield return new KeyValuePair<Category, HtmlNode>(category, node);
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

        private Category ParseCategoryFromANode(HtmlNode aNode) {
            var url = aNode.GetAttributeValue("href", string.Empty);
            var name = aNode.InnerText;
            var number = ParseCategoryNumberFromUrl(url);
            return new Category { Number = number, Url = url, Name = name, Source = "yhd" };
        }
    }
}
