using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace Common
{
	public class Env
	{
		public readonly static string ServerName;
		public readonly static string ServerIp;

		static Env()
		{
			ServerName = Dns.GetHostName();

			// 只取IPv4地址
			var ip = Dns.GetHostAddresses(ServerName).Where(t => !t.IsIPv6LinkLocal).Select(t => t.ToString()).ToArray();
			ServerIp = string.Join(",", ip);
		}

		/// <summary>
		/// 字符串MD5加密，返回大写字母
		/// </summary>
		/// <param name="plainText">明文</param>
		/// <returns>密文（大写字母）</returns>
		public static string Encrypt(string plainText)
		{
			MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
			byte[] b = md.ComputeHash(Encoding.Default.GetBytes(plainText));
			string s = string.Empty;
			for (int i = 0; i < b.Length; i++)
			{
				s += (b[i].ToString("x2"));
			}
			return s.ToUpper();
		}


        public static string ClientIp {
            get {
                var httpReq = HttpContext.Current.Request;
                if (httpReq == null)
                    return string.Empty;
                // 经过F5转发的请求，源客户端IP会记录在X-Forwarded-For头中
                var clientIp = httpReq.Headers.Get("X-Forwarded-For");
                if (string.IsNullOrEmpty(clientIp))
                    clientIp = httpReq.UserHostAddress;
                return clientIp;
            }
        }
	}
}
