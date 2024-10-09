using clawSoft.clawPDF.Core.Request;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Log = clawPDF.Core.Log;

namespace clawSoft.clawPDF.Core.Views
{
    /// <summary>
    /// BindPatient.xaml 的交互逻辑
    /// </summary>
    public partial class BindPatient : Window
    {
        public string file;
        public static PatientModel _patient;
        public LoginUser _user;
        public SigningProcesModel signingProces;
        public bool isUpload;
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
            InitUser();
            signingProces = HttpUploadRequest.GetSigningProces();
            ok.Content = signingProces.SignatureProcessConfigWay == 2 ? "签名并上传" : "绑定";
        }

        private void InitUser()
        {
            if (_user == null)
            {
                MessageBox.Show("请选择医师账号！", "提示", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }
            //List<KeyValue> keys = new List<KeyValue>
            //{
            //    new KeyValue { Key = "", Value = "全部" },
            //    new KeyValue { Key = _user.HiscaDepartmentId, Value = _user.DepartmentName }
            //};
            //cbbExecDept.ItemsSource = keys;
            //cbbExecDept.DisplayMemberPath = "Value";
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
            isUpload = true;
            this.Close();
        }
        /// <summary>
        /// 绑定患者
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedRow();
        }

        private void SelectedRow()
        {
            try
            {
                if (dataGrid.SelectedItem == null)
                {
                    MessageBox.Show("请选择绑定患者！");
                    return;
                }
                _patient = (PatientModel)this.dataGrid.SelectedItem;
                ImageEditor.SavePdfFile();
                SignLogin();
                isUpload = true;
                this.Close();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 签名登录
        /// </summary>
        private void SignLogin()
        {
            var signbase64 = "";
            // 判断是否配置独立签名
            if (signingProces.SignatureProcessConfigWay == 2)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("绑定成功并上传至后台，请确认是否签名？", "", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                // 1、取消：不签名，直接上传
                if (messageBoxResult == MessageBoxResult.Cancel)
                    return;
                if (_user.SignType == 1)
                {
                    //_user.AccountNo = "656b8a1522a3b358q4ad4w3d18y687682eb";
                    signbase64 = HttpUploadRequest.GetSignImage(_user.AccountNo);
                }
            }
            if (_user.SignType == 2)
            {
                var userCert = HttpSignRequest.GetUserCert();
                // 2、确定：判断当前登录账号是否与绑定的一致？
                if (!userCert.IsLogin || _user.AccountNo.IsEmpty() || _user.AccountNo != userCert.UserCertID)
                {
                    // 2-1、不一致，弹出登录框
                    Cmd.KillApp("clawPDF.Signature");
                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Signature", "clawPDF.Signature.exe");
                    Cmd.StartApp(path, SystemConfig.Setting.SignServer, ProcessWindowStyle.Normal, false);
                    var visible = true;
                    while (visible)
                    {
                        Thread.Sleep(1000);
                        visible = Cmd.AppVisible("clawPDF.Signature");
                        if (!visible)
                        {
                            userCert = HttpSignRequest.GetUserCert();
                            if (!userCert.IsLogin) return;
                            _user.AccountNo = userCert.UserCertID;
                            _user.DoctorInfo = $"{userCert.UserName}||{userCert.CertId}";
                            _user.SignType = 2;
                            HttpUploadRequest.BindSignatureAccount(_user);
                            SystemSetting setting = SystemConfig.Setting;
                            setting.LoginUser = Encrypt.DesEncrypt(JsonConvert.SerializeObject(_user));
                            SystemConfig.Save(setting);
                        }
                    }
                }
                // 账号一致，调用签名
                var bytes = PdfUtil.GetBytes(file, 64, 99);
                var res = HttpSignRequest.SignData(userCert.CertId, Convert.ToBase64String(bytes));
                signbase64 = userCert.PicBase64;
                _patient.Cert = res.CertId;
                _patient.SignData = res.SignValue;
                _patient.PlainData = res.OrgData;
                _patient.PlainTimestampData = res.OrgData;
                _patient.SignTimestamp = res.TimeStamp;
            }
            var sign = HttpUploadRequest.GetSignSetting(_patient.CheckItem);
            PdfUtil.AddBase64Image(file, signbase64, sign.SignPage, sign.XWide, sign.YHigh);
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
#if DEBUG
#else
            form.Topmost = true;
            form.Activate();
#endif

            form.ShowDialog();
            _user = HttpUploadRequest.GetLoginUser();
            InitUser();
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
            //cbbExecDept.SelectedIndex = 0;
            this.dataGrid.ItemsSource = null;
        }
        /// <summary>
        /// 双击选中患者行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedRow();
        }

    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
