using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace clawPDF.Lincense
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Today;
        }

        string lincense = "ris.lincense";
        public string val = $"{DateTime.Now.Year}abc@123//..";
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (val != textBox1.Text)
            {
                MessageBox.Show("请输入正确的密码！");
                return;
            }
            if (string.IsNullOrEmpty(richTextBox1.Text))
            {
                MessageBox.Show("请输入授权内容！");
                return;
            }
            if (File.Exists(lincense))
                File.Delete(lincense);
            // var key = File.ReadAllText(file);
            File.WriteAllText(lincense, Encrypt.DesEncryptMD5(richTextBox1.Text, dateTimePicker1.Text.Trim()));
            MessageBox.Show("授权码生成成功！");
            this.Close();
        }
        // string file;
        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    var ok = openFileDialog1.ShowDialog();
        //    if (ok == DialogResult.OK)
        //    {
        //        file = openFileDialog1.FileName;
        //        label3.Text = Path.GetFileName(file);
        //    }
        //}
    }
}
