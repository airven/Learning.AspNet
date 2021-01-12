﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learning.AspNetMvc.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RedirectImageFile(string catetory, string filename)
        {
            //string url = Url.Content("~/Files/Appendix/" + catetory + "/" + filename + ".jpg");
            //return base.File(url, "image/jpeg");
            //return base.File(url, "application/octet-stream");

            try
            {
                var response = HttpContext.Response;
                var request = HttpContext.Request;
                string fileExtension = Path.GetExtension(request.Url.ToString().Split('?')[0]);
                var path = HttpContext.Server.MapPath("~/Files/Appendix/" + catetory + "/" + filename + fileExtension);
                response.Clear();
                response.ContentType = GetContentType(request.Url.ToString());

                if (fileExtension == ".jpg")
                {
                    Stream stream = new MemoryStream(System.IO.File.ReadAllBytes(path));
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    Graphics gr = Graphics.FromImage(img);
                    //Color color = Color.FromArgb(255, 255, 0, 0);
                    Font font = new Font("Tahoma", (float)40, FontStyle.Regular, GraphicsUnit.Pixel);
                    Color color = Color.FromArgb(30, Color.Gray);//241, 235, 105
                    double tangent = (double)img.Height / (double)img.Width;
                    double angle = Math.Atan(tangent) * (180 / Math.PI);
                    double halfHypotenuse = Math.Sqrt((img.Height * img.Height) + (img.Width * img.Width)) / 2;
                    double sin, cos, opp1, adj1, opp2, adj2;

                    for (int i = 100; i > 0; i--)
                    {
                        font = new Font("Tahoma", i, FontStyle.Bold);
                        SizeF sizef = gr.MeasureString("南海子数字博物馆", font, int.MaxValue);

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
                    gr.DrawString("南海子数字博物馆", font, new SolidBrush(color), new Point((int)halfHypotenuse, 0), stringFormat);
                    try
                    {
                        img.Save(response.OutputStream, ImageFormat.Jpeg);
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
                        }
                    }
                }
                return new EmptyResult();
            }
            catch(Exception ex)
            {
                return new EmptyResult();
            }
        }
        private static string GetContentType(string url)
        {
            switch (Path.GetExtension(url))
            {
                case ".gif":
                    return "Image/gif";
                case ".jpg":
                    return "Image/jpeg";
                case ".png":
                    return "Image/png";
                default:
                    break;
            }
            return null;
        }
    }
}