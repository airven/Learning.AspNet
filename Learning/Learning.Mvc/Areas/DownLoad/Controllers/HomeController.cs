using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Learning.Mvc.Areas.DownLoad.Controllers
{
    public class HomeController : Controller
    {
        // GET: DownLoad/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DownLoad1()
        {
            //方法一
            var fileconent = GetFileContent();
            string fileName = string.Format("export{0}", DateTime.Now.ToString("yyyyMMddHHmmssff"));
            var bytes = Encoding.GetEncoding("utf-8").GetBytes(fileconent);
            return File(bytes, "application/ms-excel", "reflections" + DateTime.UtcNow.ToShortDateString() + ".xls");
        }

        public ActionResult DownLoad2()
        {
            //方法二
            var fileconent = GetFileContent();
            string fileName = string.Format("export{0}", DateTime.Now.ToString("yyyyMMddHHmmssff"));
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.ContentType = "application/ms-excel";
            Response.Write(fileconent);
            Response.End();
            return new EmptyResult();
        }

        private string GetFileContent()
        {
            var data = GetDataList();
            System.Text.StringBuilder sb = new StringBuilder();
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
            var packages = new Packages
            {
                PackageName = "jacktest",
                Products = projectList
            };
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
    }
}