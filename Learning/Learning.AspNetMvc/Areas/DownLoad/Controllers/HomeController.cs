using Learning.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace Learning.AspNetMvc.Areas.DownLoad.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string path = Server.MapPath("~/ImgWatermark");
            string[] files = Directory.GetFiles(path);
            ViewBag.Files = files;
            return View();
        }

        public ActionResult DownLoad(string url)
        {
            var file = url;
            if(string.IsNullOrEmpty(file))
            {
                var fileconent = GetFileContent();
                string fileName = string.Format("export{0}", DateTime.Now.ToString("yyyyMMddHHmmssff"));
                //方法一
                //Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
                //Response.Charset = "utf-8";
                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                //Response.ContentType = "application/ms-excel";
                //Response.Write(fileconent);
                //Response.End();
                //return null;

                //方法二
                var bytes = Encoding.GetEncoding("utf-8").GetBytes(fileconent);
                return File(bytes, "application/ms-excel", "reflections" + DateTime.UtcNow.ToShortDateString() + ".xls");
            }
            else
            {
                Stream stream;
                var textMark = "测试文字";
                var fileExtension = Path.GetExtension(file).Substring(1);
                if (fileExtension == FileFormat.jpg.ToString() || fileExtension == FileFormat.jpeg.ToString())
                {
                    stream = WaterMarkerDownLoad.AddImgWaterMark(file, textMark);
                }
                else if (fileExtension == FileFormat.doc.ToString() || fileExtension == FileFormat.docx.ToString())
                {
                    stream = WaterMarkerDownLoad.AddWordWaterMarkV2(file, textMark);
                }
                else if (fileExtension == FileFormat.pdf.ToString())
                {
                    stream = WaterMarkerDownLoad.AddPdfWaterMark(file, textMark);
                }
                else
                {
                    stream = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(file));
                }
                return File(stream, "application/octet-stream", Path.GetFileName(file));
            }
        }

        [HttpGet]
        public ActionResult Upload(string imgName)
        {
            ViewBag.ImgName = imgName;
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileToUpload)
        {
            using (Image image = Image.FromStream(fileToUpload.InputStream, true, false))
            {
                string name = Path.GetFileNameWithoutExtension(fileToUpload.FileName);
                var ext = Path.GetExtension(fileToUpload.FileName);
                string myfile = name + ext;
                var saveImagePath = Path.Combine(Server.MapPath("~/ImgWatermark"), myfile);
                Image watermarkImage = Image.FromFile(Server.MapPath("/Img/watermarklogo.png"));
                WaterMarkerUpload objWatermarker = new WaterMarkerUpload(image);
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        // Set the properties for the logo
                        objWatermarker.Position = (Common.WatermarkPosition)WatermarkPosition.Absolute;
                        objWatermarker.PositionX = j;
                        objWatermarker.PositionY = i;
                        objWatermarker.Margin = new Padding(20);
                        objWatermarker.Opacity = 0.5f;
                        objWatermarker.TransparentColor = Color.White;
                        objWatermarker.ScaleRatio = 3;
                        // Draw the logo
                        objWatermarker.DrawImage(watermarkImage);
                        //Draw the Text
                        //objWatermarker.DrawText("WaterMarkDemo")

                        j = j + 400;// watermark image width 
                    }
                    i = i + 120;//
                }
                objWatermarker.Image.Save(saveImagePath);
                return RedirectToAction("Upload", new { imgName = myfile });
            }
        }

        private string GetFileContent()
        {
            var data = GetDataList();
            StringBuilder sb = new StringBuilder();
            sb.Append("<table border='1'>");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<th>链接ID</th>");
            sb.Append("<th>链接名称</th>");
            sb.Append("<th>产品名称</th>");
            sb.Append("<th>酒景名称</th>");
            sb.Append("<th>省份</th>");
            sb.Append("<th>审核状态</th>");
            sb.Append("<th>上架时间</th>");
            sb.Append("<th>下架时间</th>");
            sb.Append("<th>归属人</th>");
            sb.Append("<th>流量电话</th>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            foreach (var item in data)
            {
                int productCount = item.Packages.Sum(m => { return m.Products.Count; });
                sb.AppendFormat("<tr>");
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.ChannelLinkId, productCount);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.ChannelLinkName, productCount);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.Packages[0].PackageName, item.Packages[0].Products.Count);
                sb.AppendFormat("<td>{0}</td>", item.Packages[0].Products[0].ProductName);
                sb.AppendFormat("<td>{0}</td>", item.Packages[0].Products[0].ProvinceName);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.AuditStatus, productCount);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.StartDate, productCount);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.EndDate, productCount);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.BDName, productCount);
                sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", item.BDTel, productCount);
                sb.AppendFormat("</tr>");
                for (int i = 0; i < item.Packages.Count; i++)
                {
                    var p = item.Packages[i];
                    if (i == 0)
                    {
                        if (p.Products.Count > 1)
                        {
                            for (int k = 1; k < p.Products.Count; k++)
                            {
                                sb.AppendFormat("<tr>");
                                sb.AppendFormat("<td>{0}</td>", p.Products[k].ProductName);
                                sb.AppendFormat("<td>{0}</td>", p.Products[k].ProvinceName);
                                sb.AppendFormat("</tr>");
                            }
                        }
                    }
                    else
                    {
                        sb.AppendFormat("<tr>");
                        sb.AppendFormat("<td rowspan=\"{1}\">{0}</td>", p.PackageName, p.Products.Count);
                        if (p.Products.Count > 0)
                        {
                            sb.AppendFormat("<td>{0}</td>", p.Products[0].ProductName);
                            sb.AppendFormat("<td>{0}</td>", p.Products[0].ProvinceName);
                            for (int k = 1; k < p.Products.Count; k++)
                            {
                                sb.AppendFormat("<tr>");
                                sb.AppendFormat("<td>{0}</td>", p.Products[k].ProductName);
                                sb.AppendFormat("<td>{0}</td>", p.Products[k].ProvinceName);
                                sb.AppendFormat("</tr>");
                            }
                        }
                        sb.AppendFormat("</tr>");
                    }
                }
            }
            sb.AppendFormat("</table>");
            return sb.ToString();
        }
        private IEnumerable<ProductCollection> GetDataList()
        {
            var factory = new ProductCollection();
            var projectList = new List<Products>()
            {
                new Products{ProductName="prodct1",ProvinceName="beijing" },
                new Products{ProductName="prodct2",ProvinceName="sandong" },
                new Products{ProductName="prodct3",ProvinceName="shanghai" },
                new Products{ProductName="prodct4",ProvinceName="kongkong" },
                new Products{ProductName="prodct5",ProvinceName="suzhou" },
            };
            var packages = new Packages();
            packages.PackageName = "jacktest";
            packages.Products = projectList;
            factory.StartDate = DateTime.Now;
            factory.EndDate = DateTime.Now;
            factory.Packages = new List<Packages> { packages };
            factory.ChannelLinkId = 1;
            factory.BDTel = "123456789";
            factory.BDName = "lunfactory";
            factory.AuditStatus = 1;

            var factoryList = new List<ProductCollection> { factory, factory, factory, factory, factory };
            return factoryList;
        }
    }


    class ProductCollection
    {
        public int ChannelLinkId { get; set; }
        public string ChannelLinkName { get; set; }

        public List<Packages> Packages { get; set; }

        public int AuditStatus { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

        public string BDName { get; set; }

        public string BDTel { get; set; }
    }

    class Packages
    {
        public string PackageName { get; set; }
        public List<Products> Products { get; set; }
    }

    class Products
    {
        public string ProductName { get; set; }
        public string ProvinceName { get; set; }
    }

    enum FileFormat
    {
        pdf,
        doc,
        docx,
        txt,
        xls,
        xlsx,

        dwg,

        jpg,
        jpeg,
        png,
        tif,

        mpg,
        mpeg,
        mp4,
        mkv,
        avi,
        ogg,
        webm,
        mp3,

        obj,
        html,
    }
}