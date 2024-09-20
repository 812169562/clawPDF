using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.IO;

namespace clawSoft.clawPDF.Utilities
{
    public class PdfUtil
    {
        /// <summary>
        /// PDF增加签名图片
        /// </summary>
        /// <param name="pdfPath">PDF文件路径</param>
        /// <param name="base64Image">签名base64值</param>
        /// <param name="signPage">签名页码 1、第一页 2、每一页 3、最后一页</param>
        /// <param name="x">签名位置X轴</param>
        /// <param name="y">签名位置Y轴</param>
        /// <exception cref="Exception"></exception>
        public static void AddBase64Image(string pdfPath, string base64Image, int signPage, int x, int y)
        {
            try
            {
                if (base64Image.IsEmpty() || pdfPath.IsEmpty())
                    return;
                using (PdfDocument document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Modify))
                {
                    // 转换Base64字符串为图像
                    byte[] imageBytes = Convert.FromBase64String(base64Image);
                    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    XImage image = XImage.FromStream(ms);
                    if (signPage == 1)
                    {
                        PdfPage page = document.Pages[0];
                        DrawImage(x, y, page, image);
                    }
                    else if (signPage == 2)
                    {
                        foreach (PdfPage page in document.Pages)
                        {
                            DrawImage(x, y, page, image);
                        }
                    }
                    else
                    {
                        int pages = document.Pages.Count;
                        PdfPage page = document.Pages[pages - 1];
                        DrawImage(x, y, page, image);
                    }
                    image.Dispose();
                    document.Save(pdfPath);
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="page"></param>
        /// <param name="image"></param>
        private static void DrawImage(int x, int y, PdfPage page, XImage image)
        {
            x = page.Width - image.PixelWidth >= x ? x : Convert.ToInt32(page.Width - image.PixelWidth);
            y = page.Height - image.PixelHeight >= y ? y : Convert.ToInt32(page.Height - image.PixelHeight);
            using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append))
            {
                gfx.DrawImage(image, x, y);
            }
        }
    }
}
