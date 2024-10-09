using clawPDF.Signature.Model.Result;
using clawPDF.Signature.Service;
using Nancy;

namespace clawPDF.Signature.Controller
{
    public class SignatureController : NancyModule
    {
        public SignatureController()
        {
            Get["/SignatureController/GetUserCert"] = _ =>
            {
                UserCertResult certResult = new UserCertResult();
                certResult.CertId = Login.strCertId;
                certResult.UserCert = Login.strUserCert;
                certResult.UserCertID = Login.strUserCertID;
                certResult.UserName = Login.strUserName;
                certResult.PicBase64 = Login.strPicBase64;
                certResult.IsLogin = Login.IsLogin;
                return ResultUtil.Success(certResult);
            };
            Get["/SignatureController/SignData"] = _ =>
            {
                var strCertId = Request.Query["CertId"];
                var strOrgData = Request.Query["OrgData"];
                return SignatureService.Instance.SignData(strCertId, strOrgData);
            };
        }
    }
}
