using clawPDF.Signature.Model.Result;

namespace clawPDF.Signature.Service
{
    public class BJCAService : ISignatureService
    {
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="strCertId">证书操作唯一标识，也支持只输入设备序列号</param>
        /// <param name="strOrgData">签名原文</param>
        /// <returns></returns>
        public Result<SignResult> SignData(string strCertId, string strOrgData)
        {
            try
            {

                BJCA_SVS_CLIENTCOMLib.BJCASVSEngine svs = new BJCA_SVS_CLIENTCOMLib.BJCASVSEngine();
                XTXAppCOMLib.XTXAppClass xtx = new XTXAppCOMLib.XTXAppClass();

                //1、客户端签名，利用私钥对签名原文签名，签名时必须插上key而且必须已经登录  
                if (string.IsNullOrEmpty(strOrgData))
                    throw new System.Exception("签名原文为空");
                Log.Info("strCertId:" + strCertId + ", strOrgData:" + strOrgData);
                string strSignValue = xtx.SOF_SignData(strCertId, strOrgData);//签名数据
                //string strSignValue = xtx.SOF_SignFile(strCertId, strOrgData);
                Log.Info("strSignValue:" + strSignValue);
                if (string.IsNullOrEmpty(strSignValue))
                    throw new System.Exception("客户端签名失败");
                //2、对原文加盖时间戳
                BJCA_TS_CLIENTCOMLib.BJCATSEngine ts = new BJCA_TS_CLIENTCOMLib.BJCATSEngine();
                // 时间戳请求
                string str_TS_Resquest = ts.CreateTSRequest(strOrgData, 0);//产生时间戳请求
                string strTimeStamp = ts.CreateTS(str_TS_Resquest);//加盖时间戳
                if (string.IsNullOrEmpty(strTimeStamp))
                    throw new System.Exception("加盖时间戳失败");
                SignResult sign = new SignResult();
                sign.OrgData = strOrgData;
                sign.SignValue = strSignValue;
                sign.CertId = strCertId;
                sign.TimeStamp = strTimeStamp;
                return ResultUtil.Success(sign);
            }
            catch (System.Exception ex)
            {
                Log.Error($"签名失败：{ex.Message}");
                return ResultUtil.Fail<SignResult>(ex.Message);
            }
        }
    }
}
