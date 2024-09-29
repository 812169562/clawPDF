using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Request.Models
{
    public class SignConfigModel
    {
        public SignConfigModel()
        {
            SignPage = 3;
            XWide = 504;
            YHigh = 787;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /**
         * 报告类型（中文全匹配）
         */
        public string ReportType { get; set; }

        /**
         * 纸张尺寸(A4、A5)
         */
        public string PaperType { get; set; }

        /**
         * x位置坐标
         */
        public int XWide { get; set; }

        /**
         * y位置坐标
         */
        public int YHigh { get; set; }
        /**
         * 第二个人x位置坐标
         */
        public int XWideSecond { get; set; }

        /**
         * 第二个人y位置坐标
         */
        public int YHighSecond { get; set; }

        /**
         * 签名页码
         */
        public int SignPage { get; set; }

        /**
         * 状态:  0：初始 ;1：绑定；2：删除
         */
        public string Status { get; set; }

        /**
         * 设备id
         */
        public string EquipmentId { get; set; }

        /**
         * mac地址
         */
        public string MacAddress { get; set; }

        /**
         * 0：横向 1：纵向
         */
        public string PaperDirection { get; set; }

        /**
         * 0：默认 1：非默认
         */
        public string DefaultFlag { get; set; }

        /**
         * 创建时间
         */
        public DateTime Ctime { get; set; }

        /**
         * 更新时间
         */
        public DateTime Utime { get; set; }
    }
}
