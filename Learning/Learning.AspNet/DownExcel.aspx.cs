using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Learning.AspNet
{
    public partial class DownExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Response.Write("这是一个测试");
            }
        }

        private IEnumerable<ProductFactory> GetDataList()
        {
            var factory = new ProductFactory();
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

            var factoryList = new List<ProductFactory> { factory, factory, factory, factory, factory };
            return factoryList;
        }

        protected void btndownload_Click(object sender, EventArgs e)
        {
            var data = GetDataList();
            string fileName = string.Format("export{0}", DateTime.Now.ToString("yyyyMMddHHmmssff"));
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            //Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.ContentType = "application/ms-excel";
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
            Response.Write(sb);
            Response.End();
        }
    }

    class ProductFactory
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

    public class Products
    {
        public string ProductName { get; set; }
        public string ProvinceName { get; set; }
    }
}