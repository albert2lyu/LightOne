using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Data {
        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("toolText")]
        public string ToolText { get; set; }
    }
}
