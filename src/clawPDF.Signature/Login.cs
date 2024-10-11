using clawSoft.clawPDF.Core.Settings;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace clawPDF.Signature
{
    public partial class Login : Form
    {
        public static string strCertId;//certId
        public static string strUserCert;//用户证书
        public static string strUserCertID;//用户证书Id
        public static string strUserName;//用户证书名称
        public static string strPicBase64;// = "R0lGODlhWwA3ANUAAAAAAP///wUGBhgZGRcaGB8jIAABAAECAQIDAgYIBgMEAwQFBAUGBRIVEhoeGg0PDQ4QDgcIBw8RDwgJCBodGgkKCQoLChUXFQwNDBweHA4PDhAREBITEhoeGR4iHSEkIBocGRsdGgkKCAEBAAICAQMDAgUFBAkJCBgYGAYGBgUFBQQEBAMDAwICAgEBAf///wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAEAAC8ALAAAAABbADcAAAb/wJdwSCwaj8ikcslsOp/QaKuFdAGu0aL1yu0CVtnsyusttq4KQFi4BbimrTZ33SS7wGeW2oilL/VXLn5ICG5JaHx7g0tuiotPfUSRj0mAlJCOL2OCl4yZnUeTL5ErpaBFKqKnko5XK2dkKmGqRbSro4pfGVYuCK9ynJBxnrdsLEKRXSYHBlRDZwYAznUJn7XWl322uEZ6Vk4t0cSnsi5vyNjcoemJCLJV5rPYWwpj6IfWACbsQoAYHVumEUkTDMqXKweGgBGSqoK9barGICgYileLEwZSndMC4JiTNhEmLHiwxYVHNi0GqNnGohkRKyT4qYuzR9W2ImPINCAAAQAD/yPm7LEYgQCJpRfReDkJxM2cwBcs4jUp8QkAh29Gcg6x9SYNF4pMYDkAkEJUCaxOFqTQ4kIF2q0b1YUSETfMGwqTNMo8sqBWoVYAUBwYc+wtx4V+AES4QEaanxXvsBhCx6IAVj19jzC42STOBzUsWqy4s4hBvG+iOrYCu3UvPgAhcnU6wyBqqkSs8K05mow1nQUANhzN3Vp3lNGk7wk5c+rKu2tEaFdx3c8NYnSQQ1PHdKhWQiQASDTJOe3VFQkeuvj2w3xpOs4vnGpqrAFE8mJK1hYHz84KCwuwBHISABPhF4VhxBlxRgsHdPEUMnUZiARimymAxBnPRYeQCUuM0KGLhEu4Bdch6zkRVYkgjsIJRNvxl6ISGsmF2yDwgeiGaO+1uM6LjLggQDp50KhjMXOQKCSPTLBw0o6JDYlkgmvU+KRxdEg55REIRmHllah01CSXUY7ghxWZgWmQk8t9YaaWaL5wxnVrMrFlNybF+cRESx43mZ11oJlKm1POScSffLpHB6GFhgVAhlAAB2igTpb16JOC7pfoOJcWo0emBiIWBAA7";
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public static bool IsLogin = false;
        public Login()
        {
            InitializeComponent();
        }

        private string oid = "2.16.840.1.113732.2";//RSA 对象唯一标示符 SM2 1.2.156.112562.2.1.1.1
        private void Login_Load(object sender, EventArgs e)
        {
            GetUserList();
        }
        /// <summary>
        /// 获取用户列表  
        /// </summary>
        private void GetUserList()
        {
            try
            {
                string CertID = axXTXApp1.SOF_GetUserList();
                if (string.IsNullOrEmpty(CertID))
                {
                    msg.Text = "提示：请插入实体key，并完成登录";
                    return;
                }
                // CertID返回     王晓东||102080000879376/011712001357&&&
                string[] sArray = CertID.Split(new string[] { "&&&" }, StringSplitOptions.RemoveEmptyEntries);
                if (sArray.Length <= 0)
                    return;
                List<User> users = new List<User>();
                for (int i = 0; i < sArray.Length; i++)
                {
                    var arry = sArray[i].Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arry.Length <= 0) continue;
                    users.Add(new User() { CertId = arry[1], Username = arry[0] });
                }
                cbxUsername.Items.Clear();
                cbxUsername.DataSource = users;
                cbxUsername.DisplayMember = "Username";
                cbxUsername.ValueMember = "CertId";
            }
            catch (Exception ex)
            {
                msg.Text = "提示：请检查证书环境";
                Log.SignError($"初始化插件：{ex.Message}");
            }
        }
        /// <summary>
        /// UKey设备插拔事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axXTXApp1_OnUsbkeyChange(object sender, EventArgs e)
        {
            strCertId = "";
            strUserCert = "";
            strUserCertID = "";
            strUserName = "";
            strPicBase64 = "";
            IsLogin = false;
            GetUserList();
        }
        /// <summary>
        /// 账号发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxUsername_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxUsername.SelectedIndex == -1) return;

            strCertId = ((User)cbxUsername.SelectedItem).CertId;
            strUserName = ((User)cbxUsername.SelectedItem).Username;
            strUserCert = axXTXApp1.SOF_ExportUserCert(strCertId);//导出用户证书
            var certType = axXTXApp1.SOF_GetCertInfo(strUserCert, 3); // 证书类型 返回"RSA"或"SM2"
            oid = certType == "SM2" ? "1.2.156.112562.2.1.1.1" : "2.16.840.1.113732.2";
            strUserCertID = axXTXApp1.SOF_GetCertInfoByOid(strUserCert, oid); //注意oid的加密方式要对应才行
            if (!string.IsNullOrEmpty(SystemConfig.LoginUser?.UserCertID) && SystemConfig.LoginUser?.UserCertID != strUserCertID)
            {
                msg.Text = "提示：插入的实体key与单机账号不匹配！";
            }
            else
            {
                msg.Text = "";
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text) || txtPassword.Text.Length < 6 || txtPassword.Text.Length > 16)
            {
                //msg.Text = "提示：密码不能为空";
                msg.Text = "提示：请输入6-16位密码";
                return;
            }
            //获取随机数  随机数签名值 服务器证书
            BJCA_SVS_CLIENTCOMLib.BJCASVSEngine svs = new BJCA_SVS_CLIENTCOMLib.BJCASVSEngine();
            XTXAppCOMLib.XTXAppClass xtx = new XTXAppCOMLib.XTXAppClass();

            int nRandom = 16;
            string Random_num = svs.GenRandom(nRandom);            //返回的随机数是str 类型
            string ServerCertificate = svs.GetServerCertificate(); // 获取服务器证书
            string Sigdata = svs.SignData(Random_num);             //将随机数签名

            /*客户端验证服务器证书以及验证服务器签名*/
            //客户端验证服务器证书
            bool blRet = xtx.SOF_VerifySignedData(ServerCertificate, Random_num, Sigdata);
            if (!blRet)
            {
                msg.Text = "提示：客户端验证服务器签名失败";
                return;
            }
            // 验证密码是否通过
            string PassWd = txtPassword.Text;
            //校验口令 第一个参数是密码卡/介质序号certid
            bool isLogin = xtx.SOF_Login(strCertId, PassWd);
            if (!isLogin)
            {
                int errorTimes = xtx.SOF_GetPinRetryCount(strCertId);//获取容错次数
                //int errorTimes = xtx.SOF_GetRetryCount(strCertId);//获取容错次数
                if (errorTimes < 0)
                {
                    msg.Text = "提示：获取重试次数失败";
                }
                else if (errorTimes == 0)
                {
                    msg.Text = "提示：您的证书已经锁死，请联系北京CA进行解锁.";
                }
                else
                {
                    msg.Text = $"提示：账号密码不正确，您还有{errorTimes}次机会";
                }
                return;
            }
            string strSingnValue = xtx.SOF_SignData(strCertId, Random_num);
            int ret = svs.VerifySignedData(strUserCert, Random_num, strSingnValue); //用户证书 随机值 签名值
            if (ret != 0)
            {
                msg.Text = "提示：服务器验证签名失败";
                return;
            }
            int retV_cert = svs.ValidateCertificate(strUserCert);//验证客户端证书有效性操作

            switch (retV_cert)
            {
                case 0: break;
                case -1:
                    msg.Text = "提示：不是所信任的根";
                    return;
                case -2:
                    msg.Text = "提示：超过有效期，请更换证书";
                    return;
                case -3:
                    msg.Text = "提示：作废证书";
                    return;
                case -4:
                    msg.Text = "提示：证书已加入黑名单";
                    return;
                case -5:
                    msg.Text = "提示：证书未生效";
                    return;
                default: return;
            }
            //创建获取签章图片对象
            GETKEYPICLib.GetPic pic = new GETKEYPICLib.GetPic();
            //获取签章图片信息
            strPicBase64 = pic.GetPic(strCertId);
            if (string.IsNullOrEmpty(strPicBase64))
            {
                msg.Text = "提示：获取签章图片失败";
                return;
            }
            IsLogin = true;
            this.Hide();
            MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsLogin = false;
            this.Hide();
        }
        /// <summary>
        /// 获取设备数量
        /// </summary>
        /// <returns></returns>
        private int GetDeviceCount()
        {
            //var a = axXTXApp1.GetDeviceSNByIndex(0);
            //var b = axXTXApp1.IsDeviceExist(a);
            return axXTXApp1.GetDeviceCount();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// CertID
        /// </summary>
        public string CertId { get; set; }
    }
}
