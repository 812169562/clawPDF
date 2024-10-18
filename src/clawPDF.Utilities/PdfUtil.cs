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
        public static void AddBase64Image(string pdfPath, string base64Image, int signPage, int x, int y, string outPath)
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
                    document.Save(outPath);
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
            double scale = 1.5;
            int width = Convert.ToInt32(image.PixelWidth * scale);
            int height = Convert.ToInt32(image.PixelHeight * scale);
            x = page.Width - width >= x ? x : Convert.ToInt32(page.Width - width);
            y = page.Height - height >= y ? Convert.ToInt32(page.Height - y - 22) : Convert.ToInt32(page.Height - height);
            using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append))
            {
                XRect pageRect = new XRect(x, y, width, height);
                gfx.DrawImage(image, pageRect);
                //gfx.DrawImage(image, x, y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startPosition">从第X位开始</param>
        /// <param name="length">读取X位，即X字节</param>
        /// <returns></returns>
        public static byte[] GetBytes(string path, long offset, int length)
        {
            // 确保文件存在
            if (!File.Exists(path))
                return null;
            // 打开文件流
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                if (length <= 0) length = (int)stream.Length;
                // 创建缓冲区来存储读取的字节
                byte[] buffer = new byte[length];
                // 确保文件长度足够读取
                if (stream.Length < offset + length)
                {
                    buffer = new byte[stream.Length < length ? stream.Length : length];
                    stream.Read(buffer, 0, (int)stream.Length);
                }
                // 移动到起始位置
                stream.Seek(offset - 1, SeekOrigin.Begin);
                // 读取指定长度的字节
                int bytesRead = stream.Read(buffer, 0, length);
                return buffer;
            }
        }
    }
}
