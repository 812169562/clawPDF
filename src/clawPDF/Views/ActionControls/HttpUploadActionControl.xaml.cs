using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace clawSoft.clawPDF.Shared.Views.ActionControls
{
    /// <summary>
    /// HttpUploadActionControl.xaml 的交互逻辑
    /// </summary>
    public partial class HttpUploadActionControl : ActionControl
    {
        public HttpUploadActionControl()
        {
            InitializeComponent();
            DisplayName = "上传地址";
            Description = "upload pdf to Http Server when pdf was created.";
        }
        public override bool IsActionEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.HttpUploader.Enabled;
            }
            set => CurrentProfile.HttpUploader.Enabled = value;
        }
    }
}
