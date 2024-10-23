using clawPDF.Core;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static string QueryUsersUrl = "/ris/out-api/print/getAccountListByEquipment"; // 获取设备下所有账户
        private static string QueryPatientsUrl = "/ris/out-api/print/queryPatientInfo"; // 绑定操作 查询患者信息
        private static string PrintSettingUrl = "/ris/out-api/print/getPrintConfig"; // 获取设备打印配置
        private static string SigningProcesUrl = "/ris/setting/signing-process/get"; // 获取是否存在签名流程
        private static string SignSettingUrl = "/ris/out-api/print/getSignConfig"; // 根据绑定患者的检查项目获取签章配置
        private static string BindSignatureAccountUrl = "/ris/out-api/print/bindSignatureAccount"; // 绑定签名账户
        private static string GetUserSignStampUrl = "/ris/out-api/print/getUserSignStamp"; // 获取用户签章信息
        private static string GetSignatureFirmUrl = "/ris/setting/signature-firm/get"; // 获取签名厂商配置



        public static string UploadUrl;
        private static string guid = "QzwQbZxDanJiRistkhHARz9fdBehDF8r";
        public static string _equipmentId;

        static HttpUploadRequest()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
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
                            if (!Directory.Exists(directory)) return;
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
            request.AddParameter("guid", guid);
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
            request.AddParameter("guid", guid);
            IRestResponse response = client.Execute(request);
            Log.Info("获取打印流程配置返回：" + JsonConvert.SerializeObject(response.Content));
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
            PrintSettingModel settingModel = JsonConvert.DeserializeObject<PrintSettingModel>(model.Data.ToString());
            _equipmentId = settingModel.EquipmentId;
            return settingModel.PrintProcess == 2;
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
            var body = new { PatientName = patientName, RequestNum = requestNum, InpatientNum = inpatientNum, loginUser.HiscaDepartmentId, guid };
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
                    ApplyCode = patient.RequestNum,
                    ApplyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PatientNo = "",
                    AdviceCode = "",
                    patient.PatientName,
                    patient.PatientType,
                    patient.Times,
                    patient.Sex,
                    patient.Age,
                    patient.CheckItem,
                    patient.DocTitle,
                    patient.BindSource,
                    patient.Cert,
                    patient.ReportSignatureReqs,
                    //patient.UKeySignPicture,
                    patient.IsSign
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
                    SystemConfig.AuthorizationStatus
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
        /// <summary>
        /// 保存上传失败文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <param name="patient"></param>
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

        /// <summary>
        /// 根据绑定患者的检查项目获取签章配置
        /// </summary>
        /// <returns></returns>
        public static SignConfigModel GetSignSetting(string checkItem)
        {
            SignConfigModel config = new SignConfigModel();
            try
            {
                if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                    return config;
                var client = new RestClient(SystemConfig.Setting.RisUrl);
                var request = new RestRequest(SignSettingUrl, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("checkItem", checkItem);
                request.AddParameter("equipmentId", _equipmentId);
                request.AddParameter("guid", guid);
                Log.Trace($"获取签章配置: {SystemConfig.Setting.RisUrl}{SignSettingUrl}?checkItem={checkItem}&equipmentId={_equipmentId}&guid={guid}");
                IRestResponse response = client.Execute(request);
                Log.Trace($"获取签章配置res: {response.Content}");
                if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                {
                    Log.Error("获取签名配置失败：" + response.StatusDescription + response.ErrorMessage);
                    return config;
                }
                ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                if (model.Status != "0")
                {
                    Log.Error("获取签名配置失败：" + model.Message);
                    return config;
                }
                if (model.Data != null)
                    config = JsonConvert.DeserializeObject<SignConfigModel>(model.Data.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("获取签名配置异常：" + ex.Message);
            }
            return config;
        }
        /// <summary>
        /// 获取是否存在签名流程
        /// </summary>
        /// <returns></returns>
        public static SigningProcesModel GetSigningProces()
        {
            var config = new SigningProcesModel();
            try
            {
                if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                    return config;
                var client = new RestClient(SystemConfig.Setting.RisUrl);
                var request = new RestRequest(SigningProcesUrl, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("guid", guid);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                {
                    Log.Error("获取签名流程失败：" + response.StatusDescription + response.ErrorMessage);
                    return config;
                }
                ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                if (model.Status != "0")
                {
                    Log.Error("获取签名流程失败：" + model.Message);
                    return config;
                }
                config = JsonConvert.DeserializeObject<SigningProcesModel>(model.Data.ToString());
            }
            catch (Exception ex)
            {
                Log.Error("获取签名流程异常：" + ex.Message);
            }
            return config;
        }
        /// <summary>
        /// 获取医网信签章图片
        /// </summary>
        /// <returns></returns>
        public static string GetSignImage(string accountNo)
        {
            try
            {
                if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                    return string.Empty;
                var client = new RestClient(SystemConfig.Setting.RisUrl);
                client.Timeout = 6000;
                var request = new RestRequest(GetUserSignStampUrl, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("guid", guid);
                request.AddParameter("accountNo", accountNo);
                Log.Info($"获取签章图片入参：{GetUserSignStampUrl}accountNo={accountNo}&guid={guid}");
                IRestResponse response = client.Execute(request);
                Log.Info("获取签章图片返回：" + JsonConvert.SerializeObject(response.Content));
                if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                {
                    Log.Error("获取医网信签章图片失败：" + response.StatusDescription + response.ErrorMessage);
                }
                ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                if (model.Status != "0")
                {
                    Log.Error("获取医网信签章图片失败：" + model.Message);
                }
                return model.Data == null ? "" : model.Data.ToString();
            }
            catch (Exception ex)
            {
                Log.Error("获取医网信签章图片异常：" + ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// 绑定签名账户
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void BindSignatureAccount(LoginUser user)
        {
            try
            {
                if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                    return;
                var client = new RestClient(SystemConfig.Setting.RisUrl);
                client.Timeout = 6000;
                var request = new RestRequest(BindSignatureAccountUrl, Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = new
                {
                    guid,
                    uniqueId = user.UniqueId,
                    departmentId = user.HiscaDepartmentId,
                    accountName = user.AccountName,
                    accountNo = user.AccountNo,
                    doctorInfo = user.DoctorInfo,
                    phone = user.Phone,
                    signType = user.SignType,
                };
                request.AddJsonBody(body);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                {
                    Log.Error("绑定签名账户失败：" + response.StatusDescription + response.ErrorMessage);
                    throw new Exception(response.StatusDescription + response.ErrorMessage);
                }
                ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                if (model.Status != "0")
                {
                    Log.Error("绑定签名账户失败：" + model.Message);
                    throw new Exception(model.Message);
                }
                //SystemSetting setting = SystemConfig.Setting;
                //setting.LoginUser = Encrypt.DesEncrypt(JsonConvert.SerializeObject(user));
                //SystemConfig.Save(setting);
            }
            catch (Exception ex)
            {
                Log.Error("绑定签名账户异常：" + ex.Message);
                throw new Exception("绑定签名账户异常：" + ex.Message);
            }
        }
        /// <summary>
        /// 获取签名厂商
        /// </summary>
        /// <returns></returns>
        public static SignatureFirmModel GetSignatureFirm()
        {
            try
            {
                if (string.IsNullOrEmpty(SystemConfig.Setting.RisUrl))
                    return null;
                var client = new RestClient(SystemConfig.Setting.RisUrl);
                client.Timeout = 6000;
                var request = new RestRequest(GetSignatureFirmUrl, Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("guid", guid);
                IRestResponse response = client.Execute(request);
                Log.Info("获取签名厂商返回：" + JsonConvert.SerializeObject(response.Content));
                if (response.StatusCode != HttpStatusCode.OK || response.ResponseStatus != ResponseStatus.Completed)
                {
                    Log.Error("获取签名厂商失败：" + response.StatusDescription + response.ErrorMessage);
                }
                ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                if (model.Status != "0")
                {
                    Log.Error("获取签名厂商失败：" + model.Message);
                }
                if (model.Data != null)
                {
                    var list = JsonConvert.DeserializeObject<List<SignatureFirmModel>>(model.Data.ToString());
                    if (list != null && list.Any())
                        return list.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取签名厂商异常：" + ex.Message);
            }
            return null;
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
