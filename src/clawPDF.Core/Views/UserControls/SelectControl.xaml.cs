using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace clawSoft.clawPDF.Core.Views.UserControls
{
    /// <summary>
    /// SelectControl.xaml 的交互逻辑
    /// </summary>
    public partial class SelectControl : UserControl
    {
        public SelectControl()
        {
            InitializeComponent();
            init();
        }
        [Category("Behavior")]
        public event Action<object, object> SelectionChanged;
        [Category("Behavior")]
        public event Action<object, TextChangedEventArgs> TextChanged;
        /// <summary>
        /// 绑定数据列
        /// </summary>
        public List<DataGridTextColumn> Columns
        {
            get { return (List<DataGridTextColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(List<DataGridTextColumn>), typeof(SelectControl), new PropertyMetadata(null));
        /// <summary>
        /// 绑定数据源
        /// </summary>
        public System.Collections.IEnumerable ItemsSource
        {
            get { return (System.Collections.IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", typeof(System.Collections.IEnumerable), typeof(SelectControl), new PropertyMetadata(null));
        /// <summary>
        /// 绑定数据源
        /// </summary>
        public ItemCollection Items { get; set; }
        /// <summary>
        /// 输入文本框文字
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 选中的项
        /// </summary>
        public object SelectedItem { get; set; }
        /// <summary>
        /// 输入文本发生改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Pop.IsOpen = true;
            this.Text = txtBox.Text;
            if (TextChanged != null)
                TextChanged(sender, e);
            this.dataGrid.ItemsSource = ItemsSource;
            this.Items = this.dataGrid.Items;
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.ItemsSource = ItemsSource;
            this.Items = this.dataGrid.Items;
            if (Columns != null)
            {
                foreach (var item in Columns)
                {
                    this.dataGrid.Columns.Add(item);
                }
            }
        }

        private void init()
        {
            this.dataGrid.Items.Clear();
            this.SelectedItem = null;
        }
        /// <summary>
        /// 键盘选中行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.SelectedItem = dataGrid.SelectedItem;
                if (SelectionChanged != null)
                    SelectionChanged(sender, dataGrid.SelectedItem);
                txtBox.Text = this.Text;
                this.Pop.IsOpen = false;
            }
            else if (e.Key == Key.Up && dataGrid.Items.Count > 0)
            {
                //上
                if (dataGrid.SelectedIndex - 1 < 0)
                    dataGrid.SelectedIndex = 0;
                else
                {
                    dataGrid.SelectedIndex = dataGrid.SelectedIndex - 1;
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
            else if (e.Key == Key.Down && dataGrid.Items.Count > 0)
            {
                //下
                if (dataGrid.SelectedIndex >= dataGrid.Items.Count - 1)
                {
                    dataGrid.SelectedIndex = dataGrid.Items.Count - 1;
                }
                else
                {
                    dataGrid.SelectedIndex = dataGrid.SelectedIndex + 1;
                    dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                }
            }
        }
        /// <summary>
        /// 鼠标选择行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.SelectedItem = dataGrid.SelectedItem;
            if (SelectionChanged != null)
                SelectionChanged(sender, dataGrid.SelectedItem);
            txtBox.Text = this.Text;
            this.Pop.IsOpen = false;
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
