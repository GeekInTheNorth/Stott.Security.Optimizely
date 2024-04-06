namespace Stott.Security.Optimizely.Common;

using System.IO;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common.Validation;

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

    protected async Task<string> GetBody()
    {
        try
        {
            var bodyStream = Request.Body;
            if (bodyStream.CanSeek)
            {
                bodyStream.Seek(0, SeekOrigin.Begin);
            }

            using var streamReader = new StreamReader(Request.Body);

            var content = await streamReader.ReadToEndAsync();

            return content;
        }
        catch (Exception)
        {
            return string.Empty;
        }
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