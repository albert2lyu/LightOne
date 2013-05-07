using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Color {
        [JsonProperty("minvalue")]
        public double MinValue { get; set; }

        [JsonProperty("maxvalue")]
        public double MaxValue { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
