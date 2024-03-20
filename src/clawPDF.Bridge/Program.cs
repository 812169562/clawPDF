﻿using clawPDF.Core;
using clawPDF.Core.Helper;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using static clawPDF.Bridge.ProcessExtensions;

namespace clawPDF.Bridge
{
    internal class Program
    {
        private static string filePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\NetworkPrinter.bin";

        private static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major == 5 || true)
            //if (Environment.OSVersion.Version.Major == 5)
            {
                XP(args);
            }
            else
            {
                Win7(args);
            }
        }

        private static void XP(string[] args)
        {
            try
            {
                Log.Debug("启动");
                String standardInputFilename = Path.GetTempFileName();
                using (BinaryReader standardInputReader = new BinaryReader(Console.OpenStandardInput()))
                {
                    using (FileStream standardInputFile = new FileStream(standardInputFilename, FileMode.Create, FileAccess.ReadWrite))
                    {
                        standardInputReader.BaseStream.CopyTo(standardInputFile);
                    }
                }
                Log.Info(standardInputFilename);
                StripNoDistill(standardInputFilename);
                var infFileTemp = standardInputFilename.Replace("tmp", "inf");
                CreateInf(infFileTemp, standardInputFilename);
                start(infFileTemp);
                //这是一种曲线救国的方式等待研究
                //if (args != null || args.Length != 0)
                //{
                //    try
                //    {
                //        string inffile = args[0].Split('=')[1];
                //        Log.Info("参数1" + args[0]);
                //        var infFileTemp = inffile.Replace("ps", "inf");
                //        CreateInf(infFileTemp, inffile);
                //        if (File.Exists(infFileTemp))
                //        {
                //            start(infFileTemp);
                //        }
                //        else
                //        {
                //            Log.Info("未找到文件--" + inffile);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Log.Error(ex);
                //        Log.Error("错误的桥接 参数--" + args[0]);
                //    }
                //}
                //else
                //{
                //    Log.Error("未获取到参数");
                //}
                //Log.Error("结束");
            }
            catch (Exception ex2)
            {
                Log.Error(ex2);
                MessageBox.Show("打印错误；查看文件夹下logs" + ex2.Message);
            }
        }

        public enum NoDistillStripping : int
        {
            Searching,
            Removing,
            Complete
        }
        static void StripNoDistill(String postscriptFile)
        {

            String strippedFile = Path.GetTempFileName();

            using (StreamReader inputReader = new StreamReader(File.OpenRead(postscriptFile), System.Text.Encoding.UTF8))
            using (StreamWriter strippedWriter = new StreamWriter(File.OpenWrite(strippedFile), new UTF8Encoding(false)))
            {
                NoDistillStripping strippingStatus = NoDistillStripping.Searching;
                String inputLine;
                while (!inputReader.EndOfStream)
                {
                    inputLine = inputReader.ReadLine();
                    if (inputLine != null)
                    {
                        switch ((int)strippingStatus)
                        {
                            case (int)NoDistillStripping.Searching:
                                if (inputLine == "%ADOBeginClientInjection: DocumentSetup Start \"No Re-Distill\"")
                                    strippingStatus = NoDistillStripping.Removing;
                                else
                                    strippedWriter.WriteLine(inputLine);
                                break;
                            case (int)NoDistillStripping.Removing:
                                if (inputLine == "%ADOEndClientInjection: DocumentSetup Start \"No Re-Distill\"")
                                    strippingStatus = NoDistillStripping.Complete;
                                break;
                            case (int)NoDistillStripping.Complete:
                                strippedWriter.WriteLine(inputLine);
                                break;
                        }
                    }
                }
                strippedWriter.Close();
                inputReader.Close();

                File.Delete(postscriptFile);
                File.Move(strippedFile, postscriptFile);
            }
        }
        private static void start(string infFile)
        {
            Log.Info("进入桥接--" + infFile);
            INIFile iniFile = new INIFile(infFile);
            string username = _username;
            if (string.IsNullOrEmpty(username))
            {
                username = Environment.UserName;
            }
            Log.Info("进入桥接1--" + username);
            Log.Info("进入桥接2--" + Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe");
            Log.Info("进入桥接3--" + Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe" + " /INFODATAFILE=" + infFile);
            Log.Info("进入桥接4--" + Path.GetDirectoryName(Application.ExecutablePath));
            try
            {
                if (Environment.OSVersion.Version.Major == 5)
                {
                    Cmd.StartApp(Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe", "\"/INFODATAFILE=" + infFile + "\"");
                }
                else
                {
                    ProcessExtensions.StartProcessAsUser(username, Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe", Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe" + " /INFODATAFILE=" + infFile, Path.GetDirectoryName(Application.ExecutablePath), true);
                }
            }
            catch (Exception)
            {
                Cmd.StartApp(Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe", "\"/INFODATAFILE=" + infFile + "\"");
            }
            //
        }

        private static string _username = "";
        private static void CreateInf(string infFile, string sourceFile)
        {
            var tempName = "-SZYXPrint";
            var path2 = Path.GetDirectoryName(Application.ExecutablePath) + @"\template1.inf";
            var path = Path.GetDirectoryName(Application.ExecutablePath) + @"\template.inf";
            var sb = new StringBuilder();
            File.Copy(path2, path, true);
            sb.AppendLine("[0]");
            sb.AppendLine($"SpoolFileName={Path.GetFileName(sourceFile)}");
            //InfHelper.WriteString("0", "SpoolFileName", Path.GetFileName(sourceFile), path);
            //InfHelper.WriteString("0", "Username", Environment.UserName, path);
            try
            {
                string[] text = File.ReadLines(sourceFile).ToArray();
                //Log.Print(text);
                foreach (string line in text)
                {
                    if (line.Contains("%%For") && !sb.ToString().Contains("Username"))
                    {
                        _username = line.Replace("%%For:", "").Trim();
                        sb.AppendLine($"Username={_username}");
                        break;
                    }
                }
                foreach (string line in text)
                {
                    if (line.Contains("%%Title"))
                    {
                        tempName = line.Replace("%%Title: <", "").Replace(">", "").Trim();
                        tempName = tempName.HexToString();
                        Log.Print(tempName);
                        break;
                    }
                    if (line.Contains("%% Title"))
                    {
                        tempName = line.Replace("%% Title: <", "").Replace(">", "").Trim();
                        tempName = tempName.HexToString();
                        Log.Print(tempName);
                        break;
                    }
                }
                sb.AppendLine($"DocumentTitle={tempName}");
                //InfHelper.WriteString("0", "DocumentTitle", tempName, path);
                var totalPages = text[text.Length - 4].Replace("%%Pages: ", "").Trim();
                //InfHelper.WriteString("0", "TotalPages", totalPages, path);
                sb.AppendLine($"TotalPages={totalPages}");
                File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
                File.Copy(path, infFile, true);
                //throw new Exception("1");
            }
            catch (Exception ex)
            {
                InfHelper.WriteString("0", "TotalPages", "1", path);
                throw ex;
            }
        }

        private static void Usage()
        {
            Log.Bridge("clawPDF.Bridge " + Assembly.GetExecutingAssembly().GetName().Version);
            Log.Bridge("usage:");
            Log.Bridge("clawPDF.Bridge.exe [/Networkprinter=Enable|Disable /Username=user [/Domain=domain]]");
        }

        private static string GetPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("");
                Console.WriteLine("Password cannot be empty.");
                Environment.Exit(1);
            }

            Console.WriteLine("");

            return password;
        }

        private static void SaveLogon(string username, string domain, string password)
        {
            byte[] data = Encoding.UTF8.GetBytes($"{domain},{username},{password}");
            byte[] encryptedData = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
            File.WriteAllBytes(filePath, encryptedData);

            FileSecurity fileSecurity = new FileSecurity(filePath, AccessControlSections.None);
            fileSecurity.SetAccessRuleProtection(true, false);

            AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));
            foreach (AuthorizationRule rule in rules)
            {
                if (rule is FileSystemAccessRule fsRule)
                {
                    fileSecurity.RemoveAccessRule(fsRule);
                }
            }

            IdentityReference builtinAdministrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            FileSystemAccessRule builtinAdministratorsRule = new FileSystemAccessRule(
                builtinAdministrators,
                FileSystemRights.FullControl,
                AccessControlType.Allow);

            fileSecurity.AddAccessRule(builtinAdministratorsRule);

            File.SetAccessControl(filePath, fileSecurity);
        }

        private static void DeleteLogon()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("");
                Console.WriteLine("Network printing was deleted successfully.");
            }
        }

        private static Dictionary<string, Dictionary<string, string>> ReadIniFile(string filePath)
        {
            Dictionary<string, Dictionary<string, string>> iniData = new Dictionary<string, Dictionary<string, string>>();
            string currentSection = "";

            foreach (string line in File.ReadAllLines(filePath))
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    iniData[currentSection] = new Dictionary<string, string>();
                }
                else if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith(";"))
                {
                    int equalsIndex = trimmedLine.IndexOf('=');
                    if (equalsIndex >= 0)
                    {
                        string key = trimmedLine.Substring(0, equalsIndex).Trim();
                        string value = trimmedLine.Substring(equalsIndex + 1).Trim();
                        iniData[currentSection][key] = value;
                    }
                }
            }

            return iniData;
        }

        private static void StartAsPrintedUser(string infFile)
        {
            Dictionary<string, Dictionary<string, string>> iniData = ReadIniFile(infFile);
            string username = iniData["0"]["Username"];
            Log.Bridge(Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe" + " /INFODATAFILE=" + "\"" + infFile + "\"");
            StartProcessAsUser(username, Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe", Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe" + " /INFODATAFILE=" + "\"" + infFile + "\"", Path.GetDirectoryName(Application.ExecutablePath), true);
        }

        private static void StartAsNetworkPrinter(string infFile)
        {
            try
            {
                byte[] encryptedData = File.ReadAllBytes(filePath);
                byte[] data = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.LocalMachine);
                string[] credentials = Encoding.UTF8.GetString(data).Split(',');
                string domain = credentials[0];
                string username = credentials[1];
                string password = credentials[2];

                FileInfo infFileInfo = new FileInfo(infFile);

                FileSecurity infFileSecurity = infFileInfo.GetAccessControl();
                FileSystemAccessRule infRule = new FileSystemAccessRule(
                    username,
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);

                infFileSecurity.AddAccessRule(infRule);
                infFileInfo.SetAccessControl(infFileSecurity);

                FileInfo psFileInfo = new FileInfo(infFile.Replace(".inf", ".ps"));

                FileSecurity psFileSecurity = psFileInfo.GetAccessControl();
                FileSystemAccessRule psRule = new FileSystemAccessRule(
                    username,
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);

                psFileSecurity.AddAccessRule(psRule);
                psFileInfo.SetAccessControl(psFileSecurity);

                if (String.IsNullOrEmpty(domain)) domain = Environment.MachineName;

                GrantAccessToWindowStationAndDesktop(username);
                GrantAccessToWindowStationAndDesktop(domain + "\\" + username);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.LoadUserProfile = true;
                startInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\" + "clawPDF.exe";
                startInfo.Arguments = "/INFODATAFILE=" + "\"" + infFile + "\"";
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                startInfo.UserName = username;
                startInfo.Password = SecureStringHelper.ConvertToSecureString(password);
                startInfo.Domain = domain;

                Process.Start(startInfo);
            }
            catch { }
        }

        public static void Win7(string[] args)
        {
            Log.Bridge("启动");
            var clp = new CommandLineParser(args);
            var showUsage = true;
            Log.Bridge(JsonConvert.SerializeObject(args));

            if (clp.HasArgument("Networkprinter"))
            {
                showUsage = false;

                try
                {
                    switch (clp.GetArgument("Networkprinter").ToLower())
                    {
                        case "enable":
                            Log.Bridge("");
                            Log.Bridge("> Use only an account with the minimum required permissions");
                            Log.Bridge("");
                            string username = clp.GetArgument("Username");
                            string domain = ".";
                            if (clp.HasArgument("Domain")) domain = !string.IsNullOrEmpty(clp.GetArgument("Domain")) ? clp.GetArgument("Domain") : ".";
                            Log.Bridge("Enter the password of the user '" + username + "': ");
                            string password = GetPassword();
                            Log.Bridge("Repeat the password of the user '" + username + "': ");
                            string repeat = GetPassword();
                            if (password == repeat)
                            {
                                SaveLogon(username, domain, password);
                                Log.Bridge("");
                                Log.Bridge("> Authentication validation");
                                IntPtr token = IntPtr.Zero;

                                if (LogonUserW(username, domain, password, (int)LOGON_TYPE.LOGON32_LOGON_NETWORK, (int)LOGON_PROVIDER.LOGON32_PROVIDER_DEFAULT, out token))
                                {
                                    Log.Bridge("");
                                    Log.Bridge("Network printing was set up successfully");
                                }
                                else
                                {
                                    Log.Bridge("");
                                    Log.Bridge("Incorrect credentials");
                                }
                            }
                            else
                            {
                                Log.Bridge("");
                                Log.Bridge("Error: Password doesn't match");
                            }
                            break;

                        case "disable":
                            DeleteLogon();
                            break;

                        default:
                            showUsage = true;
                            break;
                    }
                }
                catch
                {
                    showUsage = true;
                    Environment.ExitCode = 1;
                }
            }

            if (clp.HasArgument("INFODATAFILE"))
            {
                showUsage = false;
                try
                {
                    string infFile = clp.GetArgument("INFODATAFILE");

                    showUsage = false;
                    if (File.Exists(infFile))
                    {
                        Dictionary<string, Dictionary<string, string>> iniData = ReadIniFile(infFile);
                        string clientComputer = iniData["0"]["ClientComputer"];
                        if (clientComputer.ToLower() == Environment.MachineName.ToLower())
                        {
                            StartAsPrintedUser(infFile);
                        }
                        else
                        {
                            StartAsNetworkPrinter(infFile);
                        }
                    }
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    Environment.ExitCode = 1;
                }
            }

            if (showUsage) Usage();
        }
    }

    public static class SecureStringHelper
    {
        public static System.Security.SecureString ConvertToSecureString(string password)
        {
            System.Security.SecureString securePassword = new System.Security.SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            return securePassword;
        }
    }
}