using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
	public class SingleSeriesCharts
	{
		[JsonProperty("chart")]
		public Chart Chart { get; set; }

		[JsonProperty("data")]
		public IEnumerable<Data> Data { get; set; }


		public string ToJson()
		{
			var setting = new JsonSerializerSettings();
			setting.NullValueHandling = NullValueHandling.Ignore;
			return JsonConvert.SerializeObject(this, Formatting.None, setting);
		}
	}
}
