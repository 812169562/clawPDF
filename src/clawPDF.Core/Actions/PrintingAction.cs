using System;
using clawPDF.Core;
using clawSoft.clawPDF.Core.Ghostscript;
using clawSoft.clawPDF.Core.Ghostscript.OutputDevices;
using clawSoft.clawPDF.Core.Jobs;
using clawSoft.clawPDF.Core.Printer;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Core.Views;
using NLog;

namespace clawSoft.clawPDF.Core.Actions
{
    /// <summary>
    ///     Implements the action to print the input files
    /// </summary>
    public class PrintingAction : IAction
    {
        private const int ActionId = 13;
        protected static NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly GhostScript _ghostscript;

        public PrintingAction(GhostScript ghostscript)
        {
            _ghostscript = ghostscript;

        }

        /// <summary>
        ///     Prints the input files to the configured printer
        /// </summary>
        /// <param name="job">The job to process</param>
        /// <returns>An ActionResult to determine the success and a list of errors</returns>
        public ActionResult ProcessJob(IJob job)
        {
            Logger.Debug("Launched Printing-Action");

            try
            {
                if (SystemConfig.Setting.PrintWay != 2)
                {
                    OutputDevice printingDevice = new PrintingDevice(job);
                    _ghostscript.Run(printingDevice, job.JobTempFolder);
                    return new ActionResult();
                }
                PrintQueue._selectPrinter = job.Profile.Printing.SelectPrinter;
                PrintQueue._printer = job.Profile.Printing.PrinterName;
                if (job.Profile.Printing.SelectPrinter == Settings.Enums.SelectPrinter.ShowDialog)
                {
                    SelectPrinter dialog = new SelectPrinter();
                    dialog.Height = 300;
                    dialog.Width = 400;
                    dialog.ShowDialog();
                    if (string.IsNullOrEmpty(dialog.PrintName))
                        throw new Exception("取消打印！");
                    PrintQueue._printer = dialog.PrintName;
                }
                foreach (var item in job.OutputFiles)
                {
                    var key = Date.Number4();
                    PrintQueue.Add(key, item);
                }
                return new ActionResult();
            }
            catch (Exception ex)
            {
                try
                {
                    var errorCode = Convert.ToInt32(ex.Message);
                    return new ActionResult(ActionId, errorCode);
                }
                catch
                {
                    Logger.Error("Error while printing");
                    return new ActionResult(ActionId, 999);
                }
            }
        }
    }
}