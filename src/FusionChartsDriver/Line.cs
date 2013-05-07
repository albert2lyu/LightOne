using System;
using System.Linq;
using Newtonsoft.Json;
namespace FusionChartsDriver
{
	public class Line
	{
		[JsonProperty("startvalue")]
		public int? StartValue { get; set; }

		[JsonProperty("color")]
        public string Color{get;set;}

		[JsonProperty("displayvalue")]
        public string DisplayValue{get;set;}

		[JsonProperty("valueonright")]
        public int? ValueonRight{get;set;}
	}
}
