namespace OptimizelyTwelveTest.Features.API;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

[EnableCors("TEST-POLICY")]
public sealed class TestApiController : Controller
{
    [HttpGet]
    [Route("/api/test/list")]
    public IActionResult List([FromQuery]string nameToReturn)
    {
        var list = new List<string> { "Foo", "Bar", nameToReturn };

        return Json(list.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}