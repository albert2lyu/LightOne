using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Common.Logging;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;

namespace Bee {
    public static class ProxyHelper {
        private readonly static string BaseUrl = ConfigurationManager.AppSettings["ServerUrl"];
        private readonly static ILog Logger = LogManager.GetLogger(typeof(ProxyHelper));

        private static T Execute<T>(Func<WebClient, string> action, int retryTimes = 0) {
            try {
                using (var client = new WebClient()) {
                    client.Encoding = Encoding.UTF8;
                    client.BaseAddress = BaseUrl;

                    var response = action.Invoke(client);
                    return JsonConvert.DeserializeObject<T>(response);
                }
            }
            catch (Exception e) {
                if (retryTimes < 4) {
                    retryTimes++;
                    var retryInterval = TimeSpan.FromSeconds(10 * retryTimes);
                    Logger.Warn(string.Format("{0}发送http请求异常，{1}秒后第{2}次重试", BaseUrl, retryInterval.TotalSeconds, retryTimes), e);
                    Thread.Sleep(retryInterval);
                    return Execute<T>(action, retryTimes);
                }
                throw e;
            }
        }

        public static T Get<T>(string urlFormat, params string[] urlArgs) {
            return ProxyHelper.Execute<T>((client) => {
                //return client.DownloadString(string.Format(urlFormat, urlArgs));
                return client.DownloadString(string.Format(urlFormat, urlArgs));
            });
        }

        public static T Post<T>(string url, object data) {
            return ProxyHelper.Execute<T>((client) => {
                var json = JsonConvert.SerializeObject(data);

                if (json.Length > 1024) {
                    var compressedData = CompressData(Encoding.UTF8.GetBytes(json));
                    client.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
                    var response = client.UploadData(url, compressedData);
                    return Encoding.UTF8.GetString(response);
                }
                else {
                    return client.UploadString(url, json);
                }
            });
        }

        private static byte[] CompressData(byte[] bytes) {
            using (var memStream = new MemoryStream()) {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Compress, true)) {
                    zipStream.Write(bytes, 0, bytes.Length);
                }
                return memStream.ToArray();
            }
        }
    }
}