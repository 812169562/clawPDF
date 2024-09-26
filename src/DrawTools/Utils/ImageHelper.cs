using clawSoft.clawPDF.Core.Settings;
using Newtonsoft.Json;
using PdfiumViewer;
using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
                fs?.Dispose();
                fs?.Close();
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
                if (Directory.Exists(imgPath))
                {
                    Directory.Delete(imgPath, true);
                }
                Directory.CreateDirectory(imgPath);
                var dpi = SystemConfig.Setting.Dpi ?? 150;
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
        /// <summary>
        /// Images
        /// </summary>
        /// <param name="path"></param>
        public static void ToPDF(List<string> files, string pdffile)
        {
            try
            {
                using (PdfSharp.Pdf.PdfDocument pdfDocument = new PdfSharp.Pdf.PdfDocument())
                {
                    files = files.OrderBy(t => t).ToList();
                    foreach (var img in files)
                    {
                        XImage image = XImage.FromFile(img);
                        var page = pdfDocument.AddPage();
                        if (SystemConfig.Setting.PageSize == "A4")
                        {
                            page.Size = PageSize.A4;
                            double widthMm = image.PixelWidth / image.HorizontalResolution * 25.4;
                            double a4WidthMm = 210.0;
                            if (widthMm > a4WidthMm && image.PixelWidth > image.PixelHeight)
                            {
                                page.Orientation = PageOrientation.Landscape;
                            }
                        }
                        else
                        {
                            page.Width = image.PixelWidth;
                            page.Height = image.PixelHeight;
                        }
                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        double scaleX = page.Width / image.PixelWidth;
                        double scaleY = page.Height / image.PixelHeight;
                        if (scaleX < scaleY)
                        {
                            scaleY = scaleX;
                        }
                        XRect pageRect = new XRect(0, 0, image.PixelWidth * scaleX, image.PixelHeight * scaleY);
                        gfx.DrawImage(image, pageRect);
                        image.Dispose();
                        gfx.Dispose();
                    }
                    pdfDocument.Save(pdffile);
                    //pdfDocument.Save("D:\\szyx\\test-pdf\\sign\\1.pdf");
                    pdfDocument?.Dispose();
                    pdfDocument?.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
