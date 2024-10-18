using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class PatientModel
    {
        /// <summary>
        /// 患者类型 1、门诊 0、住院
        /// </summary>
        public int PatientType { get; set; }
        public string PatientTypeStr { get { return PatientType == 1 ? "门诊" : "住院"; } }
        /// <summary>
        /// 住院次数
        /// </summary>
        public string InpatientNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PatientNum { get; set; }
        /// <summary>
        ///  住院次数
        /// <summary>
        public string Times { get; set; }
        /// <summary>
        ///  患者姓名
        /// <summary>
        public string PatientName { get; set; }
        /// <summary>
        ///  性别
        /// <summary>
        public string Sex { get; set; }
        /// <summary>
        ///  年龄
        /// <summary>
        public string Age { get; set; }
        /// <summary>
        ///  检查项目
        /// <summary>
        public string CheckItem { get; set; }
        /// <summary>
        ///  申请单号
        /// <summary>
        public string RequestNum { get; set; }
        /// <summary>
        ///  申请时间
        /// <summary>
        public DateTime RequestTime { get; set; }
        public string RequestTimeStr { get { return RequestTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
        /// <summary>
        ///  类别(检查报告/检验报告等)
        /// <summary>
        public string DocTitle { get; set; }

        /// <summary>
        ///  0-查询，1-绑定
        /// <summary>
        public int BindSource { get; set; }
        /// <summary>
        ///  签名证书
        /// <summary>
        public string Cert { get; set; }
        /// <summary>
        /// 是否签名
        /// </summary>
        public bool IsSign { get; set; }
        /// <summary>
        /// userKey 签章图片
        /// </summary>
        public string UKeySignPicture { get; set; }
        /// <summary>
        /// 报告签名信息
        /// </summary>
        public List<ReportSignature> ReportSignatureReqs { get; set; }
    }
    public class ReportSignature
    {
        /// <summary>
        ///  时间戳原文
        /// <summary>
        public string ReportId { get; set; }
        /// <summary>
        ///  签名原文
        /// <summary>
        public string PlainData { get; set; }
        /// <summary>
        ///  签名值
        /// <summary>
        public string SignData { get; set; }
        /// <summary>
        ///  签名时间戳
        /// <summary>
        public string SignTimestamp { get; set; }
    }
}
