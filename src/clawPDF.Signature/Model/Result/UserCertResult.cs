using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawPDF.Signature.Model.Result
{
    public class UserCertResult
    {
        /// <summary>
        /// certId
        /// </summary>
        public string CertId { get; set; }
        /// <summary>
        /// 用户证书
        /// </summary>
        public string UserCert { get; set; }
        /// <summary>
        /// 用户证书Id
        /// </summary>
        public string UserCertID { get; set; }
        /// <summary>
        /// 用户证书名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 签章图片
        /// </summary>
        public string PicBase64 { get; set; }
        public bool IsLogin { get; set; }
    }
}
