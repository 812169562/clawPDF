using clawPDF.Core;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Shared.Helper;
using clawSoft.clawPDF.Utilities;
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
        public int _printWay;
        public string _pageSize;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtRisUrl.Text = SystemConfig.Setting.RisUrl;
            List<KeyValue> keys = new List<KeyValue>
            {
                new KeyValue { Key = 1, Value = "印刷体打印" },
                new KeyValue { Key = 2, Value = "PDF打印" }
            };
            cbbPrintWay.ItemsSource = keys;
            if (SystemConfig.Setting.PrintWay <= 0 || !keys.Any(t => t.Key == SystemConfig.Setting.PrintWay))
                cbbPrintWay.SelectedItem = keys[0];
            else
                cbbPrintWay.SelectedItem = keys.FirstOrDefault(t => t.Key == SystemConfig.Setting.PrintWay);

            List<KeyValue> keys1 = new List<KeyValue>
            {
                new KeyValue { Key = 1, Value = "A4", Label = "A4" },
                new KeyValue { Key = 2, Value = "Other", Label = "其他" }
            };
            cbbPageSize.ItemsSource = keys1;
            if (SystemConfig.Setting.PageSize.IsEmpty() || !keys.Any(t => t.Value == SystemConfig.Setting.PageSize))
                cbbPrintWay.SelectedItem = keys[0];
            else
                cbbPrintWay.SelectedItem = keys.FirstOrDefault(t => t.Value == SystemConfig.Setting.PageSize);

            PdfTabVisible.IsChecked = SystemConfig.Setting.PdfTabVisible;
            //OCRTabVisible.IsChecked = SystemConfig.Setting.OCRTabVisible;
            ScriptActionVisible.IsChecked = SystemConfig.Setting.ScriptActionVisible;
            AttachmentActionVisible.IsChecked = SystemConfig.Setting.AttachmentActionVisible;
            BackgroundActionVisible.IsChecked = SystemConfig.Setting.BackgroundActionVisible;
            CoverActionVisible.IsChecked = SystemConfig.Setting.CoverActionVisible;
            EmailClientActionVisible.IsChecked = SystemConfig.Setting.EmailClientActionVisible;
            EmailSmtpActionVisible.IsChecked = SystemConfig.Setting.EmailSmtpActionVisible;
            FtpActionVisible.IsChecked = SystemConfig.Setting.FtpActionVisible;
            txtTextTemplate.Text = SystemConfig.Setting.TextTemplate;
            txtDPI.Text = (SystemConfig.Setting.Dpi ?? 150).ToString();
        }

        private void cbbPrintWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbPrintWay.SelectedItem == null)
                _printWay = 1;
            else
                _printWay = ((KeyValue)cbbPrintWay.SelectedItem).Key;
        }

        private void cbbPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbPrintWay.SelectedItem == null)
                _pageSize = "A4";
            else
                _pageSize = ((KeyValue)cbbPrintWay.SelectedItem).Value;
        }
    }
}

public class KeyValue
{
    public int Key { get; set; }

    public string Value { get; set; }
    public string Label { get; set; }
}
