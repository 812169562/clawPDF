using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace clawPDF.Core
{
    public static class Log
    {

        public static void Error(Exception ex)
        {
            Error(ex.Message + ex.StackTrace);
        }

        public static void Bat(List<string> messages, string name)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "bat" + $"";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"# ----------------------------Start Exceute-----------------------------");
                stringBuilder.AppendLine($"# ----------------------------{name}-----------------------------");
                stringBuilder.AppendLine($"# DateTime{Date.yyyy_MMddHHmmss}");
                foreach (var message in messages)
                {
                    stringBuilder.AppendLine($"{message}");
                }
                stringBuilder.AppendLine($"pause");
                stringBuilder.AppendLine($"# ----------------------------End Exceute---------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{name}.ps1", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }

            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Batch(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "batch" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"info：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Info(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "info" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"info：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }


        public static void Print(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "print" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"上传日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"print：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void PrintError(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "print_error" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"上传错误日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"print：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Sql(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "sql" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"sql：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Error(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "error" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"错误：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMdd}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }
        public static void Fatal(Exception ex)
        {
            Error(ex.Message + ex.StackTrace);
        }
        public static void Fatal(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "fatal" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"错误：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMdd}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Write<T>(T t)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + $"//log";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                if (t == null)
                {
                    stringBuilder.AppendLine($"输出空字符串了：{JsonConvert.SerializeObject(t)}");
                }
                else
                {
                    stringBuilder.AppendLine($"输出：{JsonConvert.SerializeObject(t)}");
                }
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMdd}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Write2(string t)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "log" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                stringBuilder.AppendLine($"输出：{t}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Write(string code, string input, string output, string name, string mdtrtCertNo)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "log" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                stringBuilder.AppendLine($"地区：{code}");
                stringBuilder.AppendLine($"交易编号：{input}");
                stringBuilder.AppendLine($"参数3：隐私");
                stringBuilder.AppendLine($"参数4：{name}");
                stringBuilder.AppendLine($"参数5：{mdtrtCertNo}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMddHH}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Debug(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "debug" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"错误：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMdd}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Warn(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "warn" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"错误：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMdd}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }

        public static void Trace(string message)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "trace" + $"//{Date.yyyyMMdd}";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"-------------------------------------------------------");
                stringBuilder.AppendLine($"交易日期：{Date._yyyyMMddHHmmssSSS}");
                //stringBuilder.AppendLine($"交易编号：{code}");
                //stringBuilder.AppendLine($"输入：{input}");
                //stringBuilder.AppendLine($"输出：{output}");
                stringBuilder.AppendLine($"错误：{message}");
                stringBuilder.AppendLine($"-------------------------------------------------------");
                using (StreamWriter sw = new StreamWriter($"{path}//{Date.yyyyMMdd}.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(stringBuilder.ToString());
                }
            }
            catch (System.Exception ex)
            {
                using (StreamWriter sw = new StreamWriter($"{path}//systemerror.log", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message + ex.StackTrace);
                }
            }
        }
    }

    public class Logger
    {
        public bool IsTraceEnabled => false;
        public static Logger GetCurrentClassLogger()
        {
            return new Logger();
        }

        public void Debug(string msg)
        {
            Log.Debug(msg);
        }

        public void Error(string mgs)
        {
            Log.Error(mgs);
        }

        public void Error(Exception ex)
        {
            Log.Error(ex);
        }
        public void Error(Exception ex, string msg)
        {
            Log.Error(ex);
            Log.Error(msg);
        }
        public void Info(string message)
        {
            Log.Info(message);
        }
        public void Fatal(string message)
        {
            Log.Fatal(message);
        }
        public void Fatal(Exception ex)
        {
            Log.Fatal(ex);
        }
        public void Fatal(Exception ex, string message, bool bo)
        {
            Log.Fatal(ex);
            Log.Fatal(message);
        }
        public void Warn(string message)
        {
            Log.Warn(message);
        }
        public void Warn(Exception ex, string message)
        {
            Log.Warn(ex.Message + ex.StackTrace + message);
        }
        public void Trace(Exception ex)
        {
            Log.Trace(ex.Message + ex.StackTrace);
        }
        public void Trace(string message)
        {
            Log.Trace(message);
        }
    }
}
