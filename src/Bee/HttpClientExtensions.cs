using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bee {
    public static class HttpClientExtensions {
        public static async Task<string> GetStringAsync(this HttpClient client, string url, int maxRetryTimes) {
            var responseContent = string.Empty;
            
            bool faield;
            var retryTimes = 0;
            do {
                if (retryTimes > 0)
                    Thread.Sleep(retryTimes * 1000);

                try {
                    responseContent = await client.GetStringAsync(url);
                    faield = false;
                }
                catch {
                    faield = true;
                }
            } while (faield && retryTimes++ < maxRetryTimes);

            return responseContent;
        }
    }
}
