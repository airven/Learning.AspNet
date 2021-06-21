using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Aspose.Words.Drawing;
using iTextSharp.text.pdf;
using System.Drawing.Drawing2D;

namespace Common.WaterMarker
{
    public static class ImgExtension
    {
        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
        public static Stream ToStream(this Document document)
        {
            var stream = new System.IO.MemoryStream();
            document.Save(stream, SaveFormat.Doc);
            stream.Position = 0;
            return stream;
        }
    }
    public class WaterMarkerDownLoad
    {
        /// <summary>
        /// 对图片添加文字水印
        /// </summary>
        /// <param name="sourcefile">图片源路径</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="desfile">图片目的路径</param>
        public static void AddImgWaterMark(string sourcefile, string watermarkText, string desfile)
        {
            try
            {
                Stream stream = new MemoryStream(System.IO.File.ReadAllBytes(sourcefile));
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                //System.Drawing.Image img = System.Drawing.Image.FromFile(sourcefile);
                Graphics gr = Graphics.FromImage(img);
                System.Drawing.Font font = new System.Drawing.Font("Tahoma", (float)40, FontStyle.Bold, GraphicsUnit.Pixel);
                Color color = Color.FromArgb(20, Color.Gray);//241, 235, 105 Color.Silver
                double tangent = (double)img.Height / (double)img.Width;
                double angle = Math.Atan(tangent) * (180 / Math.PI);
                double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;
                double sin, cos, opp1, adj1, opp2, adj2;

                for (int i = 100; i > 0; i--)
                {
                    font = new System.Drawing.Font("Tahoma", i, FontStyle.Bold);
                    SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue);

                    sin = Math.Sin(angle * (Math.PI / 180));
                    cos = Math.Cos(angle * (Math.PI / 180));
                    opp1 = sin * sizef.Width;
                    adj1 = cos * sizef.Height;
                    opp2 = sin * sizef.Height;
                    adj2 = cos * sizef.Width;

                    if (opp1 + adj1 < img.Height && opp2 + adj2 < img.Width)
                        break;
                }
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.RotateTransform((float)angle);
                gr.DrawString(watermarkText, font, new SolidBrush(color), new Point((int)halfHypotenuse, 0), stringFormat);

                try
                {
                    img.Save(desfile, ImageFormat.Jpeg);
                }
                finally
                {
                    if (img != null)
                    {
                        img.Dispose();
                        img = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("{0},{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 对word文档添加文字水印
        /// </summary>
        /// <param name="filepath">word文件源路径</param>
        /// <param name="watermarkText">word水印文字</param>
        public static void AddWordWaterMark(string filepath, string watermarkText)
        {
            try
            {
                Document doc = new Document(filepath);
                Paragraph watermarkPara = new Paragraph(doc);
                IList<Tuple<VerticalAlignment, HorizontalAlignment>> points = new List<Tuple<VerticalAlignment, HorizontalAlignment>>();
                points.Add(Tuple.Create(VerticalAlignment.Top, HorizontalAlignment.Left));
                points.Add(Tuple.Create(VerticalAlignment.Center, HorizontalAlignment.Center));
                points.Add(Tuple.Create(VerticalAlignment.Bottom, HorizontalAlignment.Right));

                foreach (var point in points)
                {
                    Shape shape = ShapeGenerate(doc, watermarkText, 50, 50, point.Item1, point.Item2);
                    watermarkPara.AppendChild(shape);
                }
                foreach (Section sect in doc.Sections)
                {
                    // the watermark to appear on all pages, insert into all headers.
                    InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                    InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                    InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
                }
                doc.Save(filepath);
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("{0},{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// 对pdf文档添加文字水印
        /// </summary>
        /// <param name="sourcefile">源文件</param>
        /// <param name="desfile">目的文件</param>
        /// <param name="watermarkText">水印文字</param>
        public static void AddPdfWaterMark(string sourcefile, string desfile, string watermarkText)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(System.IO.File.ReadAllBytes(sourcefile));
                pdfStamper = new PdfStamper(pdfReader, new FileStream(desfile, FileMode.Create));// 设置密码
                int total = pdfReader.NumberOfPages + 1;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\SIMFANG.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                gs.FillOpacity = 0.2f;

                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);

                float width = psize.Width;
                float height = psize.Height;
                int j = watermarkText.Length;
                int rise = 0;

                for (int i = 1; i < total; i++)
                {
                    rise = 100;
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    content.BeginText();
                    content.SetColorFill(iTextSharp.text.BaseColor.LIGHT_GRAY);
                    content.SetFontAndSize(font, 60);
                    content.SetTextRise(rise);
                    content.SetGState(gs);
                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, watermarkText, 300, 500, 45);
                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_CENTER, watermarkText, 400, 400, 45);
                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_RIGHT, watermarkText, 500, 300, 45);
                    content.EndText();
                }

            }
            catch (Exception ex)
            {
                WriteLog(string.Format("{0},{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        /// <summary>
        /// 对图片添加文字水印
        /// </summary>
        /// <param name="sourcefile">图片源路径</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="desfile">图片目的路径</param>
        public static Stream AddImgWaterMark(string sourcefile, string watermarkText)
        {
            try
            {
                Stream stream = new MemoryStream(System.IO.File.ReadAllBytes(sourcefile));
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                //System.Drawing.Image img = System.Drawing.Image.FromFile(sourcefile);
                Graphics gr = Graphics.FromImage(img);
                System.Drawing.Font font = new System.Drawing.Font("Tahoma", (float)40, FontStyle.Bold, GraphicsUnit.Pixel);
                Color color = Color.FromArgb(20, Color.Gray);//241, 235, 105 Color.Silver
                double tangent = (double)img.Height / (double)img.Width;
                double angle = Math.Atan(tangent) * (180 / Math.PI);
                double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;
                double sin, cos, opp1, adj1, opp2, adj2;

                for (int i = 100; i > 0; i--)
                {
                    font = new System.Drawing.Font("Tahoma", i, FontStyle.Bold);
                    SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue);

                    sin = Math.Sin(angle * (Math.PI / 180));
                    cos = Math.Cos(angle * (Math.PI / 180));
                    opp1 = sin * sizef.Width;
                    adj1 = cos * sizef.Height;
                    opp2 = sin * sizef.Height;
                    adj2 = cos * sizef.Width;

                    if (opp1 + adj1 < img.Height && opp2 + adj2 < img.Width)
                        break;
                }
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.RotateTransform((float)angle);
                gr.DrawString(watermarkText, font, new SolidBrush(color), new Point((int)halfHypotenuse, 0), stringFormat);

                try
                {
                    return img.ToStream(ImageFormat.Jpeg);
                }
                finally
                {
                    if (img != null)
                    {
                        img.Dispose();
                        img = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("{0},{1}", ex.Message, ex.StackTrace));
            }
            return new MemoryStream(System.IO.File.ReadAllBytes(sourcefile));
        }

        /// <summary>
        /// 对word文档添加文字水印
        /// </summary>
        /// <param name="filepath">word文件源路径</param>
        /// <param name="watermarkText">word水印文字</param>
        public static Stream AddWordWaterMarkV2(string filepath, string watermarkText)
        {
            try
            {
                Document doc = new Document(filepath);
                Paragraph watermarkPara = new Paragraph(doc);
                IList<Tuple<VerticalAlignment, HorizontalAlignment>> points = new List<Tuple<VerticalAlignment, HorizontalAlignment>>();
                points.Add(Tuple.Create(VerticalAlignment.Top, HorizontalAlignment.Left));
                points.Add(Tuple.Create(VerticalAlignment.Center, HorizontalAlignment.Center));
                points.Add(Tuple.Create(VerticalAlignment.Bottom, HorizontalAlignment.Right));

                foreach (var point in points)
                {
                    Shape shape = ShapeGenerate(doc, watermarkText, 50, 50, point.Item1, point.Item2);
                    watermarkPara.AppendChild(shape);
                }
                foreach (Section sect in doc.Sections)
                {
                    InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                    InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                    InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
                }
                return doc.ToStream();
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("{0},{1}", ex.Message, ex.StackTrace));
            }
            return new MemoryStream(System.IO.File.ReadAllBytes(filepath));
        }
        public static Stream AddPdfWaterMark(string sourceFile, string stringToWriteToPdf)
        {
            PdfReader reader = null;
            PdfStamper pdfStamper = null;
            try
            {
                reader = new PdfReader(System.IO.File.ReadAllBytes(sourceFile));
                iTextSharp.text.Rectangle psize = reader.GetPageSize(1);

                float width = psize.Width;
                float height = psize.Height;
                int j = stringToWriteToPdf.Length;
                PdfGState gs = new PdfGState();
                gs.FillOpacity = 0.15f;
                BaseFont font = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\SIMFANG.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int rise = 100;
                    int total = reader.NumberOfPages + 1;
                    pdfStamper = new PdfStamper(reader, memoryStream);
                    for (int i = 1; i < total; i++)
                    {
                        PdfContentByte pdfPageContents = pdfStamper.GetOverContent(i);
                        BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, Encoding.ASCII.EncodingName, false);
                        pdfPageContents.BeginText();
                        pdfPageContents.SetFontAndSize(baseFont, 40);
                        pdfPageContents.SetRGBColorFill(255, 0, 0);
                        pdfPageContents.SetColorFill(iTextSharp.text.BaseColor.GRAY);
                        pdfPageContents.SetFontAndSize(font, 60);
                        pdfPageContents.SetTextRise(rise);
                        pdfPageContents.SetGState(gs);
                        pdfPageContents.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, stringToWriteToPdf, 300, 500, 45);
                        pdfPageContents.ShowTextAligned(iTextSharp.text.Element.ALIGN_CENTER, stringToWriteToPdf, 400, 400, 45);
                        pdfPageContents.ShowTextAligned(iTextSharp.text.Element.ALIGN_RIGHT, stringToWriteToPdf, 500, 300, 45);
                        pdfPageContents.EndText();
                    }
                    pdfStamper.FormFlattening = true;
                    pdfStamper.Close();
                    return new System.IO.MemoryStream(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("{0},{1}", ex.Message, ex.StackTrace));
                return new System.IO.MemoryStream(File.ReadAllBytes(sourceFile));
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();

                if (reader != null)
                    reader.Close();
            }
        }
        private static Shape ShapeGenerate(Document doc, string textMark, double left, double top, VerticalAlignment alignV, HorizontalAlignment alignH)
        {
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);
            // Set up the text of the watermark.
            watermark.TextPath.Text = textMark;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 400;
            watermark.Height = 100;
            watermark.Top = top;
            watermark.Left = left;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color = Color.Gray;
            watermark.Fill.Opacity = 0.1;
            watermark.StrokeColor = Color.Gray;
            watermark.Stroke.Opacity = 0.1;
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = alignV;
            watermark.HorizontalAlignment = alignH;
            return watermark;
        }
        private static void InsertWatermarkIntoHeader(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
        {
            HeaderFooter header = sect.HeadersFooters[headerType];
            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }
            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));
        }
        private static void WriteLog(string msg)
        {
            try
            {
                string path = @"D:\";
                string filePath = Path.Combine(path, "ErrMsg");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fileFullPath = Path.Combine(filePath, string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));
                StreamWriter sw = new StreamWriter(fileFullPath, false);
                sw.WriteLine(msg);

                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                //释放
                sw.Dispose();
            }
            catch
            {
            }
        }
    }
}
