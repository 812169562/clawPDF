using clawPDF.Core;
using clawSoft.clawPDF.Core.Settings.Enums;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clawSoft.clawPDF.Core.Printer
{
    public static class PrintQueue
    {
        public static ConcurrentDictionary<string, string> PdfFiles;
        public static ConcurrentQueue<string> PdfQueue;
        public static string _printer = "";
        public static SelectPrinter _selectPrinter;
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
                        if (!PrinterUtil.Printing(_printer))
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
                    var newPath = Path.Combine(Path.GetDirectoryName(pdfFile), Path.GetFileNameWithoutExtension(pdfFile) + "_sign" + Path.GetExtension(pdfFile));
                    if (File.Exists(newPath))
                    {
                        foxitReaderPrintPdf(newPath);
                    }
                    else
                    {
                        foxitReaderPrintPdf(pdfFile);
                    }
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
                var arg = @"/p " + "\"" + url + "\"";
                if (_selectPrinter == SelectPrinter.SelectedPrinter)
                {
                    arg = @"/t " + "\"" + url + "\"" + " \"" + _printer + "\"";
                }
                processStartInfo.Arguments = arg;
                //processStartInfo.Arguments = string.Format(" /p  \"{0}\"", url);
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                var printProcess = new Process
                {
                    StartInfo = processStartInfo
                };
                printProcess.Start();
                if (printProcess.WaitForExit(-1))
                    printResult = true;
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
