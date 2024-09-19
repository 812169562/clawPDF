namespace clawPDF.Signature
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.MaskedTextBox();
            this.cbxUsername = new System.Windows.Forms.ComboBox();
            this.axXTXApp1 = new AxXTXAppCOMLib.AxXTXApp();
            ((System.ComponentModel.ISupportInitialize)(this.axXTXApp1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "账号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(82, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码：";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(106, 182);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 34);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(267, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 34);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(142, 107);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new System.Drawing.Size(227, 25);
            this.txtPassword.TabIndex = 6;
            // 
            // cbxUsername
            // 
            this.cbxUsername.FormattingEnabled = true;
            this.cbxUsername.Location = new System.Drawing.Point(141, 57);
            this.cbxUsername.Name = "cbxUsername";
            this.cbxUsername.Size = new System.Drawing.Size(228, 23);
            this.cbxUsername.TabIndex = 8;
            this.cbxUsername.SelectedIndexChanged += new System.EventHandler(this.cbxUsername_SelectedIndexChanged);
            // 
            // axXTXApp1
            // 
            this.axXTXApp1.Enabled = true;
            this.axXTXApp1.Location = new System.Drawing.Point(1, 0);
            this.axXTXApp1.Name = "axXTXApp1";
            this.axXTXApp1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axXTXApp1.OcxState")));
            this.axXTXApp1.Size = new System.Drawing.Size(240, 240);
            this.axXTXApp1.TabIndex = 7;
            this.axXTXApp1.Visible = false;
            this.axXTXApp1.OnUsbkeyChange += new System.EventHandler(this.axXTXApp1_OnUsbkeyChange);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 236);
            this.Controls.Add(this.cbxUsername);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.axXTXApp1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数字认证签名账号登录";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axXTXApp1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.MaskedTextBox txtPassword;
        private System.Windows.Forms.ComboBox cbxUsername;
        private AxXTXAppCOMLib.AxXTXApp axXTXApp1;
    }
}