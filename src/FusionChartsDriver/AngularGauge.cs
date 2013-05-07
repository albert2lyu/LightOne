using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class AngularGauge {
        [JsonProperty("chart")]
        public Chart Chart { get; set; }

        [JsonProperty("colorrange")]
        public ColorRange ColorRange { get; set; }

        [JsonProperty("dials")]
        public Dials Dials { get; set; }

        public string ToJson() {
            var setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(this, Formatting.None, setting);
        }
    }
}
