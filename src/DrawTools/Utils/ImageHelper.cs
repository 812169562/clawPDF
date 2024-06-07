using PdfiumViewer;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;

namespace DrawTools.Utils
{
    public static class ImageHelper
    {
        public static void Save(String filepath, params BitmapFrame[] frames)
        {
            BitmapEncoder encoder = null;


            switch (Path.GetExtension(filepath))
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                default:
                    encoder = new BmpBitmapEncoder();
                    break;
            }


            foreach (var frame in frames)
            {
                encoder.Frames.Add(frame);
            }

            using (var fs = new FileStream(filepath, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }


        /// <summary>
        /// PDF
        /// </summary>
        /// <param name="path"></param>
        public static void ToImages(string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                var name = Path.GetFileNameWithoutExtension(path);
                var imgPath = Path.Combine(directory, name);
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }
                var dpi = 96;
                using (var document = PdfDocument.Load(path))
                {
                    for (int i = 0; i < document.PageCount; i++)
                    {
                        using (var image = document.Render(i, dpi, dpi, PdfRenderFlags.CorrectFromDpi))
                        {
                            var savePath = Path.Combine(imgPath, Directory.GetParent(path).Name + i.ToString().PadLeft(5, '0') + ".jpg");
                            image.Save(savePath, ImageFormat.Jpeg);
                            image.Dispose();
                        }
                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
