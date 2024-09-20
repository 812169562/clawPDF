using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Utilities
{
    public static class StringUtil
    {
        /// <summary>
        /// 判断是否为控
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 判断是否为控
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
