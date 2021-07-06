using System;
using System.Linq;
using Learning.NetCoreApi.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Learning.NetCoreApi.Filters
{
    public class ValidationFailedResultModel : BaseResultModel
    {
        public ValidationFailedResultModel(ModelStateDictionary modelState)
        {
            Code = 422;
            Message = "参数不合法";
            Result = modelState.Keys
                        .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                        .ToList();
            ReturnStatus = ReturnStatus.Fail;
        }
    }

    public class ValidationError
    {
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }
        public string Message { get; }
        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }
    }
}