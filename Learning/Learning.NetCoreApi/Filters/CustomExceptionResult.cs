using System;
using Learning.NetCoreApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace Learning.NetCoreApi.Filters
{
    public class CustomExceptionResult : ObjectResult
    {
        public CustomExceptionResult(int? code, Exception exception)
                : base(new CustomExceptionResultModel(code, exception))
        {
            StatusCode = code;
        }
    }
}