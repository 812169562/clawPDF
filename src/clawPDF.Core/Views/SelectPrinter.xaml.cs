using System.Drawing.Printing;
using System.Linq;
using System.Windows;

namespace clawSoft.clawPDF.Core.Views
{
    /// <summary>
    /// SelectPrinter.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPrinter : Window
    {
        public SelectPrinter()
        {
            InitializeComponent();
            this.Topmost = true;
        }
        public string PrintName;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var printers = PrinterUtil.Get();
            cmbPrinter.ItemsSource = printers;
            cmbPrinter.SelectedItem = printers.FirstOrDefault(t => t.Default);

        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPrinter.SelectedItem == null)
            {
                MessageBox.Show("请选择打印机！");
                return;
            }
            PrintName = ((PrinterInfo)cmbPrinter.SelectedItem).Name;
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            PrintName = null;
            this.Close();
        }

        private void cmbPrinter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbPrinter.SelectedItem != null)
            {
                var info = (PrinterInfo)cmbPrinter.SelectedItem;
                labStatus.Text = info.State;
            }
            else
            {
                labStatus.Text = "";
            }
        }
    }
}
