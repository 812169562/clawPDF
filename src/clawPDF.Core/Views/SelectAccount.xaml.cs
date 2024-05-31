using clawSoft.clawPDF.Core.Request;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            this.Topmost = true;
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
            if (dataGrid.SelectedItem == null) return;
            LoginUser user = (LoginUser)dataGrid.SelectedItem;
            SystemSetting setting = SystemConfig.Setting;
            setting.LoginUser = Encrypt.DesEncrypt(JsonConvert.SerializeObject(user));
            SystemConfig.Save(setting);
            this.Close();
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
