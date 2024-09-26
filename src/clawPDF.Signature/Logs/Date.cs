using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clawPDF.Signature
{
    /// <summary>
    /// 日期处理事件
    /// </summary>
    public static class Date
    {
        private static object _object;


        static Date()
        {
            _object = new object();
        }

        public static DateTime Now => DateTime.Now;
        /// <summary>
        /// yyyyMMdd
        /// </summary>
        public static string yyyyMMdd => Now.ToString("yyyyMMdd");
        public static string yyyyMM01 => Now.ToString("yyyyMM01");
        public static string yyyy_MM_01 => Now.ToString("yyyy_MM_01");

        public static string yyyyMM => Now.ToString("yyyyMM");
        public static string yyyyMMddHH => Now.ToString("yyyyMMddHH");
        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        public static string yyyyMMddHHmmss => Now.ToString("yyyyMMddHHmmss");
        public static string yyMMddHHmmss => Now.ToString("yyMMddHHmmss");

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string yyyy_MMddHHmmss => Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        /// yyyyMMddHHmmssSSS
        /// </summary>
        public static string yyyyMMddHHmmssSSS => Now.ToString("yyyyMMddHHmmssfff");
        public static string yyMMddHHmmssfff => Now.ToString("yyMMddHHmmssfff");

        public static string _yyyyMMddHHmmssSSS => Now.ToString("yyyy年MM月dd日HH时mm分ss秒fffff");

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public static string yyyy_MMdd => Now.ToString("yyyy-MM-dd");
        /// <summary>
        /// yyyy-MM-01
        /// </summary>
        public static string yyyy_MM01 => Now.ToString("yyyy-MM-01");

        private static int _numbver4;
        public static string Number4(int code = 0)
        {
            lock (_object)
            {
                _numbver4++;
                if (code == 0 || code > 10)
                {
                    if (_numbver4 == 9999)
                    {
                        _numbver4 = 0;
                    }
                    return _numbver4.ToString().PadLeft(4, '0');
                }
                return code + _numbver4.ToString().PadLeft(3, '0');
            }
        }
    }
}
