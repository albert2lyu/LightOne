using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class ColorRange {
        [JsonProperty("color")]
        public IEnumerable<Color> Colors { get; set; }
    }
}
