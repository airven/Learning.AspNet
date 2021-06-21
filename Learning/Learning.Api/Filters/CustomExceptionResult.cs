using System;
using Learning.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Learning.Api.Filters
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