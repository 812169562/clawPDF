using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class SigningProcesModel
    {
        /// <summary>
        /// 签署流程 1:绑定后自动签名 2:独立部署
        /// </summary>
        public int SignatureProcessConfigWay {  get; set; }
    }
}
