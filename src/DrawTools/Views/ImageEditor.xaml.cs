using DrawTools.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DrawTools.Views
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEditor : UserControl
    {
        public string FilePath { get; set; }

        private List<string> images = new List<string>();
        private int currentIndex = 0;
        private string currentFilePath;
        private string file;//= "D:\\szyx\\test-pdf\\00001\\a8eccece20ac4f06bf304b56df2cc2bc.pdf";
        public ImageEditor()
        {
            InitializeComponent();
            this.PreviewMouseMove += MainWindow_PreviewMouseMove;
            this.Loaded += MainWindow_Loaded;

            color_picker.SelectedColorChanged += delegate { this.drawCanvas.Brush = color_picker.SelectedBrush; btn_color.IsChecked = false; };
            color_picker.Canceled += delegate { btn_color.IsChecked = false; };

            this.toolbar.AddHandler(RadioButton.CheckedEvent, new RoutedEventHandler(OnDrawToolChecked));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            file = FilePath;
            if (string.IsNullOrEmpty(file)) return;
            ImageHelper.ToImages(file);

            var directory = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
            var files = Directory.GetFiles(directory);
            foreach (var item in files)
            {
                if (!item.Contains("_update") && !item.Contains("_original"))
                {
                    images.Add(item);
                }
            }
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
                this.btn_save_Click(null, null);
                drawCanvas.Clear();
            }
            // 加载页面图片
            this.currentFilePath = images[currentIndex];
            //this.drawViewer.BackgroundImage = new BitmapImage(new Uri(currentFilePath));
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(currentFilePath, UriKind.RelativeOrAbsolute);
            //bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            this.drawViewer.BackgroundImage = bitmapImage.Clone();
            this.drawViewer.InvalidateVisual();
            this.currentPage.Text = (currentIndex + 1).ToString();
            UpdateImage();
        }

        private void UpdateImage()
        {
            // 替换原图
            var directory = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
            var files = Directory.GetFiles(directory);
            var currentFileName = Path.GetFileNameWithoutExtension(currentFilePath);
            foreach (var item in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(item);
                if (!item.Contains("_update")) continue;
                if (fileName.Contains(currentFileName)) continue;

                try
                {
                    var original = item.Replace("_update", "");
                    var originalFilePath = Path.Combine(directory, fileName.Replace("_update", "") + "_original" + Path.GetExtension(item));
                    if (!File.Exists(originalFilePath))
                        File.Move(original, originalFilePath);
                    if (File.Exists(original))
                        File.Delete(original);
                    File.Move(item, original);
                    //File.Copy(item, original);
                }
                catch (Exception ex)
                {
                }

            }
        }

        private void MainWindow_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.btn_redo.IsEnabled = drawCanvas.RedoEnabled;
            this.btn_redo.Opacity = drawCanvas.RedoEnabled ? 1 : 0.7;

            this.btn_undo.IsEnabled = drawCanvas.UndoEnabled;
            this.btn_undo.Opacity = drawCanvas.UndoEnabled ? 1 : 0.7;
            if (drawCanvas.DrawingToolType == DrawToolType.Pointer)
                this.Pointer.IsChecked = true;

        }
        private void OnDrawToolChecked(Object sender, RoutedEventArgs e)
        {
            if (e.Source is RadioButton btn && btn.Tag is String typeStr)
                drawCanvas.DrawingToolType = (DrawToolType)Enum.Parse(typeof(DrawToolType), typeStr);
        }

        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            drawCanvas.Clear();
        }

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

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            this.drawCanvas.Undo();
        }

        private void Redo_Click_1(object sender, RoutedEventArgs e)
        {
            this.drawCanvas.Redo();
        }

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

        private void first_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = 0;
            LoadImage();
        }

        private void before_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == 0) return;
            currentIndex--;
            LoadImage();
        }

        private void after_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex == images.Count() - 1) return;
            currentIndex++;
            LoadImage();
        }

        private void last_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = images.Count() - 1;
            LoadImage();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var backgroundImage = this.drawViewer.BackgroundImage;
                var frame = this.drawCanvas.ToBitmapFrame(backgroundImage);
                //this.drawCanvas = null;
                if (frame == null || string.IsNullOrEmpty(currentFilePath))
                    return;

                var fileName = Path.Combine(Path.GetDirectoryName(currentFilePath), Path.GetFileNameWithoutExtension(currentFilePath) + "_update" + Path.GetExtension(currentFilePath));
                ImageHelper.Save(fileName, frame);
                MessageBox.Show("文件保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("文件保存失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 绑定患者保存并生成pdf
        /// </summary>
        public void SaveFile()
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

        public void ImageToPdf()
        {
            var directory = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
            var files = Directory.GetFiles(directory);
            List<string> list = new List<string>();
            foreach (var item in files)
            {
                if (!item.Contains("_original"))
                {
                    list.Add(item);
                }
            }
            var update = list.Where(t => t.Contains("_update")).ToList();
            foreach (var item in update)
            {
                var name = item.Replace("_update", "");
                list.Remove(name);
            }
            ImageHelper.ToPDF(list, file);
        }
    }
}
