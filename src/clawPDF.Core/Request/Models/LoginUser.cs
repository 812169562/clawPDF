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
        ///
        /// <summary>
        public string DepartmentID { get { return HiscaDepartmentId; } }

        /// <summary>
        ///账户ID
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
        /// 签名账户类型 1-医网信 2-其他
        /// <summary>
        public int SignType { get; set; }
        /// <summary>
        /// 密码（密文）
        /// </summary>
        public string Password { get; set; }
    }
}
