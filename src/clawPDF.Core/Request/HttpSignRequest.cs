using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Controls;

namespace clawSoft.clawPDF.Core.Request
{
    public class HttpSignRequest
    {
        public HttpSignRequest()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
        }
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SignResult SignData(string strCertId, string strOrgData)
        {
            var client = new RestClient(SystemConfig.Setting.SignServer);
            var request = new RestRequest("/SignatureController/SignData", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("CertId", strCertId);
            request.AddParameter("OrgData", strOrgData);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                throw new Exception(response.StatusDescription + response.ErrorMessage);
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
            if (model.Status == "-1")
                throw new Exception($"签名失败：{model.Message}");
            SignResult data = JsonConvert.DeserializeObject<SignResult>(model.Data.ToString());
            return data;
        }

        /// <summary>
        /// 获取用户证书
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UserCertResult GetUserCert()
        {
            var client = new RestClient(SystemConfig.Setting.SignServer);
            var request = new RestRequest("/SignatureController/GetUserCert", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                throw new Exception(response.StatusDescription + response.ErrorMessage);
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
            if (model.Status == "-1")
                throw new Exception($"获取用户证书失败：{model.Message}");
            UserCertResult data = JsonConvert.DeserializeObject<UserCertResult>(model.Data.ToString());
            return data;
        }
    }
}
