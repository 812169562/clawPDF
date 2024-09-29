using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class LoginUser
    {
        /// <summary>
        ///主键ID
        /// <summary>
        public int UniqueId { get; set; }

        /// <summary>
        ///科室表主键ID
        /// <summary>
        public string HiscaDepartmentId { get; set; }
        /// <summary>
        ///科室名称
        /// <summary>
        public string DepartmentName { get; set; }

        /// <summary>
        ///关联电子签名账户号 医网信openid ，北京ca ukey 账户唯一值
        /// <summary>
        public string AccountNo { get; set; }

        /// <summary>
        ///账号名称
        /// <summary>
        public string AccountName { get; set; }

        /// <summary>
        ///手机号
        /// <summary>
        public string Phone { get; set; }

        /// <summary>
        ///账户类型(1-个人账号)
        /// <summary>
        public int Type { get; set; }

        /// <summary>
        ///创建时间
        /// <summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 签名账户类型 1-医网签 2-北京ca
        /// <summary>
        public int SignType { get; set; }
        /// <summary>
        /// 密码（密文）
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 电子签名账户名称
        /// </summary>
        public string DoctorInfo { get; set; }
    }
}
