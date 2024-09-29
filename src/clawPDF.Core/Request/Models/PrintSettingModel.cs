using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class PrintSettingModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 设备id
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 虚拟打印流程: 1-打印成功后立即同步 2-打印后先绑定再同步
        /// </summary>
        public int PrintProcess { get; set; }
    }
}
