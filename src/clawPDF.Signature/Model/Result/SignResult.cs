using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawPDF.Signature.Model.Result
{
    public class SignResult
    {
        /// <summary>
        /// 签名值
        /// </summary>
        public string SignValue { get; set; }
        /// <summary>
        /// 签名原文
        /// </summary>
        public string OrgData { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { get; set; }
        /// <summary>
        /// 签名证书
        /// </summary>
        public string CertId { get; set; }
    }
}
