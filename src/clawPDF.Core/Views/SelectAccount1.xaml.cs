using clawSoft.clawPDF.Core.Request;
using clawSoft.clawPDF.Core.Request.Models;
using clawSoft.clawPDF.Core.Settings;
using clawSoft.clawPDF.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace clawSoft.clawPDF.Core.Views
{
    /// <summary>
    /// SelectAccount.xaml 的交互逻辑
    /// </summary>
    public partial class SelectAccount1 : Window
    {
        public SelectAccount1()
        {
            InitializeComponent();
            this.Loaded += SelectAccount1_Loaded;
        }

        private void SelectAccount1_Loaded(object sender, RoutedEventArgs e)
        {
            ImageEditor.FilePath = "D:\\szyx\\test-pdf\\00001\\a8eccece20ac4f06bf304b56df2cc2bc.pdf";
        }
    }
}
