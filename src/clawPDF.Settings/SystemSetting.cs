using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Settings
{
    public class SystemSetting
    {
        /// <summary>
        /// 单机系统地址
        /// </summary>
        public string RisUrl { get; set; }
        /// <summary>
        /// 登录用户信息
        /// </summary>
        public string LoginUser { get; set; }
        /// <summary>
        /// 打印方式：1、印刷体打印 2、pdf文件打印
        /// </summary>
        public int PrintWay { get; set; }
        /// <summary>
        /// Pdf配置页是否可见
        /// </summary>
        public bool PdfTabVisible { get; set; }
        /// <summary>
        /// OCR配置页是否可见
        /// </summary>
        public bool OCRTabVisible { get; set; }
        /// <summary>
        /// 加入背景 (仅PDF)
        /// </summary>
        public bool BackgroundActionVisible { get; set; }
        /// <summary>
        /// 加入封面
        /// </summary>
        public bool CoverActionVisible { get; set; }
        /// <summary>
        /// 加入附件
        /// </summary>
        public bool AttachmentActionVisible { get; set; }
        /// <summary>
        /// 开启电子邮件客户端
        /// </summary>
        public bool EmailClientActionVisible { get; set; }
        /// <summary>
        /// 通过SMTP发送电子邮件
        /// </summary>
        public bool EmailSmtpActionVisible { get; set; }
        /// <summary>
        /// 执行脚本
        /// </summary>
        public bool ScriptActionVisible { get; set; }
        /// <summary>
        /// 使用FTP上传
        /// </summary>
        public bool FtpActionVisible { get; set; }
    }
}
