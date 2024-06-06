using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace clawPDF.Lincense
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (File.Exists("com.lincense"))
            //    File.Delete("com.lincense");
            //File.WriteAllText("com.lincense", Encrypt.Compter().Md5By32());
            //OpenFolder(Application.StartupPath);
            richTextBox1.Text = Encrypt.Compter().Md5By32();
        }
        public void OpenFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                return;

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe");
            startInfo.Arguments = folder;
            process.StartInfo = startInfo;
            try
            {
                process.Start();
            }
            catch (Exception)
            {
            }
        }
    }
}
