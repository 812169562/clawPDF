using clawSoft.clawPDF.Core.Settings;
using DrawTools.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawTools.Views
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEditor : UserControl
    {
        public string FilePath { get; set; }

        private ConcurrentDictionary<string, List<Visual>> VisualList;
        private List<string> images = new List<string>();
        private int currentIndex = 0;
        private string currentFilePath;
        private string file;//= "D:\\szyx\\test-pdf\\00001\\a8eccece20ac4f06bf304b56df2cc2bc.pdf";
        public ImageEditor()
        {
            VisualList = new ConcurrentDictionary<string, List<Visual>>();

            InitializeComponent();
            this.PreviewMouseMove += MainWindow_PreviewMouseMove;
            this.Loaded += MainWindow_Loaded;
            this.PreviewKeyDown += ImageEditor_PreviewKeyDown;
            this.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(MainWindow_MouseDown), true);

            color_picker.SelectedColorChanged += delegate { this.drawCanvas.Brush = color_picker.SelectedBrush; btn_color.IsChecked = false; };
            color_picker.Canceled += delegate { btn_color.IsChecked = false; };

            this.toolbar.AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(OnDrawToolChecked));
            drawCanvas.FontSize = SystemConfig.Setting.FontSize > 14 ? SystemConfig.Setting.FontSize : 14;
        }

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ModifierKeys key = e.KeyboardDevice.Modifiers & ModifierKeys.Control;
            if (key == ModifierKeys.Control && e.Key == Key.Z)
            {
                Undo_Click(sender, null);
            }
            if (key == ModifierKeys.Control && e.Key == Key.S)
            {
                btn_save_Click(sender, null);
            }
            if (e.Key == Key.Up)
            {
                before_Click(sender, null);
            }
            if (e.Key == Key.Down)
            {
                after_Click(sender, null);
            }
        }

        /// <summary>
        /// 鼠标点击空白区域默认选择拾取操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.drawCanvas.IsMouseOver && !this.toolbar.IsMouseOver)
            {
                drawCanvas.DrawingToolType = DrawToolType.Pointer;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            file = FilePath;
            if (!string.IsNullOrEmpty(SystemConfig.Setting.TextTemplate))
            {
                var list = SystemConfig.Setting.TextTemplate.Split(',').ToList();
                list.Insert(0, "选择模板");
                cbxTextTemplate.ItemsSource = list;
                cbxTextTemplate.SelectedIndex = 0;
            }

            if (string.IsNullOrEmpty(file)) return;
            ImageHelper.ToImages(file);

            var directory = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
            var files = Directory.GetFiles(directory);
            images = files.Where(t => !t.Contains("_update")).ToList();
            this.totalPages.Text = "/ " + images.Count();
            currentIndex = images.Count() - 1;
            LoadImage();
        }
        private void LoadImage()
        {
            if (images.Count() <= 0 || currentIndex >= images.Count()) return;

            // 切换时自动保存
            if (drawCanvas != null)
            {
                SaveFile();
                drawCanvas.Clear();
            }
            // 加载页面图片
            this.currentFilePath = images[currentIndex];
            this.drawViewer.BackgroundImage = new BitmapImage(new Uri(currentFilePath));
            this.currentPage.Text = (currentIndex + 1).ToString();

            if (VisualList == null || !VisualList.Any(t => t.Key == currentFilePath)) return;
            var visuals = VisualList[currentFilePath];
            foreach (var item in visuals)
            {
                //this.drawCanvas.AddWorkingDrawTool(item as IDrawTool);
                this.drawCanvas.AddVisual(item);
                //this.drawCanvas.DeleteWorkingDrawTool(item as IDrawTool);
            }
            var data = new List<Visual>();
            this.VisualList.TryRemove(currentFilePath, out data);
            //var bitmapImage = new BitmapImage();
            //bitmapImage.BeginInit();
            //bitmapImage.UriSource = new Uri(currentFilePath, UriKind.RelativeOrAbsolute);
            //bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //bitmapImage.EndInit();
            //this.drawViewer.BackgroundImage = bitmapImage.Clone();
            //this.drawViewer.InvalidateVisual();
            //UpdateImage();
        }

        //private void UpdateImage()
        //{
        //    // 替换原图
        //    var directory = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
        //    var files = Directory.GetFiles(directory);
        //    var currentFileName = Path.GetFileNameWithoutExtension(currentFilePath);
        //    foreach (var item in files)
        //    {
        //        var fileName = Path.GetFileNameWithoutExtension(item);
        //        if (!item.Contains("_update")) continue;
        //        if (fileName.Contains(currentFileName)) continue;

        //        try
        //        {
        //            var original = item.Replace("_update", "");
        //            var originalFilePath = Path.Combine(directory, fileName.Replace("_update", "") + "_original" + Path.GetExtension(item));
        //            if (!File.Exists(originalFilePath))
        //                File.Move(original, originalFilePath);
        //            if (File.Exists(original))
        //                File.Delete(original);
        //            File.Move(item, original);
        //            //File.Copy(item, original);
        //        }
        //        catch (Exception ex)
        //        {
        //        }

        //    }
        //}
        /// <summary>
        /// 鼠标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.btn_redo.IsEnabled = drawCanvas.RedoEnabled;
            this.btn_redo.Opacity = drawCanvas.RedoEnabled ? 1 : 0.7;

            this.btn_undo.IsEnabled = drawCanvas.UndoEnabled;
            this.btn_undo.Opacity = drawCanvas.UndoEnabled ? 1 : 0.7;

            if (drawCanvas.DrawingToolType == DrawToolType.Pointer)
                this.Pointer.IsChecked = true;

        }
        /// <summary>
        /// 选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDrawToolChecked(Object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton btn && btn.Tag is String typeStr)
                drawCanvas.DrawingToolType = (DrawToolType)Enum.Parse(typeof(DrawToolType), typeStr);
        }
        /// <summary>
        /// 清楚图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            drawCanvas.Clear();
        }

        #region 文件操作（暂未使用）
        private void OnSaveClick(Object sender, RoutedEventArgs e)
        {
            LoadImage();
            if (this.drawCanvas.GetDrawGeometries().Count() == 0)
                return;

            var folder = Path.Combine(Environment.CurrentDirectory, "Draws");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory("Draws");

            var dlg = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                OverwritePrompt = true,
                DefaultExt = "xml",
                InitialDirectory = folder,
                RestoreDirectory = true
            };

            if ((Boolean)dlg.ShowDialog())
                this.drawCanvas.Save(dlg.FileName);
        }

        private void OnOpenClick_1(Object sender, RoutedEventArgs e)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, "Draws");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory("Draws");

            var dlg = new OpenFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                DefaultExt = "xml",
                InitialDirectory = folder,
                RestoreDirectory = true
            };

            if ((Boolean)dlg.ShowDialog())
                this.drawCanvas.Load(dlg.FileName);
        }

        private void OnPrintClick(Object sender, RoutedEventArgs e)
        {
            var backgroundImage = this.drawViewer.BackgroundImage;

            this.drawCanvas.Print(backgroundImage.PixelWidth, backgroundImage.PixelHeight, DpiHelper.GetDpiFromVisual(this.drawCanvas), backgroundImage);
        }

        private void OnSaveImageClick(Object sender, RoutedEventArgs e)
        {
            var backgroundImage = this.drawViewer.BackgroundImage;

            var frame = this.drawCanvas.ToBitmapFrame(backgroundImage.PixelWidth, backgroundImage.PixelHeight, DpiHelper.GetDpiFromVisual(this.drawCanvas), backgroundImage);

            if (frame == null)
                return;

            var folder = Path.Combine(Environment.CurrentDirectory, "Images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory("Images");

            var dlg = new SaveFileDialog
            {
                Filter = "Images files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                OverwritePrompt = true,
                DefaultExt = "jpg",
                InitialDirectory = folder,
                RestoreDirectory = true
            };

            if ((Boolean)dlg.ShowDialog())
                ImageHelper.Save(dlg.FileName, frame);
        }
        #endregion

        /// <summary>
        /// 撤销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            this.drawCanvas.Undo();
        }
        /// <summary>
        /// 重做
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Redo_Click_1(object sender, RoutedEventArgs e)
        {
            this.drawCanvas.Redo();
        }
        /// <summary>
        /// 页码改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            var page = 1;
            try
            {
                page = Convert.ToInt32(this.currentPage.Text);
            }
            catch (Exception)
            {
            }
            if (page > images.Count()) page = 1;
            currentIndex = page - 1;
            LoadImage();
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void first_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = 0;
            LoadImage();
        }
        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void before_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == 0) return;
            currentIndex--;
            LoadImage();
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void after_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == images.Count() - 1) return;
            currentIndex++;
            LoadImage();
        }
        /// <summary>
        /// 最后一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void last_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = images.Count() - 1;
            LoadImage();
        }
        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFile();
                MessageBox.Show("文件保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件保存失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 保存image
        /// </summary>
        private void SaveFile()
        {
            var backgroundImage = this.drawViewer.BackgroundImage;
            var frame = this.drawCanvas.ToBitmapFrame(backgroundImage);
            if (frame == null || string.IsNullOrEmpty(currentFilePath))
                return;

            var fileName = Path.Combine(Path.GetDirectoryName(currentFilePath), Path.GetFileNameWithoutExtension(currentFilePath) + "_update" + Path.GetExtension(currentFilePath));
            ImageHelper.Save(fileName, frame);

            var visuals = this.drawCanvas.GetVisuals();
            var list = new List<Visual>(visuals);
            if (visuals != null && visuals.Any())
                VisualList.TryAdd(currentFilePath, list);
        }
        /// <summary>
        /// 绑定患者保存并生成pdf
        /// </summary>
        public void SavePdfFile()
        {
            try
            {
                var backgroundImage = this.drawViewer.BackgroundImage;
                var frame = this.drawCanvas.ToBitmapFrame(backgroundImage);

                if (frame != null && !string.IsNullOrEmpty(currentFilePath))
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(currentFilePath), Path.GetFileNameWithoutExtension(currentFilePath) + "_update" + Path.GetExtension(currentFilePath));
                    ImageHelper.Save(fileName, frame);
                }
                ImageToPdf();
            }
            catch (Exception ex)
            {
                Log.Error("绑定患者自动保存文件失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 合并图片生成pdf
        /// </summary>
        public void ImageToPdf()
        {
            var directory = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
            var files = Directory.GetFiles(directory);
            List<string> list = files.ToList();
            var update = list.Where(t => t.Contains("_update")).ToList();
            foreach (var item in update)
            {
                var name = item.Replace("_update", "");
                list.Remove(name);
            }
            ImageHelper.ToPDF(list, file);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxTextTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbxTextTemplate.SelectedIndex > 0)
            {
                this.drawCanvas.AddTextDrawTool(this.cbxTextTemplate.SelectedItem.ToString());
                this.cbxTextTemplate.SelectedIndex = 0;
            }
        }
        private void font_size_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e)
        {
            SystemSetting setting = SystemConfig.Setting;
            setting.FontSize = font_size.Value;
            SystemConfig.Save(setting);
        }
    }
}
