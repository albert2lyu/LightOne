using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Web;
using Business;
using System.Threading;
using System.Diagnostics;

namespace Bee.Yhd {
    class YhdProductExtractor {
        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<IEnumerable<Product>> ExtractProductsInCategoryAsync(string categoryNumber) {
            var pages = await GetTotalPageAsync(categoryNumber);

            var results = new ConcurrentBag<Product>(); // 因为使用多线程填充，所以使用线程安全的集合类

            await Task.WhenAll(Enumerable.Range(1, pages).Select(async page => {
                var doc = await DownloadProductListDocumentAsync(categoryNumber, page);
                if (doc != null) {
                    var products = ParseProductsFromHtmlDocument(doc);
                    foreach (var product in products)
                        results.Add(product);
                        //yield return product;
                }
            }));

            return results;
        }

        private async Task<int> GetTotalPageAsync(string categoryNumber) {
            // 爬第一页的数据，为了解析页码
            var doc = await DownloadProductListDocumentAsync(categoryNumber, 1);
            if (doc == null || IsEmptyResult(doc.DocumentNode))
                return 0;

            return ParseTotalPage(doc.DocumentNode);
        }

        private async Task<string> DownloadProductListHtmlAsync(string url) {
            //Logger.Debug("start " + url.GetHashCode());
            using (var webClient = new WebClient()) {
                webClient.Headers.Add(HttpRequestHeader.Cookie, "provinceId=2");    // 北京站
                //webClient.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                webClient.Encoding = Encoding.UTF8;
                var responseContent = await webClient.DownloadStringTaskAsync(url);

                var html = JsonConvert.DeserializeAnonymousType(responseContent, new { value = string.Empty }).value;
                if (string.IsNullOrWhiteSpace(html))
                    throw new ParseException("无法反序列化Json响应内容，缺少value属性？");

                //Logger.Debug("done  " + url.GetHashCode());
                return html;
            }
        }

        private async Task<HtmlDocument> DownloadProductListDocumentAsync(string categoryNumber, int page, int retryTimes = 0) {
            try {
                var defaultHtml = await DownloadProductListHtmlAsync(string.Format(@"http://www.yihaodian.com/ctg/searchPage/c{0}-/b0/a-s1-v0-p{1}-price-d0-f04-m1-rt0-pid-k/", categoryNumber, page));
                // 产品列表具有延迟加载功能，所以需要抓取两次才能获取到完整一页的内容
                var moreHtml = await DownloadProductListHtmlAsync(string.Format(@"http://www.yihaodian.com/ctg/searchPage/c{0}-/b0/a-s1-v0-p{1}-price-d0-f04-m1-rt0-pid-k/?isGetMoreProducts=1", categoryNumber, page));

                var doc = new HtmlDocument();
                doc.LoadHtml(new StringBuilder().Append(defaultHtml).Append(moreHtml).ToString());

                return doc;
            }
            catch (Exception e) {
                Logger.Warn(string.Format("抓取产品信息错误，分类{0}，页码{1}", categoryNumber, page), e);
                return null;
                //throw new ApplicationException(string.Format("下载产品信息错误，分类{0}，页码{1}", categoryNumber, page), e);
                //if (retryTimes < 3)
                //    return GetResponseFromServerAsync(categoryNumber, page, retryTimes + 1);
                //else {
                //Logger.Warn(string.Format("抓取产品信息错误，分类{0}，页码{1}", categoryNumber, page), e);
                //return null;
                //}
            }
        }


        private IEnumerable<Product> ParseProductsFromHtmlDocument(HtmlDocument doc) {
            var products = doc.DocumentNode.SelectNodes(@"//li[@class='producteg']")
                .AsParallel()
                .SelectMany(node => ParseProductsFromLiNode(node))
                .ToList();
            
            // 调用truestock页面，获取真实价格
            SetRealPrice(products);
            return products;
        }

        private async void SetRealPrice(List<Product> products) {
            var index = 0;
            const int BATCH_SIZE = 20;
            while (index < products.Count) {
                var productsInBatch = products.Skip(index).Take(BATCH_SIZE);
                index += BATCH_SIZE;

                var url = "http://busystock.i.yihaodian.com/busystock/restful/truestock?mcsite=1&provinceId=2&" +
                    string.Join("&", productsInBatch.Select(p => string.Format("pmIds={0}", p.Number)));

                using (var webClient = new WebClient()) {
                    try {
                        webClient.Encoding = Encoding.UTF8;
                        var json = await webClient.DownloadStringTaskAsync(url);    // 这里出现异常

                        var productsPrices = JsonConvert.DeserializeAnonymousType(json, new[] {
                            new {
                                pmId = string.Empty,
                                productPrice = (decimal)0
                            }
                        });
                        productsPrices.AsParallel()
                            .ForAll(productPrice => {
                                var product = products.FirstOrDefault(p => p.Number == productPrice.pmId);
                                if (product != null)
                                    product.Price = productPrice.productPrice;
                            });
                    }
                    catch (Exception e) {
                        Logger.Warn("调用获取价格接口异常", e);
                    }
                }
            }
        }

        private bool IsEmptyResult(HtmlNode node) {
            return node.SelectSingleNode(@"//div[@class='emptyResultTipsCondt mt']") != null;
        }

        private int ParseTotalPage(HtmlNode doc) {
            var node = doc.SelectSingleNode(@"//span[@class='pageOp']");
            if (node == null)
                throw new ParseException("无法找到总页数标签：span[class=\"pageOp\"]{0}{1}", Environment.NewLine, doc.OuterHtml);

            var pattern = @"共(\d+)页";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = regex.Match(node.InnerText);
            if (!m.Success)
                throw new ParseException("从字符串'{0}'中解析总页数失败，exp pattern:'{1}'", node.InnerText, pattern);

            return int.Parse(m.Groups[1].Value);
        }

        private IEnumerable<Product> ParseProductsFromLiNode(HtmlNode liTag) {
            var bookNode = liTag.SelectSingleNode(@"./div[contains(@class, 'bookDetail')]");
            if (bookNode == null) {
                // 普通商品
                var divNodes = liTag.SelectNodes(@"./div[contains(@class, 'itemSearchResultCon')]");
                if (divNodes == null)
                    throw new ParseException("无法从产品li标签中解析产品div标签{0}{1}", Environment.NewLine, liTag.OuterHtml);

                foreach (var node in divNodes) {
                    yield return ParseNormalProductFromDivNode(node);
                }
            }
            else {
                // 图书、票务产品
                yield return ParseBookProductFromLiNode(liTag);
            }
        }

        private Product ParseNormalProductFromDivNode(HtmlNode divTag) {
            var productATag = divTag.SelectSingleNode(@"./a[@class='title']");
            if (productATag == null)
                throw new ParseException("无法找到产品标签：div > a[class=\"title\"]");

            var number = productATag.GetAttributeValue("pmid", string.Empty);
            if (string.IsNullOrWhiteSpace(number))
                throw new ParseException("无法解析产品编码：{0}", productATag.OuterHtml);

            // 有些产品名称中带有"，导致title属性解析错误，所以采用InnerText解析产品名称
            //var name = productATag.GetAttributeValue("title", string.Empty);
            var name = ParseNameFromString(productATag.InnerText);
            if (string.IsNullOrWhiteSpace(name))
                throw new ParseException("无法解析产品名称：{0}", productATag.OuterHtml);
            var url = productATag.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(url))
                throw new ParseException("无法解析产品url：{0}", productATag.OuterHtml);

            var productImgTag = divTag.SelectSingleNode(@"./a/img");
            if (productImgTag == null)
                throw new ParseException("无法找到产品图片标签：div > a > img");
            var imgUrl = productImgTag.GetAttributeValue("src", string.Empty);
            if (string.IsNullOrWhiteSpace(imgUrl) || string.Compare(imgUrl, "http://image.yihaodianimg.com/search/global/images/blank.gif", true) == 0)
                imgUrl = productImgTag.GetAttributeValue("original", string.Empty);
            if (string.IsNullOrWhiteSpace(imgUrl))
                throw new ParseException("无法找到产品图片链接：{0}", productImgTag.OuterHtml);

            // 因为有缓存，此处的价格未必准确
            var priceTag = divTag.SelectSingleNode(@"./p[@class='price']//strong");
            if (priceTag == null)
                throw new ParseException("无法找到产品价格标签：div > p[class=\"price\"] > strong");
            var price = !string.IsNullOrWhiteSpace(priceTag.InnerText) ? ParsePriceFromString(priceTag.InnerText) : 0;

            return new Product {
                Number = number,
                Name = name,
                Url = url,
                ImgUrl = imgUrl,
                Price = price,
                Source = "yhd"
            };
        }

        private Product ParseBookProductFromLiNode(HtmlNode liTag) {
            var productATag = liTag.SelectSingleNode(@".//a[@class='title']");
            if (productATag == null)
                throw new ParseException("无法找到产品标签：li >> a[class=\"title\"]");

            var number = productATag.GetAttributeValue("pmid", string.Empty);
            if (string.IsNullOrWhiteSpace(number))
                throw new ParseException("无法解析产品编码：{0}", productATag.OuterHtml);

            // 有些产品名称中带有"，导致title属性解析错误，所以采用InnerText解析产品名称
            //var name = productATag.GetAttributeValue("title", string.Empty);
            var name = ParseNameFromString(productATag.InnerText);
            if (string.IsNullOrWhiteSpace(name))
                throw new ParseException("无法解析产品名称：{0}", productATag.OuterHtml);
            var url = productATag.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(url))
                throw new ParseException("无法解析产品url：{0}", productATag.OuterHtml);

            var productImgTag = liTag.SelectSingleNode(@".//a/img");
            if (productImgTag == null)
                throw new ParseException("无法找到产品图片标签：li >> a > img");
            var imgUrl = productImgTag.GetAttributeValue("src", string.Empty);
            if (string.IsNullOrWhiteSpace(imgUrl) || string.Compare(imgUrl, "http://image.yihaodianimg.com/search/global/images/blank.gif", true) == 0)
                imgUrl = productImgTag.GetAttributeValue("original", string.Empty);
            if (string.IsNullOrWhiteSpace(imgUrl))
                throw new ParseException("无法找到产品图片链接：{0}", productImgTag.OuterHtml);

            // 因为有缓存，此处的价格未必准确
            var priceTag = liTag.SelectSingleNode(@"./div/p/span/strong");
            if (priceTag == null)
                throw new ParseException("无法找到产品价格标签：li > div > p > span > strong");
            var price = !string.IsNullOrWhiteSpace(priceTag.InnerText) ? ParsePriceFromString(priceTag.InnerText) : 0;

            return new Product {
                Number = number,
                Name = name,
                Url = url,
                ImgUrl = imgUrl,
                Price = price,
                Source = "yhd"
            };
        }

        private string ParseNameFromString(string str) {
            // 删除<!-- -->注释中的内容
            if (string.IsNullOrWhiteSpace(str))
                return null;

            string name;

            const string COMMENT_START_TAG = "<!--";
            const string COMMENT_FINISH_TAG = "-->";
            var commentIndexes = new[] { str.IndexOf(COMMENT_START_TAG), str.LastIndexOf(COMMENT_FINISH_TAG) };
            if (commentIndexes[0] != -1 && commentIndexes[1] != -1) {
                name = (str.Substring(0, commentIndexes[0]) + str.Substring(commentIndexes[1] + COMMENT_FINISH_TAG.Length)).Trim();
            }
            else {
                name = new Regex(@"\<!\-\-.*?\-\-\>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline)
                    .Replace(str, string.Empty)
                    .Trim();
            }

            name = HttpUtility.HtmlDecode(name);
            return string.IsNullOrWhiteSpace(name) ? str : name;
        }

        private decimal ParsePriceFromString(string str) {
            // 尝试使用字符串操作解析，因为正则表达式耗性能
            if (string.IsNullOrWhiteSpace(str))
                throw new ParseException("从字符串'{0}'中解析价格失败", str);
            var s = str.Trim(new []{' ', '¥'});

            decimal price;
            if (decimal.TryParse(s, out price))
                return price;
                
            var pattern = @"\d+(.\d+)?";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = regex.Match(str);
            if (!m.Success)
                throw new ParseException("从字符串'{0}'中解析价格失败，exp pattern:'{1}'", str, pattern);

            var val = m.Groups[0].Value;
            if (string.IsNullOrWhiteSpace(val))
                throw new ParseException("从字符串'{0}'中解析价格失败，exp pattern:'{1}'", str, pattern);

            return decimal.Parse(val);
        }
    }
}
