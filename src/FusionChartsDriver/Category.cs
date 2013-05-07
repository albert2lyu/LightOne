using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Category {
        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
