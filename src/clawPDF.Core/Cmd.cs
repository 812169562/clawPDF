using System;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.InteropServices;

namespace clawSoft.clawPDF.Core
{
    /// <summary>
    /// 执行命令
    /// </summary>
    public static class Cmd
    {
        ///
        /// 执行cmd.exe命令
        ///
        ///命令文本
        /// 命令输出文本
        public static string ExeCommand(string commandText)
        {
            return ExeCommand(new string[] { commandText });
        }
        ///
        /// 执行多条cmd.exe命令
        ///
        ///命令文本数组
        /// 命令输出文本
        public static string ExeCommand(string[] commandTexts)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            string strOutput = null;
            try
            {
                p.Start();
                foreach (string item in commandTexts)
                {
                    p.StandardInput.WriteLine(item);
                }
                p.StandardInput.WriteLine("Y");
                p.StandardInput.WriteLine("exit");
                strOutput = p.StandardOutput.ReadToEnd();
                //strOutput = Encoding.UTF8.GetString(Encoding.Default.GetBytes(strOutput));
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }
            return strOutput;
        }
        ///
        /// 启动外部Windows应用程序，隐藏程序界面
        ///
        ///应用程序路径名称
        /// true表示成功，false表示失败
        public static bool StartApp(string appName)
        {
            return StartApp(appName, ProcessWindowStyle.Hidden);
        }
        ///
        /// 启动外部应用程序
        ///
        ///应用程序路径名称
        ///进程窗口模式
        /// true表示成功，false表示失败
        public static bool StartApp(string appName, ProcessWindowStyle style)
        {
            return StartApp(appName, null, style);
        }

        /// <summary>
        /// 启动外部应用程序，隐藏程序界面
        /// </summary>
        /// <param name="appName">应用程序路径名称</param>
        /// <param name="arguments">启动参数</param>
        /// <returns></returns>
        public static bool StartApp(string appName, string arguments)
        {
            return StartApp(appName, arguments, ProcessWindowStyle.Hidden);
        }
        /// <summary>
        /// 终止进程
        /// </summary>
        /// <param name="appName">应用程序路径名称</param>
        public static void KillApp(string appName)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                Process[] array = processes;
                foreach (Process process in array)
                {
                    if (process.ProcessName.Contains(appName))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 启动外部应用程序
        /// </summary>
        /// <param name="appName">应用程序路径名称</param>
        /// <param name="arguments">启动参数</param>
        /// <param name="style">进程窗口模式</param>
        /// <param name="blnRst">true表示成功，false表示失败</param>
        /// <returns></returns>
        public static bool StartApp(string appName, string arguments, ProcessWindowStyle style, bool blnRst = false)
        {
            Process p = new Process();
            p.StartInfo.FileName = appName;//exe,bat and so on
            p.StartInfo.WindowStyle = style;
            p.StartInfo.Arguments = arguments;
            try
            {
                p.Start();
                if (blnRst)
                {
                    p.WaitForExit();
                    p.Close();
                }
                blnRst = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return blnRst;
        }
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        const int GWL_STYLE = -16;
        const int WS_VISIBLE = 268435456;
        /// <summary>
        /// 获取应用程序状态
        /// </summary>
        /// <param name="appName"></param>
        /// <returns>true：显示 false：隐藏</returns>
        public static bool AppVisible(string appName)
        {
            Process process = Process.GetProcessesByName(appName).FirstOrDefault();
            if (process != null)
            {
                IntPtr windowHandle = FindWindow(null, process.MainWindowTitle);
                if (windowHandle != IntPtr.Zero)
                {
                    int style = GetWindowLong(windowHandle, GWL_STYLE);
                    bool isWindowVisible = (style & WS_VISIBLE) == WS_VISIBLE;
                    return isWindowVisible;
                }
            }
            return false;
        }

        public static string HexToString(this string hexString)
        {
            try
            {
                var reg = @"^[a-zA-Z]+$";
                var reg2 = @"^[0-9]+$";
                if (Regex.IsMatch(hexString, reg) || Regex.IsMatch(hexString, reg2))
                    return hexString;
                byte[] byteArray = HexStringToByteArray(hexString);
                var val = Encoding.GetEncoding("GB2312").GetString(byteArray);
                if (!val.Contains("?????"))
                {
                    return val;
                }
                return Encoding.UTF8.GetString(byteArray);
            }
            catch (Exception)
            {
                return hexString;
            }
        }

        public static string ConvertAndPrint(string gb2312String)
        {
            // 将GB2312编码的字符串转换为字节序列
            byte[] gb2312Bytes = Encoding.GetEncoding("GB2312").GetBytes(gb2312String);

            // 将GB2312字节序列转换为UTF8编码的字节序列
            byte[] utf8Bytes = Encoding.Convert(Encoding.GetEncoding("GB2312"), Encoding.UTF8, gb2312Bytes);

            // 将UTF8编码的字节序列转换回字符串
            string utf8String = Encoding.UTF8.GetString(utf8Bytes);

            return utf8String;
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (var b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException(nameof(hex));

            var numberOfCharacters = hex.Length;
            var returnArray = new byte[numberOfCharacters / 2];

            for (var i = 0; i < numberOfCharacters; i += 2)
                returnArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return returnArray;
        }
    }
}