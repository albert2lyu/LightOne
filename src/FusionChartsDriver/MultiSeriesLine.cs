using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class MultiSeriesLine {
        [JsonProperty("chart")]
        public Chart Chart { get; set; }

        [JsonProperty("categories")]
        public IEnumerable<Categories> CategoriesCollection { get; set; }

        [JsonProperty("dataset")]
        public IEnumerable<DataSet> DataSetCollection { get; set; }

		[JsonProperty("data")]
		public IEnumerable<Data> DataCollection { get; set; }

		[JsonProperty("trendlines")]
		public IEnumerable<TrendLines> TrendLineCollection{get;set;}

        public string ToJson() {
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(this, Formatting.None, setting);
        }
    }
}
