using System.Web.Mvc;

namespace Learning.Mvc.Areas.DownLoad
{
    public class DownLoadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DownLoad";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DownLoad_default",
                "DownLoad/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}