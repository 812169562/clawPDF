using clawSoft.clawPDF.Core.Settings;
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
                using (var document = PdfiumViewer.PdfDocument.Load(path))
                {
                    //for (int i = 0; i < document.PageCount; i++)
                    //{
                    //    int width = Convert.ToInt32(document.PageSizes[i].Width);
                    //    int height = Convert.ToInt32(document.PageSizes[i].Height);
                    //    using (var image = document.Render(i, width, height, dpi, dpi, PdfRenderFlags.Annotations))
                    //    {
                    //        var savePath = Path.Combine(imgPath, Directory.GetParent(path).Name + i.ToString().PadLeft(5, '0') + ".png");
                    //        image.Save(savePath, ImageFormat.Png);
                    //    }
                    //    Thread.Sleep(1);
                    //}
                    for (int i = 0; i < document.PageCount; i++)
                    {
                        using (var image = document.Render(i, dpi, dpi, PdfRenderFlags.CorrectFromDpi))
                        {
                            var savePath = Path.Combine(imgPath, Directory.GetParent(path).Name + i.ToString().PadLeft(5, '0') + ".png");
                            var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Png.Guid);
                            var encParams = new EncoderParameters(1);
                            encParams.Param[0] = new EncoderParameter(Encoder.Compression, 10L);
                            image.Save(savePath, encoder, encParams);
                            image.Dispose();
                        }
                        Thread.Sleep(1);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
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
                var document = PdfiumViewer.PdfDocument.Load(pdffile);
                using (PdfSharp.Pdf.PdfDocument pdfDocument = new PdfSharp.Pdf.PdfDocument())
                {
                    files = files.OrderBy(t => t).ToList();
                    for (int i = 0; i < files.Count; i++)
                    {
                        var img = files[i];
                        XImage image = XImage.FromFile(img);
                        var page = pdfDocument.AddPage();
                        if (SystemConfig.Setting.PageSize == "A4")
                        {
                            page.Size = PdfSharp.PageSize.A4;
                            double widthMm = image.PixelWidth / image.HorizontalResolution * 25.4;
                            double a4WidthMm = 210.0;
                            if (widthMm > a4WidthMm && image.PixelWidth > image.PixelHeight)
                            {
                                page.Orientation = PageOrientation.Landscape;
                            }
                        }
                        else
                        {
                            page.Width = document.PageSizes[i].Width;
                            page.Height = document.PageSizes[i].Height;
                            //page.Width = image.PixelWidth;
                            //page.Height = image.PixelHeight;
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
                    document.Dispose();
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

        //public static bool PrintToPdf(List<string> files, string pdffile)
        //{
        //    try
        //    {
        //        PdfWriter pdfWriter = new PdfWriter(pdffile);
        //        using (iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
        //        {
        //            pdfDocument.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4);
        //            using (Document document = new Document(pdfDocument))
        //            {
        //                foreach (var item in files)
        //                {
        //                    var image = new iText.Layout.Element.Image(ImageDataFactory.Create(item));
        //                    document.Add(image);
        //                }
        //                document.Close();
        //                pdfDocument.Close();
        //                pdfWriter.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static void PdfIText(List<string> files, string fileName)
        //{
        //    if (files == null || files.Count == 0)
        //        return;
        //    File.Delete(fileName);
        //    Document doc = new Document();
        //    if (SystemConfig.Setting.PageSize == "Other")
        //    {
        //        Image imag = Image.GetInstance(files[0]);
        //        Rectangle rect = new Rectangle(0, 0, imag.Width, imag.Height); // 自定义页面大小为500x800，左下角坐标为(100, 200)
        //        doc = new Document(rect);
        //        doc.SetMargins(0, 0, 0, 0);
        //    }
        //    using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
        //    using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
        //    {
        //        doc.Open();
        //        foreach (var img in files)
        //        {
        //            // 添加图片
        //            Image image = Image.GetInstance(img);
        //            // 设置图片的位置和大小
        //            image.SetAbsolutePosition(0, 0);
        //            image.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
        //            doc.NewPage();
        //            doc.Add(image);
        //        }

        //        doc.Close();
        //        fs.Close();
        //        writer.Close();
        //    }
        //}
    }
}
