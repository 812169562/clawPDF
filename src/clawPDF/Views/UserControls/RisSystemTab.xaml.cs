using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace clawSoft.clawPDF.Views.UserControls
{
    /// <summary>
    /// RisSystemTab.xaml 的交互逻辑
    /// </summary>
    public partial class RisSystemTab : UserControl
    {
        private static readonly TranslationHelper TranslationHelper = TranslationHelper.Instance;
        public RisSystemTab()
        {
            InitializeComponent();
            if (TranslationHelper.IsInitialized) TranslationHelper.TranslatorInstance.Translate(this);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtRisUrl.Text = SystemConfig.Setting.RisUrl;
        }
    }
}
