using clawPDF.Core;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;

namespace clawSoft.clawPDF.Core.Request
{
    public class HttpUploadRequest
    {
        private const string QueryUsersUrl = "/ris/out-api/print/getAccountListByEquipment";
        private const string QueryPatientsUrl = "/ris/out-api/print/queryPatientInfo";
        private const string PrintSettingUrl = "/ris/out-api/print/getPrintConfig";
        public HttpUploadRequest()
        {
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<LoginUser> GetLoginUsers(string name)
        {
            if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                throw new Exception("未配置单机系统请求路径！");
            var client = new RestClient(SystemConfig.Setting.RisUrl);
            var request = new RestRequest(QueryUsersUrl, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("mac", GetMacByWMI());
            request.AddParameter("userName", name);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                throw new Exception(response.StatusDescription + response.ErrorMessage);
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
            if (model.Status != "0")
                throw new Exception(model.Message);
            List<LoginUser> data = JsonConvert.DeserializeObject<List<LoginUser>>(model.Data.ToString());
            return data;
        }
        /// <summary>
        /// 获取打印配置
        /// </summary>
        /// <returns></returns>
        public bool GetPrintSetting()
        {
            if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                return false;
            var client = new RestClient(SystemConfig.Setting.RisUrl);
            var request = new RestRequest(PrintSettingUrl, Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("mac", GetMacByWMI());
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
            {
                Log.PrintError("获取打印流程配置失败：" + response.StatusDescription + response.ErrorMessage);
                return false;
            }
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
            if (model.Status != "0")
            {
                Log.PrintError("获取打印流程配置失败：" + model.Message);
                return false;
            }
            return model.Data.ToString() == "2";
        }

        /// <summary>
        /// 获取患者信息
        /// </summary>
        /// <param name="patientName">患者姓名</param>
        /// <param name="requestNum">申请单号</param>
        /// <param name="inpatientNum">住院号</param>
        /// <returns></returns>
        public List<PatientModel> GetPatients(string patientName, string requestNum, string inpatientNum)
        {
            if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                throw new Exception("未配置单机系统请求路径！");
            if (string.IsNullOrEmpty(SystemConfig.userJson))
                throw new Exception("请选择登录账号！");
            var client = new RestClient(SystemConfig.Setting.RisUrl);
            var request = new RestRequest(QueryPatientsUrl, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("JSESSIONID_RIS", "sqOCPPJD171jbE9NPtp1TLstty96MGfHsvE_JZ-P", ParameterType.Cookie);
            LoginUser loginUser = JsonConvert.DeserializeObject<LoginUser>(SystemConfig.userJson);
            var body = new { PatientName = patientName, RequestNum = requestNum, InpatientNum = inpatientNum, HiscaDepartmentId = loginUser.HiscaDepartmentId };
            request.AddJsonBody(body);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                throw new Exception(response.StatusDescription + response.ErrorMessage);
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
            if (model.Status != "0")
                throw new Exception(model.Message);
            List<PatientModel> list = JsonConvert.DeserializeObject<List<PatientModel>>(model.Data.ToString());
            return list;
        }
        /// <summary>
        /// 上传 
        /// </summary>
        /// <returns></returns>
        public string Upload(string uploadUrl, string file, string fileName, PatientModel patient)
        {
            LoginUser loginUser = null;
            object patientDto = null;
            if (patient != null)
            {
                if (string.IsNullOrEmpty(SystemConfig.userJson))
                    throw new Exception("请选择登录账号！");
                loginUser = JsonConvert.DeserializeObject<LoginUser>(SystemConfig.userJson);
                patientDto = new
                {
                    PatientCode = patient.InpatientNum,
                    PatientName = patient.PatientName,
                    ApplyCode = patient.RequestNum,
                    PatientNo = "",
                    AdviceCode = "",
                    PatientType = patient.PatientType,
                    Times = patient.Times,
                    Sex = patient.Sex,
                    Age = patient.Age,
                    CheckItem = patient.CheckItem,
                    ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    DocTitle = patient.DocTitle,
                    BindSource = patient.BindSource
                };
            }
            var client = new RestClient(uploadUrl);
            var request = new RestRequest("", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("JSESSIONID_RIS", "sqOCPPJD171jbE9NPtp1TLstty96MGfHsvE_JZ-P", ParameterType.Cookie);
            var mac = GetMacByWMI();
            var body = new
            {
                Mac = mac,
                PdfBase64 = Convert.ToBase64String(File.ReadAllBytes(file)),
                FileName = fileName,
                BandPatientDto = patientDto,
                LoginCache = loginUser
            };
            request.AddJsonBody(body);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                throw new Exception(response.StatusDescription + response.ErrorMessage);
            return response.Content;
        }
        /// <summary>
        /// 获取mac
        /// </summary>
        /// <returns></returns>
        private static string GetMacByWMI()
        {
            string macs = String.Empty;
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        mac = mo["MacAddress"].ToString().Replace(":", "");
                        macs += mac;
                        break;
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {
            }

            return macs;
        }
    }
}

public class ResponseModel
{
    public object Data { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}
