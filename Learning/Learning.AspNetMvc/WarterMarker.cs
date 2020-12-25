using iTextSharp.text.pdf;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Learning.AspNetMvc
{

    #region WatermarkPosition
    public enum WatermarkPosition
    {
        Absolute,
        TopLeft,
        TopRight,
        TopMiddle,
        BottomLeft,
        BottomRight,
        BottomMiddle,
        MiddleLeft,
        MiddleRight,
        Center
    }
    #endregion

    public static class ImgExtension
    {
        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
    #region Watermarker
    public class WarterMarker
    {

        #region Private Fields
        private Image m_image;
        private Image m_originalImage;
        private Image m_watermark;
        private float m_opacity = 1.0f;
        private WatermarkPosition m_position = WatermarkPosition.Absolute;
        private int m_x = 0;
        private int m_y = 0;
        private Color m_transparentColor = Color.Empty;
        private RotateFlipType m_rotateFlip = RotateFlipType.RotateNoneFlipNone;
        private Padding m_margin = new Padding(0);
        private Font m_font = new Font(FontFamily.GenericSansSerif, 10);
        private Color m_fontColor = Color.Gray;// Color.DarkGray;
        private float m_scaleRatio = 1.0f;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the image with drawn watermarks
        /// </summary>
        [Browsable(false)]
        public Image Image { get { return m_image; } }

        /// <summary>
        /// Watermark position relative to the image sizes. 
        /// If Absolute is chosen, watermark positioning is being done via PositionX and PositionY 
        /// properties (0 by default)\n
        /// </summary>        
        public WatermarkPosition Position { get { return m_position; } set { m_position = value; } }

        /// <summary>
        /// Watermark X coordinate (works if Position property is set to WatermarkPosition.Absolute)
        /// </summary>
        public int PositionX { get { return m_x; } set { m_x = value; } }

        /// <summary>
        /// Watermark Y coordinate (works if Position property is set to WatermarkPosition.Absolute)
        /// </summary>
        public int PositionY { get { return m_y; } set { m_y = value; } }

        /// <summary>
        /// Watermark opacity. Can have values from 0.0 to 1.0
        /// </summary>
        public float Opacity { get { return m_opacity; } set { m_opacity = value; } }

        /// <summary>
        /// Transparent color
        /// </summary>
        public Color TransparentColor { get { return m_transparentColor; } set { m_transparentColor = value; } }

        /// <summary>
        /// Watermark rotation and flipping
        /// </summary>
        public RotateFlipType RotateFlip { get { return m_rotateFlip; } set { m_rotateFlip = value; } }

        /// <summary>
        /// Spacing between watermark and image edges
        /// </summary>
        public Padding Margin { get { return m_margin; } set { m_margin = value; } }

        /// <summary>
        /// Watermark scaling ratio. Must be greater than 0. Only for image watermarks
        /// </summary>
        public float ScaleRatio { get { return m_scaleRatio; } set { m_scaleRatio = value; } }

        /// <summary>
        /// Font of the text to add
        /// </summary>
        public Font Font { get { return m_font; } set { m_font = value; } }

        /// <summary>
        /// Color of the text to add
        /// </summary>
        public Color FontColor { get { return m_fontColor; } set { m_fontColor = value; } }


        #endregion

        #region Constructors
        public WaterHelper(Image image)
        {
            LoadImage(image);
        }

        public WaterHelper(string filename)
        {
            LoadImage(Image.FromFile(filename));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets image, clearing all drawn watermarks
        /// </summary>
        public void ResetImage()
        {
            m_image = new Bitmap(m_originalImage);
        }

        public void DrawImage(string filename)
        {
            DrawImage(Image.FromFile(filename));
        }

        public void DrawImage(Image watermark)
        {

            if (watermark == null)
                throw new ArgumentOutOfRangeException("Watermark");

            if (m_opacity < 0 || m_opacity > 1)
                throw new ArgumentOutOfRangeException("Opacity");

            if (m_scaleRatio <= 0)
                throw new ArgumentOutOfRangeException("ScaleRatio");

            // Creates a new watermark with margins (if margins are not specified returns the original watermark)
            m_watermark = GetWatermarkImage(watermark);

            // Rotates and/or flips the watermark
            m_watermark.RotateFlip(m_rotateFlip);

            // Calculate watermark position
            Point waterPos = GetWatermarkPosition();

            // Watermark destination rectangle
            Rectangle destRect = new Rectangle(waterPos.X, waterPos.Y, m_watermark.Width, m_watermark.Height);

            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][] {
                    new float[] { 1, 0f, 0f, 0f, 0f},
                    new float[] { 0f, 1, 0f, 0f, 0f},
                    new float[] { 0f, 0f, 1, 0f, 0f},
                    new float[] { 0f, 0f, 0f, m_opacity, 0f},
                    new float[] { 0f, 0f, 0f, 0f, 1}
                });

            ImageAttributes attributes = new ImageAttributes();

            // Set the opacity of the watermark
            attributes.SetColorMatrix(colorMatrix);

            // Set the transparent color 
            if (m_transparentColor != Color.Empty)
            {
                attributes.SetColorKey(m_transparentColor, m_transparentColor);
            }

            // Draw the watermark
            using (Graphics gr = Graphics.FromImage(m_image))
            {
                gr.DrawImage(m_watermark, destRect, 0, 0, m_watermark.Width, m_watermark.Height, GraphicsUnit.Pixel, attributes);
            }
        }

        public void DrawText(string text)
        {
            // Convert text to image, so we can use opacity etc.
            Image textWatermark = GetTextWatermark(text);

            DrawImage(textWatermark);
        }
        #endregion

        #region Private Methods
        private void LoadImage(Image image)
        {
            m_originalImage = image;
            ResetImage();
        }

        private Image GetTextWatermark(string text)
        {
            Brush brush = new SolidBrush(m_fontColor);
            SizeF size;

            // Figure out the size of the box to hold the watermarked text
            using (Graphics g = Graphics.FromImage(m_image))
            {
                size = g.MeasureString(text, m_font);
            }

            // Create a new bitmap for the text, and, actually, draw the text
            Bitmap bitmap = new Bitmap((int)size.Width, (int)size.Height);
            bitmap.SetResolution(m_image.HorizontalResolution, m_image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawString(text, m_font, brush, 0, 0);
            }

            return bitmap;
        }

        private Image GetWatermarkImage(Image watermark)
        {

            // If there are no margins specified and scale ration is 1, no need to create a new bitmap
            if (m_margin.All == 0 && m_scaleRatio == 1.0f)
                return watermark;

            // Create a new bitmap with new sizes (size + margins) and draw the watermark
            int newWidth = Convert.ToInt32(watermark.Width * m_scaleRatio);
            int newHeight = Convert.ToInt32(watermark.Height * m_scaleRatio);

            Rectangle sourceRect = new Rectangle(m_margin.Left, m_margin.Top, newWidth, newHeight);
            Rectangle destRect = new Rectangle(0, 0, watermark.Width, watermark.Height);

            Bitmap bitmap = new Bitmap(newWidth + m_margin.Left + m_margin.Right, newHeight + m_margin.Top + m_margin.Bottom);
            bitmap.SetResolution(watermark.HorizontalResolution, watermark.VerticalResolution);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(watermark, sourceRect, destRect, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        private Point GetWatermarkPosition()
        {
            int x = 0;
            int y = 0;

            switch (m_position)
            {
                case WatermarkPosition.Absolute:
                    x = m_x; y = m_y;
                    break;
                case WatermarkPosition.TopLeft:
                    x = 0; y = 0;
                    break;
                case WatermarkPosition.TopRight:
                    x = m_image.Width - m_watermark.Width; y = 0;
                    break;
                case WatermarkPosition.TopMiddle:
                    x = (m_image.Width - m_watermark.Width) / 2; y = 0;
                    break;
                case WatermarkPosition.BottomLeft:
                    x = 0; y = m_image.Height - m_watermark.Height;
                    break;
                case WatermarkPosition.BottomRight:
                    x = m_image.Width - m_watermark.Width; y = m_image.Height - m_watermark.Height;
                    break;
                case WatermarkPosition.BottomMiddle:
                    x = (m_image.Width - m_watermark.Width) / 2; y = m_image.Height - m_watermark.Height;
                    break;
                case WatermarkPosition.MiddleLeft:
                    x = 0; y = (m_image.Height - m_watermark.Height) / 2;
                    break;
                case WatermarkPosition.MiddleRight:
                    x = m_image.Width - m_watermark.Width; y = (m_image.Height - m_watermark.Height) / 2;
                    break;
                case WatermarkPosition.Center:
                    x = (m_image.Width - m_watermark.Width) / 2; y = (m_image.Height - m_watermark.Height) / 2;
                    break;
                default:
                    break;
            }

            return new Point(x, y);
        }
        #endregion

        private void PdfStampWithNewFile(string watermarkImagePath, string sourceFilePath, string destinationFilePath)
        {
            var pdfReader = new PdfReader(sourceFilePath);
            var pdfStamper = new PdfStamper(pdfReader, new FileStream(destinationFilePath, FileMode.Create));
            var image = iTextSharp.text.Image.GetInstance(watermarkImagePath);
            image.SetAbsolutePosition(200, 400);
            for (var i = 0; i < pdfReader.NumberOfPages; i++)
            {
                var content = pdfStamper.GetUnderContent(i + 1);
                content.AddImage(image);
            }
            pdfStamper.Close();
        }

        private void PdfStampInExistingFile(string watermarkImagePath, string sourceFilePath)
        {
            byte[] bytes = File.ReadAllBytes(sourceFilePath);
            var img = iTextSharp.text.Image.GetInstance(watermarkImagePath);
            img.SetAbsolutePosition(200, 400);
            PdfContentByte waterMark;

            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        waterMark = stamper.GetUnderContent(i);
                        waterMark.AddImage(img);
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(sourceFilePath, bytes);
        }

        public void PdfStampInExistingFileV2(string waterText, string sourceFilePath)
        {
            byte[] bytes = File.ReadAllBytes(sourceFilePath);
            Image mage = GetTextWatermark(waterText);
            var img = iTextSharp.text.Image.GetInstance(mage, ImageFormat.Png);
            img.GrayFill = 40;
            PdfReader reader = new PdfReader(bytes);
            iTextSharp.text.Rectangle pagesize = reader.GetPageSize(1);
            float waterLeft = pagesize.Width - img.Width - 10;
            float waterTop = pagesize.Height - img.Height - 10;
            img.SetAbsolutePosition(waterLeft, waterTop);
            PdfContentByte waterMark;

            using (MemoryStream stream = new MemoryStream())
            {

                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        waterMark = stamper.GetUnderContent(i);
                        waterMark.AddImage(img);
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(sourceFilePath, bytes);
        }


        public void PdfStampInExistingFileV3(string waterText, string sourceFilePath, string destinationFilePath)
        {
            var pdfReader = new PdfReader(System.IO.File.ReadAllBytes(sourceFilePath));
            var pdfStamper = new PdfStamper(pdfReader, new FileStream(destinationFilePath, FileMode.Create));

            System.Drawing.Image mage = GetTextWatermark(waterText);
            var image = iTextSharp.text.Image.GetInstance(mage, ImageFormat.Png);
            image.GrayFill = 40;
            iTextSharp.text.Rectangle pagesize = pdfReader.GetPageSize(1);
            float waterLeft = pagesize.Width - image.Width - 10;
            float waterTop = pagesize.Height - image.Height - 10;
            image.SetAbsolutePosition(waterLeft, waterTop);
            image.SetAbsolutePosition(200, 400);
            for (var i = 0; i < pdfReader.NumberOfPages; i++)
            {
                var content = pdfStamper.GetOverContent(i + 1);
                content.AddImage(image);
            }
            pdfStamper.Close();
        }


        /// <summary>
        /// 添加倾斜水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="waterMarkName"></param>
        /// <param name="userPassWord"></param>
        /// <param name="ownerPassWord"></param>
        /// <param name="permission"></param>
        public void PdfStampInExistingFileV4(string inputfilepath, string outputfilepath, string waterMarkName, string userPassWord, string ownerPassWord, int permission)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(System.IO.File.ReadAllBytes(inputfilepath));
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));// 设置密码
                //pdfStamper.SetEncryption(false,userPassWord, ownerPassWord, permission); 

                int total = pdfReader.NumberOfPages + 1;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\SIMFANG.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                gs.FillOpacity = 0.2f;//透明度


                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;

                int j = waterMarkName.Length;
                char c;
                int rise = 0;

                float ta = 1f, tb = 0f, tc = 0f, td = 1f, tx = 0f, ty = 0f;
                ta = ta + 2f;
                td = td + 2f;
                ty = ty - 0.15f;
                tc = tc + 0.3f;

                for (int i = 1; i < total; i++)
                {
                    rise = 100;
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    content.BeginText();
                    content.SetColorFill(iTextSharp.text.BaseColor.LIGHT_GRAY);
                    content.SetFontAndSize(font, 60);
                    content.SetTextRise(rise);
                    content.SetGState(gs);
                    //content.SetTextMatrix(ta, tb, tc, td, tx, ty);
                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_LEFT, waterMarkName, 300, 500, 45);


                    //content.SetTextRise(rise);

                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_CENTER, waterMarkName, 400, 400, 45);


                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_RIGHT, waterMarkName, 500, 300, 45);
                    //for (int k = 0; k < j; k++)
                    //{
                    //    content.SetTextRise(rise);
                    //    content.SetGState(gs);
                    //    c = waterMarkName[k];
                    //    content.ShowText(c + "");
                    //    rise -= 20;
                    //}
                    content.EndText();
                }

            }
            catch (Exception ex)
            {
                throw ex;
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
        /// 添加普通偏转角度文字水印
        /// </summary>
        /// <param name="inputfilepath"></param>
        /// <param name="outputfilepath"></param>
        /// <param name="waterMarkName"></param>
        /// <param name="permission"></param>
        public void PdfStampInExistingFileV5(string inputfilepath, string outputfilepath, string waterMarkName)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(inputfilepath);
                pdfStamper = new PdfStamper(pdfReader, new FileStream(outputfilepath, FileMode.Create));
                int total = pdfReader.NumberOfPages + 1;
                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);
                float width = psize.Width;
                float height = psize.Height;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\SIMFANG.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                float ta = 1f, tb = 0f, tc = 0f, td = 1f, tx = 0f, ty = 0f;
                ta = ta + 2f;
                td = td + 2f;
                ty = ty - 0.15f;
                tc = tc + 0.3f;
                for (int i = 1; i < total; i++)
                {
                    content = pdfStamper.GetOverContent(i);//在内容上方加水印
                                                           //content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                                                           //透明度
                    gs.FillOpacity = 0.2f;
                    content.SetGState(gs);
                    //content.SetGrayFill(0.3f);
                    //开始写入文本
                    content.BeginText();
                    content.SetColorFill(iTextSharp.text.BaseColor.LIGHT_GRAY);
                    content.SetFontAndSize(font, 60);
                    //content.SetTextMatrix(0, 0);
                    content.SetTextMatrix(ta, tb, tc, td, tx, ty);
                    content.ShowTextAligned(iTextSharp.text.Element.ALIGN_CENTER, waterMarkName, width / 2 - 50, height / 2 - 50, 55);
                    //content.SetColorFill(BaseColor.BLACK);
                    //content.SetFontAndSize(font, 8);
                    //content.ShowTextAligned(Element.ALIGN_CENTER, waterMarkName, 0, 0, 0);
                    content.EndText();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }

        public void AddWaterMark(MemoryStream ms, string watermarkText, MemoryStream outputStream)
        {
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            Graphics gr = Graphics.FromImage(img);
            Font font = new Font("Tahoma", (float)40);
            Color color = Color.FromArgb(50, 241, 235, 105);
            double tangent = (double)img.Height / (double)img.Width;
            double angle = Math.Atan(tangent) * (180 / Math.PI);
            double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;
            double sin, cos, opp1, adj1, opp2, adj2;

            for (int i = 100; i > 0; i--)
            {
                font = new Font("Tahoma", i, FontStyle.Bold);
                SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue);

                sin = Math.Sin(angle * (Math.PI / 180));
                cos = Math.Cos(angle * (Math.PI / 180));
                opp1 = sin * sizef.Width;
                adj1 = cos * sizef.Height;
                opp2 = sin * sizef.Height;
                adj2 = cos * sizef.Width;

                if (opp1 + adj1 < img.Height && opp2 + adj2 < img.Width)
                    break;
                //
            }

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.RotateTransform((float)angle);
            gr.DrawString(watermarkText, font, new SolidBrush(color), new Point((int)halfHypotenuse, 0), stringFormat);

            img.Save(outputStream, ImageFormat.Jpeg);
        }


        public void AddWaterMarkV2(string sourcefile, string watermarkText, string desfile)
        {
            Stream stream = new MemoryStream(System.IO.File.ReadAllBytes(sourcefile));
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            Graphics gr = Graphics.FromImage(img);
            //Color color = Color.FromArgb(255, 255, 0, 0);
            Font font = new Font("Tahoma", (float)40, FontStyle.Regular, GraphicsUnit.Pixel);
            Color color = Color.FromArgb(20, Color.Silver);//241, 235, 105
            double tangent = (double)img.Height / (double)img.Width;
            double angle = Math.Atan(tangent) * (180 / Math.PI);
            double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;
            double sin, cos, opp1, adj1, opp2, adj2;

            for (int i = 100; i > 0; i--)
            {
                font = new Font("Tahoma", i, FontStyle.Bold);
                SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue);

                sin = Math.Sin(angle * (Math.PI / 180));
                cos = Math.Cos(angle * (Math.PI / 180));
                opp1 = sin * sizef.Width;
                adj1 = cos * sizef.Height;
                opp2 = sin * sizef.Height;
                adj2 = cos * sizef.Width;

                if (opp1 + adj1 < img.Height && opp2 + adj2 < img.Width)
                    break;
                //
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
            catch (Exception ex)
            {

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


        public Stream AddWaterMarkV22(string sourcefile, string watermarkText, string desfile)
        {
            Stream stream = new MemoryStream(System.IO.File.ReadAllBytes(sourcefile));
            System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
            Graphics gr = Graphics.FromImage(img);
            //Color color = Color.FromArgb(255, 255, 0, 0);
            Font font = new Font("Tahoma", (float)40, FontStyle.Regular, GraphicsUnit.Pixel);
            Color color = Color.FromArgb(20, Color.Silver);//241, 235, 105
            double tangent = (double)img.Height / (double)img.Width;
            double angle = Math.Atan(tangent) * (180 / Math.PI);
            double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;
            double sin, cos, opp1, adj1, opp2, adj2;

            for (int i = 100; i > 0; i--)
            {
                font = new Font("Tahoma", i, FontStyle.Bold);
                SizeF sizef = gr.MeasureString(watermarkText, font, int.MaxValue);

                sin = Math.Sin(angle * (Math.PI / 180));
                cos = Math.Cos(angle * (Math.PI / 180));
                opp1 = sin * sizef.Width;
                adj1 = cos * sizef.Height;
                opp2 = sin * sizef.Height;
                adj2 = cos * sizef.Width;

                if (opp1 + adj1 < img.Height && opp2 + adj2 < img.Width)
                    break;
                //
            }
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.RotateTransform((float)angle);
            gr.DrawString(watermarkText, font, new SolidBrush(color), new Point((int)halfHypotenuse, 0), stringFormat);
            try
            {
                //img.Save(desfile, ImageFormat.Jpeg);     
                return img.ToStream(ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (img != null)
                {
                    img.Dispose();
                    img = null;
                }
            }
            return null;

        }


        public Stream WriteToPdf(string sourceFile, string stringToWriteToPdf)
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
                    pdfStamper = new PdfStamper(reader, memoryStream);
                    PdfContentByte content;

                    int rise = 100;
                    for (int i = 1; i <= reader.NumberOfPages; i++) // Must start at 1 because 0 is not an actual page.
                    {

                        iTextSharp.text.Rectangle pageSize = reader.GetPageSizeWithRotation(i);
                        float textAngle = (float)GetHypotenuseAngleInDegreesFrom(pageSize.Height, pageSize.Width);
                        PdfContentByte pdfPageContents = pdfStamper.GetUnderContent(i);
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


        public double GetHypotenuseAngleInDegreesFrom(double opposite, double adjacent)
        {
            //http://www.regentsprep.org/Regents/Math/rtritrig/LtrigA.htm
            // Tan <angle> = opposite/adjacent
            // Math.Atan2: http://msdn.microsoft.com/en-us/library/system.math.atan2(VS.80).aspx 

            double radians = Math.Atan2(opposite, adjacent); // Get Radians for Atan2
            double angle = radians * (180 / Math.PI); // Change back to degrees
            return angle;
        }
    }
    #endregion
}