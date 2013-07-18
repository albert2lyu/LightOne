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

        public async Task<IEnumerable<Category>> Extract() {
            var doc = await DownloadHtmlDocument();
            return ParseCategories(doc);
        }

        private async Task<HtmlDocument> DownloadHtmlDocument() {
            const string URL = @"http://www.yihaodian.com/marketing/allproduct.html";

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })) {
                var responseContent = await client.GetStringAsync(URL, 3);

                var doc = new HtmlDocument();
                doc.LoadHtml(responseContent);
                return doc;
            }
        }

        private IEnumerable<Category> ParseCategories(HtmlDocument doc) {
            Logger.Debug("解析分类html...");
            var sort = 0;
            foreach (var outerNode in doc.DocumentNode.SelectNodes(@"//div[@class='alonesort']")) {
                var first = ParseCategories(".//h3/a", outerNode, null, 1).FirstOrDefault();
                var firstCategory = first.Category;
                if (firstCategory == null)
                    throw new ParseException("没有找到一级分类");
                firstCategory.Sort = sort++;
                yield return firstCategory;

                // 解析二级分类
                foreach (var second in ParseCategories(".//dt/a", outerNode, firstCategory, 2)) {
                    var secondCategory = second.Category;
                    var secondCategoryNode = second.Node;
                    secondCategory.Sort = sort++;
                    yield return secondCategory;

                    // 解析三级分类
                    foreach (var third in ParseCategories("../..//dd//a", secondCategoryNode, secondCategory, 3)) {
                        var thirdCategory = third.Category;
                        thirdCategory.Sort = sort++;
                        yield return thirdCategory;
                    }
                }
            }
            Logger.Debug("完成解析");
        }

        /// <summary>
        /// 解析分类
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="outerNode"></param>
        /// <param name="parentCategory"></param>
        /// <returns></returns>
        private IEnumerable<dynamic> ParseCategories(string xpath, HtmlNode outerNode, Category parentCategory, int level) {
            var nodes = outerNode.SelectNodes(xpath);
            if (nodes == null)
                yield break;

            foreach (var node in nodes) {
                var category = ParseCategoryFromANode(node);
                category.Level = level;
                if (parentCategory != null && !string.IsNullOrWhiteSpace(parentCategory.Number))
                    category.ParentNumber = parentCategory.Number;
                yield return new { Category = category, Node = node };
            }
        }

        /// <summary>
        /// 从url中解析分类编号
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ParseCategoryNumberFromUrl(string url) {
            // url形如：http://www.yihaodian.com/ctg/s2/c21306-%E6%89%8B%E6%9C%BA%E9%80%9A%E8%AE%AF-%E6%95%B0%E7%A0%81%E7%94%B5%E5%99%A8/
            const string PATTERN = @"\/c(\d+)\-";
            var regex = new Regex(PATTERN, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = regex.Match(url);
            if (!m.Success)
                throw new ParseException("从url:'{0}'中解析分类number失败，exp pattern:'{1}'", url, PATTERN);

            var number = m.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(number))
                throw new ParseException("从url:'{0}'中解析分类number失败，exp pattern:'{1}'", url, PATTERN);

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
