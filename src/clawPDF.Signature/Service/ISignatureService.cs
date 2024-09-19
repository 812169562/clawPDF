using clawPDF.Signature.Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawPDF.Signature.Service
{
    public interface ISignatureService
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="strCertId">证书操作唯一标识，也支持只输入设备序列号</param>
        /// <param name="strOrgData">签名原文</param>
        /// <returns></returns>
        Result<SignResult> SignData(string strCertId, string strOrgData);
    }
}
