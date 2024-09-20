using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class SignConfigModel
    {
        public SignConfigModel()
        {
            SignPage = 3;
            XWide = 504;
            YHigh = 787;
        }
        /// <summary>
        /// 签名页码
        /// </summary>
        public int SignPage { get; set; }
        /// <summary>
        /// x
        /// </summary>
        public int XWide { get; set; }
        /// <summary>
        /// y
        /// </summary>
        public int YHigh { get; set; }
        public string PaperDirection { get; set; }
        public string PaperType { get; set; }
    }
}
