using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common.Validation;

namespace Stott.Security.Optimizely.Common
{
    public class BaseController : Controller
    {
        protected static IActionResult CreateSuccessJson<T>(T objectToSerialize)
        {
            return CreateActionResult(HttpStatusCode.OK, objectToSerialize);
        }

        protected static IActionResult CreateValidationErrorJson(ValidationModel validationModel)
        {
            return CreateActionResult(HttpStatusCode.BadRequest, validationModel);
        }

        private static IActionResult CreateActionResult<T>(HttpStatusCode statusCode, T objectToSerialize)
        {
            var serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var content = JsonSerializer.Serialize(objectToSerialize, serializationOptions);

            return new ContentResult
            {
                StatusCode = (int)statusCode,
                ContentType = "application/json",
                Content = content
            };
        }
    }
}
