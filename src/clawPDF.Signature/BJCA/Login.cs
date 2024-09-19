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
        public static string strPicBase64;
        public Login()
        {
            InitializeComponent();
        }

        private string oid = "2.16.840.1.113732.2";//RSA 对象唯一标示符 SM2 1.2.156.112562.2.1.1.1
        //创建获取签章图片对象
        private GETKEYPICLib.GetPic pic = new GETKEYPICLib.GetPic();

        private void Login_Load(object sender, EventArgs e)
        {
            GetUserList();
        }
        /// <summary>
        /// 获取用户列表  
        /// </summary>
        private void GetUserList()
        {
            string CertID = axXTXApp1.SOF_GetUserList();
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
        /// <summary>
        /// UKey设备插拔事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axXTXApp1_OnUsbkeyChange(object sender, EventArgs e)
        {
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
            strUserCert = axXTXApp1.SOF_ExportUserCert(strCertId);//导出用户证书
            var certType = axXTXApp1.SOF_GetCertInfo(strUserCert, 3); // 证书类型 返回"RSA"或"SM2"
            if (certType == "SM2")
                oid = "1.2.156.112562.2.1.1.1";
            strUserCertID = axXTXApp1.SOF_GetCertInfoByOid(strUserCert, oid); //注意oid的加密方式要对应才行
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("请输入密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); //弹出提示框
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
                MessageBox.Show("客户端验证服务器签名失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); //弹出提示框
                return;
            }
            // 验证密码是否通过
            string PassWd = txtPassword.Text;
            //校验口令 第一个参数是密码卡/介质序号certid
            bool isLogin = xtx.SOF_Login(strCertId, PassWd);
            if (!isLogin)
            {
                int errorTimes = xtx.SOF_GetRetryCount(strCertId);//获取容错次数
                if (errorTimes < 0)
                {
                    MessageBox.Show("获取重试次数失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (errorTimes == 0)
                {
                    MessageBox.Show("您的证书已经锁死，请到相关部门进行解锁.", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show($"登录失败，您还有{errorTimes}次机会", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string strSingnValue = xtx.SOF_SignData(strCertId, Random_num);
            int ret = svs.VerifySignedData(strUserCert, Random_num, strSingnValue); //用户证书 随机值 签名值
            if (ret != 0)
            {
                MessageBox.Show("服务器验证签名失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int retV_cert = svs.ValidateCertificate(strUserCert);//验证客户端证书有效性操作

            switch (retV_cert)
            {
                case 0: break;
                case -1: MessageBox.Show("不是所信任的根", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                case -2: MessageBox.Show("超过有效期，请更换证书", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                case -3: MessageBox.Show("证书已作废", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                case -4: MessageBox.Show("证书已加入黑名单", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                case -5: MessageBox.Show("证书未生效", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
                default: return;
            }
            //获取签章图片信息
            strPicBase64 = pic.GetPic(strCertId);
            if (string.IsNullOrEmpty(strPicBase64))
            {
                MessageBox.Show("获取签章图片失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("登录成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
