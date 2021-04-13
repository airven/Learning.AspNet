using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Learning.AspNetMvc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);


            routes.RouteExistingFiles = true;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapMvcAttributeRoutes();
            //routes.MapRoute("DiskFile", "img/hao123.jpg", null);
            //routes.Add("ImagesRoute", new Route("img/{filename}.jpg", new ImageRouteHandler()));

            //Route NewRoute = new Route("{folder}/{subfolder}/{catetory}/{filename}", new ImageRouteHandler());
            //NewRoute.Defaults = null;
            //NewRoute.Constraints = new RouteValueDictionary { { "folder", "Files" }, { "subfolder", "Appendix" } };
            //routes.Add("ImagesRoute", NewRoute);

            //routes.Add("ImagesRoute", new Route(
            //    "{folder}/{subfolder}/{catetory}/{filename}",
            //    null,
            //    new RouteValueDictionary { { "folder", "Files" }, { "subfolder", "Appendix" } },
            //    new ImageRouteHandler()
            //    ));

            routes.MapRoute(
            name: "ImgRoute",
            url: "Files/Appendix/{catetory}/{filename}.jpg",
            defaults: new { controller = "Home", action = "RedirectImageFile" },
            namespaces: new[] { "Learning.AspNetMvc.Controllers" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Learning.AspNetMvc.Controllers" }
            );
        }
    }

    public class ImageRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ImageHandler(requestContext);
        }
    }

    public class ImageHandler : IHttpHandler
    {
        public ImageHandler(RequestContext context)
        {
            ProcessRequest(context);
        }

        private void ProcessRequest(RequestContext requestContext)
        {
            var response = requestContext.HttpContext.Response;
            var request = requestContext.HttpContext.Request;
            var server = requestContext.HttpContext.Server;
            var validRequestFile = requestContext.RouteData.Values["filename"].ToString();
            var catetory = requestContext.RouteData.Values["catetory"].ToString();
            var path1 = server.MapPath("~/Files/Appendix/" + catetory + "/");

            response.Clear();
            response.ContentType = GetContentType(request.Url.ToString());
            var path = path1 + validRequestFile;
            string fileExtension = Path.GetExtension(request.Url.ToString()).Substring(1);
            if (fileExtension == "jpg")
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
                StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

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
            response.End();
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

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }


        public bool IsReusable
        {
            get { return false; }
        }
    }
}
