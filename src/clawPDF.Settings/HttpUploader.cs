using pdfforge.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clawSoft.clawPDF.Core.Settings
{
    public class HttpUploader
    {
        private string _httpUploadUrl;
        public bool Enabled { get; set; }

        public HttpUploader()
        {
            Init();
        }

        public string HttpUploadUrl
        {
            get
            {
                try
                {
                    return _httpUploadUrl;
                }
                catch
                {
                    return "";
                }
            }
            set => _httpUploadUrl = value;
        }

        private void Init()
        {
            Enabled = false;
            HttpUploadUrl = "";
        }
        public void ReadValues(Data data, string path)
        {
            try
            {
                Enabled = bool.Parse(data.GetValue(@"" + path + @"Enabled"));
            }
            catch
            {
                Enabled = false;
            }
            try
            {
                HttpUploadUrl = Data.UnescapeString(data.GetValue(@"" + path + @"HttpUploadUrl"));
            }
            catch
            {
                HttpUploadUrl = "";
            }
        }
        public void StoreValues(Data data, string path)
        {
            // 计算机\HKEY_USERS\S-1-5-21-482228137-844348924-615308013-500\Software\clawSoft\clawPDF\Settings\ConversionProfiles\0\HttpUploader
            //System.IO.File.WriteAllText("D:\\asdfg.txt", Data.EscapeString(HttpUploadUrl));
            data.SetValue(@"" + path + @"Enabled", Enabled.ToString());
            data.SetValue(@"" + path + @"HttpUploadUrl", Data.EscapeString(HttpUploadUrl));
        }

        public HttpUploader Copy()
        {
            var copy = new HttpUploader();

            copy.HttpUploadUrl = HttpUploadUrl;
            copy.Enabled = Enabled;

            return copy;
        }
        public override bool Equals(object o)
        {
            if (!(o is HttpUploader)) return false;
            var v = o as HttpUploader;

            if (!HttpUploadUrl.Equals(v.HttpUploadUrl)) return false;
            if (!Enabled.Equals(v.Enabled)) return false;

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("HttpUploadUrl=" + HttpUploadUrl);
            sb.AppendLine("Enabled=" + Enabled);

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
