using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
    public class Area2D
    {
        /// <summary>
        /// 动画效果
        /// </summary>
        private string animation;
        public string Animation
        {
            get
            {
                return (String.IsNullOrEmpty(animation) ? "1" : animation);
            }

            set
            {
                animation = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string bgSwfAlpha;
        public string BgSwfAlpha
        {
            get
            {
                return (String.IsNullOrEmpty(bgSwfAlpha) ? "100" : bgSwfAlpha);
            }

            set { bgSwfAlpha = value; }
        }

        public string BgSwf { get; set; }

        /// <summary>
        /// 显示Y轴值
        /// </summary>
        private string showyaxisvalues;
        public string ShowyAxisValues
        {
            get
            {
                return (String.IsNullOrEmpty(showyaxisvalues) ? "0" : showyaxisvalues);
            }

            set { showyaxisvalues = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OutcnvBaseFontSize { get; set; }


        private string outcnvBaseFontColor;
        public string OutcnvBaseFontColor
        {
            get
            {
                return (String.IsNullOrEmpty(outcnvBaseFontColor) ? "000000" : outcnvBaseFontColor);
            }
            set
            {
                outcnvBaseFontColor = value;
            }
        }

        /// <summary>
        /// 基准字体大小
        /// </summary>
        public string basefontsize { get; set; }

        /// <summary>
        /// 基准字体颜色
        /// </summary>
        public string basefontcolor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string canvasbgratio { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string borderalpha { get; set; }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public string bordercolor { get; set; }

        /// <summary>
        /// 数字后缀
        /// </summary>
        public string numbersuffix { get; set; }

        /// <summary>
        /// 显示情节边框
        /// </summary>
        public string showplotborder { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public string bgcolor { get; set; }

        /// <summary>
        /// 格线颜色
        /// </summary>
        public string vdivlinecolor { get; set; }

        public string vdivlinealpha { get; set; }

        public string plotfillalpha { get; set; }

        public string plotfillangle { get; set; }

        /// <summary>
        /// 画锚
        /// </summary>
        public string drawanchors { get; set; }

        public string anchorborderthickness { get; set; }

        public string anchorbgalpha { get; set; }

        public string anchorbgcolor { get; set; }

        public string anchorbordercolor { get; set; }

        public string anchorradius { get; set; }

        public string plotgradientcolor { get; set; }

        public string numvdivlines { get; set; }

        public string canvasbgalpha { get; set; }

        public string canvasborderalpha { get; set; }

        public string numdivlines { get; set; }

        public string yaxisname { get; set; }

        public string showvalues { get; set; }

        public Set[] Data { get; set; }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
