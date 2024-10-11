using clawSoft.clawPDF.Core.Request;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace clawSoft.clawPDF.Core.Views
{
    /// <summary>
    /// SelectAccount.xaml 的交互逻辑
    /// </summary>
    public partial class SelectAccount : Window
    {
        public SelectAccount()
        {
            InitializeComponent();
            //this.Topmost = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAccount();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            LoadAccount();
        }

        private void LoadAccount()
        {
            try
            {
                List<LoginUser> list = HttpUploadRequest.GetLoginUsers(txtBox.Text);
                dataGrid.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem();
        }

        private void SelectedItem()
        {
            try
            {
                if (dataGrid.SelectedItem == null) return;
                LoginUser user = (LoginUser)dataGrid.SelectedItem;
                var signatureFirm = HttpUploadRequest.GetSignatureFirm();
                if (signatureFirm?.SignType == 2)
                {
                    var userCert = HttpSignRequest.GetUserCert();
                    if (!userCert.IsLogin || user.AccountNo.IsEmpty() || user.AccountNo != userCert.UserCertID)
                    {
                        Cmd.KillApp("clawPDF.Signature");
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Signature", "clawPDF.Signature.exe");
                        Cmd.StartApp(path, SystemConfig.Setting.SignServer, ProcessWindowStyle.Normal, false);
                        var visible = true;
                        while (visible)
                        {
                            Thread.Sleep(2000);
                            visible = Cmd.AppVisible("clawPDF.Signature");
                            if (!visible)
                            {
                                userCert = HttpSignRequest.GetUserCert();
                                if (userCert.IsLogin)
                                {
                                    user.AccountNo = userCert.UserCertID;
                                    user.DoctorInfo = $"{userCert.UserName}||{userCert.CertId}";
                                    user.SignType = 2;
                                    HttpUploadRequest.BindSignatureAccount(user);
                                }
                            }
                        }
                    }
                }
                SystemSetting setting = SystemConfig.Setting;
                setting.LoginUser = Encrypt.DesEncrypt(JsonConvert.SerializeObject(user));
                SystemConfig.Save(setting);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedItem();
        }
    }
}
