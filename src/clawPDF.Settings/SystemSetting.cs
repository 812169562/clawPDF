using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Settings
{
    public class SystemSetting
    {
        /// <summary>
        /// 单机系统地址
        /// </summary>
        public string RisUrl { get; set; }
        /// <summary>
        /// 登录用户信息
        /// </summary>
        public string LoginUser { get; set; }
        /// <summary>
        /// 打印方式：1、印刷体打印 2、pdf文件打印
        /// </summary>
        public int PrintWay { get; set; }
        /// <summary>
        /// Pdf配置页是否可见
        /// </summary>
        public bool PdfTabVisible { get; set; }
        /// <summary>
        /// OCR配置页是否可见
        /// </summary>
        public bool OCRTabVisible { get; set; }
    }
}
