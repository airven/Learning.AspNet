using Learning.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Learning.Api.Filters
{
    public class ValidationFailedResult : ObjectResult
    {

        public ValidationFailedResult(ModelStateDictionary modelState)
              : base(new ValidationFailedResultModel(modelState))
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }
    }
}