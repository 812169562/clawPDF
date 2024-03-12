using clawSoft.clawPDF.Core.Request;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace clawSoft.clawPDF.Core.Views
{
    /// <summary>
    /// SelectAccount.xaml 的交互逻辑
    /// </summary>
    public partial class SelectAccount : Window
    {
        private readonly HttpUploadRequest _request;
        public SelectAccount()
        {
            InitializeComponent();
            _request = new HttpUploadRequest();
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
                List<LoginUser> list = _request.GetLoginUsers(txtBox.Text);
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
            SystemConfig._accountName = user.AccountName;
            SystemConfig.userJson = JsonConvert.SerializeObject(user);
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
