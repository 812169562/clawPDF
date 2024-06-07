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

            images.AddRange(files);
            this.totalPages.Text = "/ " + files.Count();
            currentIndex = 0;
            LoadImage();
        }
        private void LoadImage()
        {
            if (images.Count() > 0 && currentIndex < images.Count())
            {
                this.drawViewer.BackgroundImage = new BitmapImage(new Uri(images[currentIndex]));
                this.currentPage.Text = (currentIndex + 1).ToString();
            }
        }
        private void MainWindow_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.btn_redo.IsEnabled = drawCanvas.RedoEnabled;
            this.btn_redo.Opacity = drawCanvas.RedoEnabled ? 1 : 0.7;

            this.btn_undo.IsEnabled = drawCanvas.UndoEnabled;
            this.btn_undo.Opacity = drawCanvas.UndoEnabled ? 1 : 0.7;
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
            var backgroundImage = this.drawViewer.BackgroundImage;

            var frame = this.drawCanvas.ToBitmapFrame(backgroundImage);

            if (frame == null)
                return;

            var file = images[currentIndex];
            var fileName = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_update" + Path.GetExtension(file));
            ImageHelper.Save(fileName, frame);
        }
    }
}
