using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Dials {
        [JsonProperty("dial")]
        public IEnumerable<Dial> DialArray { get; set; }
    }
}
