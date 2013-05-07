using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
    public class MSStepLine
	{
		[JsonProperty("chart")]
		public Chart Chart { get; set; }

		[JsonProperty("categories")]
		public IEnumerable<Categories> Categories { get; set; }

		[JsonProperty("dataset")]
		public IEnumerable<DataSet> Dataset { get; set; }

        //[JsonProperty("styles")]
        //public Styles Styles { get; set; }


		public string ToJson()
		{
			var setting = new JsonSerializerSettings();
			setting.NullValueHandling = NullValueHandling.Ignore;
			return JsonConvert.SerializeObject(this, Formatting.None, setting);
		}
	}
}
