using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace clawSoft.clawPDF.Core.Settings
{
    public static class SystemConfig
    {
        public static SystemSetting _settings;
        public static string _file;
        public static string _file2;
        public static string lincense;
        static SystemConfig()
        {
            _settings = new SystemSetting();
            _file = System.Windows.Forms.Application.StartupPath + "\\setting.json";
            _file2 = System.Windows.Forms.Application.StartupPath + "\\setting-back.json";
            lincense = Path.Combine(System.Windows.Forms.Application.StartupPath, "ris.lincense");
            Load();
        }
        public static SystemSetting Setting => GetSetting();
        public static SystemSetting GetSetting()
        {
            _settings = JsonConvert.DeserializeObject<SystemSetting>(File.ReadAllText(_file, Encoding.UTF8));
            return _settings;
        }
        public static string Load()
        {
            try
            {
                if (!File.Exists(_file))
                {
                    File.Create(_file).Close();
                    File.WriteAllText(_file, JsonConvert.SerializeObject(_settings), Encoding.UTF8);
                }
                return File.ReadAllText(_file, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "配置文件错误，请修改或者删除setting.json");
            }
        }
        public static void Save(SystemSetting setting)
        {
            var str = JsonConvert.SerializeObject(setting);
            if (!File.Exists(_file2))
            {
                File.Copy(_file, _file2);
            }
            File.Delete(_file);
            File.WriteAllText(_file, str, Encoding.UTF8);
            _settings = setting;
        }
        public static int AuthorizationStatus => Lincense();
        public static int Lincense()
        {
            if (!File.Exists(lincense))
                return 0;
            var key2 = Encrypt.DesEncryptMD5();
            var key = File.ReadAllText(lincense);
            var desKey = Encrypt.DesDecrypt(key);
            var keys = desKey.Split(',');
            if (keys.Length == 2)
            {
                var date = Convert.ToDateTime(keys[1]);
                if (date < DateTime.Today)
                    return 2;
                key2 = Encrypt.DesEncryptMD5(keys[1]);
            }

            if (key != key2)
                return 2;
            return 1;
        }
    }
}
