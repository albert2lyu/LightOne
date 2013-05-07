using System;
using System.Linq;
using Newtonsoft.Json;

namespace FusionChartsDriver {
    public class Chart {
        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("xaxisname")]
        public string XAxisName { get; set; }

        [JsonProperty("yaxisname")]
        public string YAxisName { get; set; }

        [JsonProperty("lineThickness")]
        public int? LineThickness { get; set; }

        [JsonProperty("pyaxisname")]
        public string PYAxisName { get; set; }

        [JsonProperty("syaxisname")]
        public string SYAxisName { get; set; }

        [JsonProperty("showvalues")]
        public int? ShowValues { get; set; }

        [JsonProperty("showLabels")]
        public int? ShowLabels { get; set; }

        [JsonProperty("showLegend")]
        public int? ShowLegend { get; set; }

        [JsonProperty("showpercentintooltip")]
        public int? ShowPercentInTooltip { get; set; }

        [JsonProperty("showBorder")]
        public int? ShowBorder { get; set; }

        [JsonProperty("bgColor")]
        public string BgColor { get; set; }

        [JsonProperty("baseFontSize")]
        public int BaseFontSize { get; set; }

        [JsonProperty("baseFont")]
        public string BaseFont { get; set; }

        [JsonProperty("baseFontColor")]
        public string BaseFontColor { get; set; }

        [JsonProperty("bgAlpha")]
        public int BgAlpha { get; set; }

        [JsonProperty("upperlimit")]
        public int? UpperLimit { get; set; }

        [JsonProperty("lowerlimit")]
        public int? LowerLimit { get; set; }

        [JsonProperty("lowerLimitDisplay")]
        public string LowerLimitDisplay { get; set; }

        [JsonProperty("upperLimitDisplay")]
        public string UpperLimitDisplay { get; set; }

        [JsonProperty("palette")]
        public string Palette { get; set; }

        [JsonProperty("numberSuffix")]
        public string NnumberSuffix { get; set; }

		[JsonProperty("areaovercolumns")]
		public int? AreaOverColumns{get;set;}

		[JsonProperty("useroundedges")]
		public int? UseroundEdges{get;set;}

		[JsonProperty("legendborderalpha")]
		public int? LegendBorderAlpha{get;set;}

		[JsonProperty("formatnumberscale")]
		public int? FormatNumberScale{get;set;}

		[JsonProperty("pieyscale")]
		public int? Pieyscale{get;set;}

		[JsonProperty("slicingdistance")]
		public int? SlicingDistance{get;set;}

		[JsonProperty("pieinnerfacealpha")]
		public int? PieInnerfaceAlpha{get;set;}

		[JsonProperty("plotfillalpha")]
		public int? PlotFillAlpha{get;set;}
        //[JsonProperty("tickValueDistance")]
        //public int? TickValueDistance;

        [JsonProperty("showValue")]
        public int? ShowValue { get; set; }

        [JsonProperty("startingAngle")]
        public int? StartingAngle { get; set; }

        [JsonProperty("enableSmartLabels")]
        public int? EnableSmartLabels { get; set; }

        [JsonProperty("useRoundEdges")]
        public int? UseRoundEdges { get; set; }

        [JsonProperty("plotGradientColor")]
        public string PlotGradientColor { get; set; }

        [JsonProperty("animation")]
        public int? Animation { get; set; }

        [JsonProperty("defaultAnimation")]
        public int? DefaultAnimation { get; set; }

        [JsonProperty("outCnvBaseFontColor")]
        public string OutCnvBaseFontColor { get; set; }
		[JsonProperty("numberprefix")]
		public string NumberPrefix { get; set; }
		[JsonProperty("showSum")]
		public string ShowSum { get; set; }

        public Chart() {
            BaseFontSize = 13;
            BgAlpha = 100;
			BaseFont = "微软雅黑";
			ShowSum = "0";
            //ShowValues = 1;
            //ShowLabels = 1;
            //ShowLegend = 1;
            //ShowPercentInTooltip = 1;
            //ShowBorder = 1;

        }
    }
}
