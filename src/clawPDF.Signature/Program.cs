using Nancy.Hosting.Self;
using System;
using System.Windows.Forms;

namespace clawPDF.Signature
{
    static class Program
    {
        private static NancyHost _nancyHost;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var msg = "http://localhost:23201";
            if (args != null && args.Length > 0)
            {
                msg = args[0];
                Log.Info("--------" + msg);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += Application_ApplicationExit;
            var config = new HostConfiguration
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            _nancyHost = new NancyHost(config, new Uri(msg));
            _nancyHost.Start();

            Application.Run(new Login());
        }
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            _nancyHost?.Stop();
        }
    }
}
