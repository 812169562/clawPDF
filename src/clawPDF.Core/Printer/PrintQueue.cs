using clawPDF.Core;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clawSoft.clawPDF.Core.Printer
{
    public static class PrintQueue
    {
        public static ConcurrentDictionary<string, string> PdfFiles;
        public static ConcurrentQueue<string> PdfQueue;
        public static string _printer = "StandAloneRis";
        private static object _lock = new object();
        static PrintQueue()
        {
            PdfFiles = new ConcurrentDictionary<string, string>();
            PdfQueue = new ConcurrentQueue<string>();
            Start();
        }
        public static void Add(string key, string pdfFile)
        {
            PdfFiles.TryAdd(key, pdfFile);
            PdfQueue.Enqueue(key);
        }
        public static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (!Printing(_printer))
                        {
                            Print();
                        }
                        Thread.Sleep(300);
                    }
                    catch (Exception ex)
                    {
                        Log.PrintError(ex.Message);
                    }
                }
            });
        }
        public static bool Print()
        {
            if (PdfQueue.Any())
            {
                lock (_lock)
                {
                    string key;
                    PdfQueue.TryDequeue(out key);
                    var pdfFile = PdfFiles[key];
                    foxitReaderPrintPdf(pdfFile);
                    var files = "";
                    PdfFiles.TryRemove(key, out files);
                    return true;
                }
            }
            return true;
        }
        private static bool foxitReaderPrintPdf(string url)
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
                var printProcess = new Process
                {
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

        public static bool Printing(string printerName)
        {
            // 创建打印服务对象
            LocalPrintServer printServer = new LocalPrintServer();
            // 获取所有打印队列
            System.Printing.PrintQueue queue = printServer.GetPrintQueues().First(t => t.Name == printerName);
            try
            {
                queue.Refresh();
                return queue.IsPrinting;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
