﻿using clawPDF.Core;
using clawSoft.clawPDF.Core.Jobs;
using clawSoft.clawPDF.Core.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clawSoft.clawPDF.Core.Actions
{
    public class HttpUploadAction : IAction, ICheckable
    {
        private const int ActionId = 19;
        private static readonly Logger Logger =Logger.GetCurrentClassLogger();
        public ActionResult Check(ConversionProfile profile)
        {
            var actionResult = new ActionResult();
            //if (profile.Ftp.Enabled)
            //{
            //    if (string.IsNullOrEmpty(profile.Ftp.Server))
            //    {
            //        Logger.Error("No FTP server specified.");
            //        actionResult.Add(ActionId, 100);
            //    }

            //    if (string.IsNullOrEmpty(profile.Ftp.UserName))
            //    {
            //        Logger.Error("No FTP username specified.");
            //        actionResult.Add(ActionId, 101);
            //    }

            //    if (profile.AutoSave.Enabled)
            //        if (string.IsNullOrEmpty(profile.Ftp.Password))
            //        {
            //            Logger.Error("Automatic saving without ftp password.");
            //            actionResult.Add(ActionId, 109);
            //        }
            //}

            return actionResult;
        }

        public ActionResult ProcessJob(IJob job)
        {
            Logger.Debug("Launched http-post-Action");
            try
            {
                var result = HttpUpload(job);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("Exception while upload file to ftp:\r\n" + ex.Message);
                return new ActionResult(ActionId, 999);
            }
        }
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
        private ActionResult HttpUpload(IJob job)
        {
            Log.Info("开始上传");
            //if (string.IsNullOrEmpty(job.Profile.HttpUploader.HttpUploadUrl))
            //{
            //    Logger.Error("No Http Upload Url specified in action");
            //    return new ActionResult(ActionId, 102);
            //}
            //if (job.Profile.HttpUploader.Enabled)
            {
                String url = job.Profile.HttpUploader.HttpUploadUrl;
                if (url == null || url.Trim().Length == 0)
                {
                    //url = "http://192.168.126.51:8866/ris/hospital/reportDoc/syn";
                    //url = "http://betainner.51trust.com/ris/hospital/reportDoc/syn";
                    url = "http://devinner.51trust.com/ris/hospital/reportDoc/syn";
                }

                //System.Windows.Forms.MessageBox.Show("url:" + url);A
                if (!url.StartsWith("http"))
                {
                    return new ActionResult();
                }
                foreach (var file in job.OutputFiles)
                {

                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.Encoding = Encoding.UTF8;
                            //client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            string mac = GetMacByWMI();
                            //var values = new NameValueCollection();
                            //values.Add("mac", GetMacByWMI());
                            //values.Add("pdfBase64", Convert.ToBase64String(File.ReadAllBytes(file)));
                            //values["mac"] = GetMacByWMI();
                            //values["pdfBase64"] = Convert.ToBase64String(File.ReadAllBytes(file));
                            //File.WriteAllText("C:\\base64xx.txt", Convert.ToBase64String(File.ReadAllBytes(file)), Encoding.UTF8);
                            Log.Print("打印开始");
                            foxitReaderPrintPdf(file);
                            Log.Print(file);
                            var response = client.UploadString(url, "POST", "{\"mac\":\"" + mac + "\",\"pdfBase64\":\"" + Convert.ToBase64String(File.ReadAllBytes(file)) + "\"}");
                            Log.Print(response);
                            //var resStr = Encoding.UTF8.GetString(response);
                            //if (job.Profile.HttpUploader.HttpUploadUrl == null || job.Profile.HttpUploader.HttpUploadUrl.Trim().Length == 0)
                            //    System.Windows.Forms.MessageBox.Show("req:" + url + ",data:" + "{\"mac\":\"" + mac + "\",\"pdfBase64\":\"" + Convert.ToBase64String(File.ReadAllBytes(file)) + "\"}" + ",\r\nresponse:" + response);
                            //if (response == null || response.IndexOf("\"status\":\"0\"") == -1)
                            //{
                            //    System.Windows.Forms.MessageBox.Show("上传失败:" +);
                            //}
                            //TODO
                            //var serializer = new Newtonsoft.Json.();
                            Dictionary<string, object> json = (Dictionary<string, object>)JsonConvert.DeserializeObject(response);
                            if (json.ContainsKey("status"))
                            {
                                string status = json["status"].ToString();
                                if (status == null || !status.Equals("0"))
                                {
                                    System.Windows.Forms.MessageBox.Show(mac + " 上传失败:" + json["message"].ToString(), "警告");
                                    Log.PrintError(response);
                                }
                                else
                                {
                                    if (job.Profile.HttpUploader.HttpUploadUrl == null || job.Profile.HttpUploader.HttpUploadUrl.Trim().Length == 0)
                                        System.Windows.Forms.MessageBox.Show(mac + " 上传成功", "提示");
                                    if (File.Exists(file))
                                    {
                                        try
                                        {
                                            File.Delete(file);
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("虚拟打印组件执行出错:" + ex.Message);
                        System.Windows.Forms.MessageBox.Show("虚拟打印组件执行出错:" + ex.Message);
                    }
                }
            }
            return new ActionResult();
        }

        private bool foxitReaderPrintPdf(string url)
        {
            var printResult = true;
            try
            {
                var processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = Application.StartupPath + "\\TemrsPrint.exe";
                processStartInfo.Arguments = string.Format(" /p  \"{0}\"", url);
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                var printProcess = new Process {
                    StartInfo = processStartInfo
                };
                printProcess.Start();
            }
            catch (Exception ex)
            {
                printResult = false;
                Log.Error("FRP执行打印队列出错:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

            return printResult;
        }
    }
}
