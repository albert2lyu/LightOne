using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
	public class Styles
	{
		[JsonProperty("definition")]
		public IEnumerable<Definition> Definition { get; set; }

		[JsonProperty("application")]
		public IEnumerable<Application> Application { get; set; }
	}
}
