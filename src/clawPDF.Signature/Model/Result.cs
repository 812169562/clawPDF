using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace clawPDF.Signature
{
    /// <summary>
    /// 返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class Result<T>
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "message")]
        public string Message { get; set; }
        [DataMember(Name = "data")]
        public T Data { get; set; }
    }

    public static class ResultUtil
    {
        public static Result<T> Success<T>(T data)
        {
            return new Result<T>
            {
                Status = "0",
                Data = data,
            };
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>
            {
                Status = "-1",
                Message = message
            };
        }
    }
}
