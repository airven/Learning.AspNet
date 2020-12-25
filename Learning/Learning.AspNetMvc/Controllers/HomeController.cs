using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learning.AspNetMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Index()
        {
            //return  IndexWord();
            return IndexPdf();
            //var path1 = Server.MapPath("~") + "redis.pdf";
            //var path2 = Server.MapPath("~") + "123.png";
            //var path3 = Server.MapPath("~") + "xxxxx.pdf";
            //WaterHelper helper = new WaterHelper(path2);
            //helper.Position = WatermarkPosition.TopRight;
            //helper.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 10);
            ////helper.PdfStampInExistingFileV3("南海子数字博物馆", path1, path1);
            ////helper.PdfStampInExistingFileV4(path1, path3, "南海子数字博物馆", "南海子数字博物馆", "",1);

            ////helper.PdfStampInExistingFileV5(path1, path3, "南海子数字博物馆");

            //helper.PdfStampInExistingFileV4(path1, path1, "南海子数字博物馆", "", "", 0);
            //return File(path3, "application/pdf");
            //return View();
        }

        [HttpGet]
        public ActionResult IndexPdf()
        {
            var path1 = Server.MapPath("~") + "redis.pdf";
            var path2 = Server.MapPath("~") + "123.png";
            var path3 = Server.MapPath("~") + "xxxxx.pdf";
            WaterHelper helper = new WaterHelper(path2);
            //helper.Position = WatermarkPosition.TopRight;
            //helper.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 10);
            var result = helper.WriteToPdf(path1, "南海子数字博物馆");
            return File(result, "application/pdf");
        }


        [HttpGet]
        public ActionResult IndexImg()
        {
            var path1 = Server.MapPath("~") + "hao123.jpg";
            var path2 = Server.MapPath("~") + "123.png";
            var path3 = Server.MapPath("~") + "hao123x.jpg";
            WaterHelper helper = new WaterHelper(path2);
            helper.Position = WatermarkPosition.TopRight;
            helper.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, 20);

            var streamResult = helper.AddWaterMarkV22(path1, "南海子数字博物馆", path1);
            return File(streamResult, "image/jpeg");
            //return View();
        }

        [HttpGet]
        public ActionResult IndexWord()
        {
            var path1 = Server.MapPath("~") + "hao123.doc";
            var path2 = Server.MapPath("~") + "123.png";
            var path3 = Server.MapPath("~") + "hao123xxx.doc";


            //Document doc = new Document();
            //doc.LoadFromFile(path1);

            //TextWatermark txtWatermark = new TextWatermark();
            //txtWatermark.Text = "南海子数字博物馆";
            //txtWatermark.FontSize = 60;
            //txtWatermark.Color = Color.DimGray;
            //txtWatermark.Layout = WatermarkLayout.Diagonal;
            //doc.Watermark = txtWatermark;
            //doc.SaveToFile(path3);

            //string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            //string dataDir = new Uri(new Uri(exeDir), @"../../Data/").LocalPath;


            Document doc = new Document(path1);
            InsertWatermarkText(doc, "南海子数字博物馆");
            doc.Save(path3);
            return File(path3, "application/msword");
            //Document doc = new Document(path1);
            //WaterMarkMore(doc, "南海子数字博物馆");
            //doc.Save(path3);
            //return File(path3, "application/msword");
        }

        private void InsertWatermarkText(Document doc, string watermarkText)
        {
            // Create a watermark shape. This will be a WordArt shape.
            // You are free to try other shape types as watermarks.
            Shape watermark = new Shape(doc, ShapeType.TextPlainText);
            // Set up the text of the watermark.
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;
            watermark.Top = 50;
            watermark.Left = 50;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color = Color.Gray; // Try LightGray to get more Word-style watermark
            watermark.Fill.Opacity = 0.1;
            watermark.StrokeColor = Color.Gray; // Try LightGray to get more Word-style watermark
            watermark.Stroke.Opacity = 0.1;

            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark.WrapType = WrapType.None;
            watermark.VerticalAlignment = VerticalAlignment.Top;
            watermark.HorizontalAlignment = HorizontalAlignment.Left;



            Shape watermark2 = new Shape(doc, ShapeType.TextPlainText);
            // Set up the text of the watermark.
            watermark2.TextPath.Text = watermarkText;
            watermark2.TextPath.FontFamily = "Arial";
            watermark2.Width = 500;
            watermark2.Height = 100;
            watermark2.Top = 100;
            watermark2.Left = 100;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark2.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark2.Fill.Color = Color.Gray; // Try LightGray to get more Word-style watermark
            watermark2.Fill.Opacity = 0.1;
            watermark2.StrokeColor = Color.Gray; // Try LightGray to get more Word-style watermark
            watermark2.Stroke.Opacity = 0.1;

            // Place the watermark in the page center.
            watermark2.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark2.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark2.WrapType = WrapType.None;
            watermark2.VerticalAlignment = VerticalAlignment.Center;
            watermark2.HorizontalAlignment = HorizontalAlignment.Center;


            Shape watermark3 = new Shape(doc, ShapeType.TextPlainText);
            // Set up the text of the watermark.
            watermark3.TextPath.Text = watermarkText;
            watermark3.TextPath.FontFamily = "Arial";
            watermark3.Width = 500;
            watermark3.Height = 100;
            watermark3.Top = 100;
            watermark3.Left = 100;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark3.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark3.Fill.Color = Color.Gray; // Try LightGray to get more Word-style watermark
            watermark3.Fill.Opacity = 0.1;
            watermark3.StrokeColor = Color.Gray; // Try LightGray to get more Word-style watermark
            watermark3.Stroke.Opacity = 0.1;

            // Place the watermark in the page center.
            watermark3.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            watermark3.RelativeVerticalPosition = RelativeVerticalPosition.Page;
            watermark3.WrapType = WrapType.None;
            watermark3.VerticalAlignment = VerticalAlignment.Bottom;
            watermark3.HorizontalAlignment = HorizontalAlignment.Right;

            // Create a new paragraph and append the watermark to this paragraph.
            Paragraph watermarkPara = new Paragraph(doc);
            watermarkPara.AppendChild(watermark);
            watermarkPara.AppendChild(watermark2);
            watermarkPara.AppendChild(watermark3);

            // Insert the watermark into all headers of each document section.
            foreach (Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // the watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        private void InsertWatermarkIntoHeader(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
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



        /// <summary>
        /// 插入多个水印
        /// </summary>
        /// <param name="mdoc">Document</param>
        /// <param name="wmText">水印文字名</param>
        /// <param name="left">左边距多少</param>
        /// <param name="top">上边距多少</param>
        /// <returns></returns>
        public static Shape ShapeMore(Document mdoc, string wmText, double left, double top)
        {
            Shape waterShape = new Shape(mdoc, ShapeType.TextPlainText);
            //设置该文本的水印
            waterShape.TextPath.Text = wmText;
            waterShape.TextPath.FontFamily = "宋体";
            waterShape.Width = 50;
            waterShape.Height = 50;
            //文本将从左下角到右上角。
            waterShape.Rotation = 0;
            //绘制水印颜色
            waterShape.Fill.Color = System.Drawing.Color.Gray;//浅灰色水印
            waterShape.StrokeColor = System.Drawing.Color.Gray;
            //将水印放置在页面中心
            waterShape.Left = left;
            waterShape.Top = top;
            waterShape.WrapType = WrapType.None;
            return waterShape;
        }

        /// <summary>
        /// 插入多个水印
        /// </summary>
        /// <param name="mdoc">Document</param>
        /// <param name="wmText">水印文字名</param>
        public void WaterMarkMore(Document mdoc, string wmText)
        {
            Paragraph watermarkPara = new Paragraph(mdoc);
            for (int j = 20; j < 500; j = j + 100)
            {
                for (int i = 20; i < 700; i = i + 50)
                {
                    Shape waterShape = ShapeMore(mdoc, wmText, j, i);
                    watermarkPara.AppendChild(waterShape);
                }
            }

            // 在每个部分中，最多可以有三个不同的标题，因为我们想要出现在所有页面上的水印，插入到所有标题中。  
            foreach (Section sect in mdoc.Sections)
            {
                // 每个区段可能有多达三个不同的标题，因为我们希望所有页面上都有水印，将所有的头插入。
                InsertWatermarkIntoHeader2(watermarkPara, sect, HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader2(watermarkPara, sect, HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader2(watermarkPara, sect, HeaderFooterType.HeaderEven);
            }
        }

        private void InsertWatermarkIntoHeader2(Paragraph watermarkPara, Section sect, HeaderFooterType headerType)
        {
            HeaderFooter header = sect.HeadersFooters[headerType];
            if (header == null)
            {
                // 当前节中没有指定类型的头，创建它
                header = new HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }
            // 在头部插入一个水印的克隆
            header.AppendChild(watermarkPara.Clone(true));
        }

    }
}