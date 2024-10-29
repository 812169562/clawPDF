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
            bool hide = false;
            if (args != null && args.Length > 0)
            {
                hide = args[0] == "false";
                msg = args.Length > 1 ? args[1] : msg;
                Login.accountNo = args.Length > 2 ? args[2] : "";
                Log.Info($"{hide}---{msg}--{Login.accountNo}--{args.Length}");
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

            Login login = new Login();
            if (hide)
            {
                login.Opacity = 0;
                login.ShowInTaskbar = false;
            }
            Application.Run(login);
        }
        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            _nancyHost?.Stop();
        }
    }
}
