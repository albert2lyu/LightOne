using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
	public class TrendLines
	{
		[JsonProperty("line")]
		public IEnumerable<Line> LineData { get; set; }
	}
}
