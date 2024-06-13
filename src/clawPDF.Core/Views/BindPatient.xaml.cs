using clawSoft.clawPDF.Core.Request;
using clawSoft.clawPDF.Core.Request.Models;
using DrawTools.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace clawSoft.clawPDF.Core.Views
{
    /// <summary>
    /// BindPatient.xaml 的交互逻辑
    /// </summary>
    public partial class BindPatient : Window
    {
        public string file;
        public PatientModel _patient;
        public LoginUser _user;
        public BindPatient()
        {
            InitializeComponent();
            //this.Topmost = true;
            this.WindowState = WindowState.Maximized;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            _user = HttpUploadRequest.GetLoginUser();
        }
        /// <summary>
        /// 窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageEditor.FilePath = file;// "D:\\szyx\\test-pdf\\00001\\a8eccece20ac4f06bf304b56df2cc2bc.pdf";
            if (_user == null)
            {
                MessageBox.Show("请选择医师账号！", "提示", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            txtAccount.Text = _user.AccountName;
        }
        /// <summary>
        /// 跳过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear1_Click(object sender, RoutedEventArgs e)
        {
            _patient = null;
            ImageEditor.SavePdfFile();
            this.Close();
        }
        /// <summary>
        /// 绑定患者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择绑定患者！");
                return;
            }
            _patient = (PatientModel)this.dataGrid.SelectedItem;
            ImageEditor.SavePdfFile();
            this.Close();
        }
        /// <summary>
        /// 查询患者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void query_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) && string.IsNullOrEmpty(txtAppNo.Text) && string.IsNullOrEmpty(txtNo.Text))
            {
                MessageBox.Show("请输入查询参数！");
                return;
            }
            try
            {
                var patients = HttpUploadRequest.GetPatients(txtName.Text, txtAppNo.Text, txtNo.Text);
                this.dataGrid.ItemsSource = patients;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }
        /// <summary>
        /// 序号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        /// <summary>
        /// 切换账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectAccount form = new SelectAccount();
            form.Width = 300;
            form.Height = 400;
            form.ShowDialog();
            _user = HttpUploadRequest.GetLoginUser();
            txtAccount.Text = _user == null ? "" : _user.AccountName;
        }
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            this.txtNo.Text = "";
            this.txtAppNo.Text = "";
            this.txtName.Text = "";
            this.dataGrid.ItemsSource = null;
        }
        /// <summary>
        /// 双击选中患者行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("请选择绑定患者！");
                return;
            }
            _patient = (PatientModel)this.dataGrid.SelectedItem;
            ImageEditor.SavePdfFile();
            this.Close();
        }
    }
}
