using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class VLine : IData {
        [JsonProperty("vline")]
        private readonly string _VLine = "true";

        //[JsonProperty("value")]
        //public object Value { get; set; }
    }
}
