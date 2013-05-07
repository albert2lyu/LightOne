using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
	public class Application
	{
		[JsonProperty("toobject")]
		public string ToObject { get; set; }

		[JsonProperty("styles")]
		public string Styles { get; set; }
	}
}
