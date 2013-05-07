using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FusionChartsDriver
{
    public class Column3D
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string caption { get; set; }

        /// <summary>
        /// 横轴标题名称
        /// </summary>
        public string xAxisName { get; set; }

        /// <summary>
        /// 纵轴标题名称
        /// </summary>
        public string yAxisName { get; set; }

        /// <summary>
        /// 是否显示值
        /// </summary>
        public string showValues { get; set; }

        /// <summary>
        /// 数字前缀
        /// </summary>
        public string numberPrefix { get; set; }

        /// <summary>
        /// 线的粗细
        /// </summary>
        public int lineThickness { get; set; }

        /// <summary>
        /// 锚半径
        /// </summary>
        public double anchorRadius { get; set; }


        public Set[] Data { get; set; }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
