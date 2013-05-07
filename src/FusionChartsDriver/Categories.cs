using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Categories {
        [JsonProperty("category")]
        public IEnumerable<Category> CategoryCollection { get; set; }
    }
}
