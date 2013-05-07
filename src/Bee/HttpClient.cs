using Common.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Bee {
    class HttpClient {
        private readonly static ILog Logger = LogManager.GetLogger(typeof(HttpClient));

        public static string DownloadString(string url, string cookies = null, int retryTimes = 0) {
            try {
                using (var webClient = new WebClient()) {
                    webClient.Encoding = Encoding.UTF8;
                    if (!string.IsNullOrWhiteSpace(cookies))
                        webClient.Headers.Add(HttpRequestHeader.Cookie, cookies);
                    
                    return webClient.DownloadString(url);
                }
            }
            catch (Exception e) {
                if (retryTimes < 4) {
                    retryTimes++;
                    var retryInterval = TimeSpan.FromSeconds(10 * retryTimes);
                    Logger.Warn(string.Format("{0}下载HTML异常，{1}秒后第{2}次重试", url, retryInterval.TotalSeconds, retryTimes), e);
                    Thread.Sleep(retryInterval);
                    return DownloadString(url, cookies, retryTimes);
                }
                throw e;
            }
        }
    }
}
