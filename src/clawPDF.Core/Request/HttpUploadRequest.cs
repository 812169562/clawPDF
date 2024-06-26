using clawPDF.Core;
using clawSoft.clawPDF.Core.Printer;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace clawSoft.clawPDF.Core.Request
{
    public static class HttpUploadRequest
    {
        private static string QueryUsersUrl = "/ris/out-api/print/getAccountListByEquipment";
        private static string QueryPatientsUrl = "/ris/out-api/print/queryPatientInfo";
        private static string PrintSettingUrl = "/ris/out-api/print/getPrintConfig";
        public static string UploadUrl;

        static HttpUploadRequest()
        {
            Start();
        }
        public static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(UploadUrl))
                        {
                            var directory = Application.StartupPath + "\\failfile\\";
                            var files = Directory.GetFiles(directory);
                            foreach (var file in files)
                            {
                                try
                                {
                                    if (Path.GetExtension(file) == "xml")
                                        continue;

                                    var xml = Path.Combine(directory, Path.GetFileNameWithoutExtension(file) + ".xml");
                                    PatientModel patient = null;
                                    if (File.Exists(xml))
                                    {
                                        XmlDocument doc = new XmlDocument();
                                        // 读取XML
                                        doc.Load(xml);
                                        // 读取特定节点
                                        XmlNode rootNode = doc.DocumentElement;
                                        string txet = rootNode.SelectSingleNode("patient").InnerText;
                                        patient = JsonConvert.DeserializeObject<PatientModel>(txet);
                                    }
                                    Upload(UploadUrl, file, Path.GetFileNameWithoutExtension(file), patient);
                                    if (File.Exists(file))
                                        File.Delete(file);
                                    if (File.Exists(xml))
                                        File.Delete(xml);
                                }
                                catch (Exception ex)
                                {
                                    Log.PrintError("定时上传出错：" + ex.Message);
                                }
                                Thread.Sleep(1000);
                            }
                        }
                        Thread.Sleep(3000);
                    }
                    catch (Exception ex)
                    {
                        Log.PrintError(ex.Message);
                    }
                }
            });
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<LoginUser> GetLoginUsers(string name)
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
        public static bool GetPrintSetting()
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
        public static List<PatientModel> GetPatients(string patientName, string requestNum, string inpatientNum)
        {
            if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                throw new Exception("未配置单机系统请求路径！");
            var client = new RestClient(SystemConfig.Setting.RisUrl);
            var request = new RestRequest(QueryPatientsUrl, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("JSESSIONID_RIS", "sqOCPPJD171jbE9NPtp1TLstty96MGfHsvE_JZ-P", ParameterType.Cookie);
            LoginUser loginUser = GetLoginUser();
            if (loginUser == null)
                throw new Exception("请选择登录账号！");
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
        public static string Upload(string uploadUrl, string file, string fileName, PatientModel patient)
        {
            Log.Trace("上传单机系统--uploadUrl:" + uploadUrl + ",file:" + file + ",fileName:" + fileName);
            LoginUser loginUser = null;
            object patientDto = null;
            if (patient != null)
            {
                loginUser = GetLoginUser();
                if (loginUser == null)
                    throw new Exception("请选择登录账号！");
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
            var list = SplitFileUpload(file);
            var message = "";
            foreach (var item in list)
            {
                var client = new RestClient(uploadUrl);
                var request = new RestRequest("", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                //request.AddParameter("JSESSIONID_RIS", "sqOCPPJD171jbE9NPtp1TLstty96MGfHsvE_JZ-P", ParameterType.Cookie);
                var mac = GetMacByWMI();
                // authorizationStatus 0:未授权 1:已授权 2：已失效
                var body = new
                {
                    Mac = mac,
                    //PdfBase64 = Convert.ToBase64String(File.ReadAllBytes(file)),
                    //PdfBase64List = list,
                    FileName = fileName,
                    BandPatientDto = patientDto,
                    LoginCache = loginUser,
                    FileParam = item,
                    AuthorizationStatus = SystemConfig.AuthorizationStatus
                };
                request.AddJsonBody(body);
                Log.Trace("上传单机系统--request:" + JsonConvert.SerializeObject(body));
                IRestResponse response = client.Execute(request);
                Log.Trace("上传单机系统--response:" + JsonConvert.SerializeObject(response.Content));
                if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                    throw new Exception(response.StatusDescription + response.ErrorMessage);
                ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                if (model.Status != "0")
                    throw new Exception(model.Message);
                if (File.Exists(item.FilePath))
                    File.Delete(item.FilePath);
                //if (item.Index == item.Total)
                //{
                //    message = response.Content;
                //}
                //else
                //{
                //    ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                //    if (model.Status != "0")
                //        throw new Exception(model.Message);
                //}
            }
            return message;
        }
        /// <summary>
        /// 大文件切片
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<FileParam> SplitFileUpload(string filePath)
        {
            List<FileParam> fileList = new List<FileParam>();
            var key = Guid.NewGuid().ToString();
            var temFolder = System.Windows.Forms.Application.StartupPath + "\\partFile\\" + key;
            //var temFolder = $"D:\\szyx\\partFile\\{key}";
            // 确保目标文件夹存在
            if (!Directory.Exists(temFolder))
                Directory.CreateDirectory(temFolder);

            // 计算分片大小（字节）
            int chunkSizeBytes = 2 * 1024 * 1024;
            // 打开源文件
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int fileLength = (int)fileStream.Length;
                int chunkCount = (int)Math.Ceiling((double)fileLength / chunkSizeBytes);
                for (int i = 0; i < chunkCount; i++)
                {
                    int chunkStart = i * chunkSizeBytes;
                    int chunkLength = Math.Min(chunkSizeBytes, fileLength - chunkStart);

                    // 创建分片文件
                    string chunkFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_temp_{i}{Path.GetExtension(filePath)}";
                    string chunkFilePath = Path.Combine(temFolder, chunkFileName);
                    using (FileStream chunkFileStream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write))
                    {
                        fileStream.Position = chunkStart;
                        byte[] buffer = new byte[chunkLength];
                        int bytesRead = fileStream.Read(buffer, 0, chunkLength);
                        chunkFileStream.Write(buffer, 0, bytesRead);
                        fileList.Add(new FileParam
                        {
                            PdfBase64 = Convert.ToBase64String(buffer),
                            Index = i + 1,
                            Total = chunkCount,
                            TotalSize = fileLength,
                            FileName = chunkFileName,
                            FilePath = chunkFilePath,
                            Guid = key

                        });
                        chunkFileStream.Close();
                    }
                }
                fileStream.Close();
            }
            return fileList;
        }
        /// <summary>
        /// 获取解密后的登录用户信息
        /// </summary>
        /// <returns></returns>
        public static LoginUser GetLoginUser()
        {
            if (string.IsNullOrEmpty(SystemConfig.Setting.LoginUser)) return null;
            var userJson = Encrypt.DesDecrypt(SystemConfig.Setting.LoginUser);
            return JsonConvert.DeserializeObject<LoginUser>(userJson);
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

        public static void SaveFile(string file, string fileName, PatientModel patient)
        {
            var folder = Application.StartupPath + "\\failfile\\";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, Path.GetFileName(file));
            if (patient == null)
            {
                File.Copy(file, path, true);
                return;
            }
            // 生成XML
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(declaration);

            XmlElement root = doc.CreateElement("root");
            doc.AppendChild(root);

            XmlElement pat = doc.CreateElement("patient");
            pat.InnerText = JsonConvert.SerializeObject(patient);
            root.AppendChild(pat);

            var xml = Path.Combine(folder, Path.GetFileNameWithoutExtension(file) + ".xml");
            doc.Save(xml);
            File.Copy(file, path, true);
        }
    }
}

public class ResponseModel
{
    public object Data { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
}
public class FileParam
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public string Guid { get; set; }
    /// <summary>
    /// 当前为第几片
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// 分片后总片数
    /// </summary>
    public int Total { get; set; }
    /// <summary>
    /// 文件总大小
    /// </summary>
    public int TotalSize { get; set; }
    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }
    /// <summary>
    /// 文件Base64
    /// </summary>
    public string PdfBase64 { get; set; }
    /// <summary>
    /// 分片文件路径
    /// </summary>
    public string FilePath { get; set; }
}
