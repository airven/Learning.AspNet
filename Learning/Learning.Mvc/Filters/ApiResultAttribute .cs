using Learning.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace Learning.Mvc.Filters
{
    public class ApiResultAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // 若发生例外则不在这边处理
            if (actionExecutedContext.Exception != null)
                return;

            base.OnActionExecuted(actionExecutedContext);
            ApiResultModel result = new ApiResultModel();

            // 取得由 API 返回的状态代码
            result.Status = actionExecutedContext.ActionContext.Response.StatusCode;
            // 取得由 API 返回的资料
            result.Data = actionExecutedContext.ActionContext.Response.Content.ReadAsAsync<object>().Result;
            // 重新封装回传格式
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.Status, result);
        }
    }
}