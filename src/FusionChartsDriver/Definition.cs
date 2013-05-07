using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
	public class Definition
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("size")]
		public int? Size { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// 1 for Yes, 0 for No
        /// </summary>
        [JsonProperty("bold")]
        public int? Bold { get; set; }
	}
}
