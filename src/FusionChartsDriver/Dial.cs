using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Dial {
        [JsonProperty("value")]
        public double Value { get; set; }

        //"borderalpha": "0",
        //"bgcolor": "000000",
        //"basewidth": "28",
        //"topwidth": "1",
        //"": "130"
        //[JsonProperty("radius")]
        //public int Radius{get;set;}
    }
}
