using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class DataSet {
        [JsonProperty("seriesname")]
        public string SeriesName { get; set; }

        [JsonProperty("data")]
        public IEnumerable<Data> Data { get; set; }

        [JsonProperty("parentyaxis")]
        public string ParentYAxis { get; set; }

        [JsonProperty("renderas")]
        public string RenderAs { get; set; }

		[JsonProperty("alpha")]
		public int? Alpha{get;set;}

		[JsonProperty("showplotborder")]
		public int? ShowPlotBorder{get;set;}

		[JsonProperty("plotbordercolor")]
		public string PlotBorderColor{get;set;}

		[JsonProperty("color")]
		public string Color{get;set;}

		[JsonProperty("linethickness")]
		public int? LineThickness{get;set;}
    }
}
