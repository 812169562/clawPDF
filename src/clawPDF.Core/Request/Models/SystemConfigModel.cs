using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class SystemConfigModel
    {
        /// <summary>
        /// 签署流程 1、独立签署 2、绑定后自动签署
        /// </summary>
        public int SigningProcess {  get; set; }
        /// <summary>
        /// 签名厂商 ywq 医网签 、bjca 北京CA
        /// </summary>
        public string SignatureFirm { get; set; }
    }
}
