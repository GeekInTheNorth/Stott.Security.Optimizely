using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.SecurityTxt.Service;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class SecurityTxtApiController : BaseController
{
    private readonly ISecurityTxtContentService _service;

    private readonly ILogger<SecurityTxtApiController> _logger;

    public SecurityTxtApiController(
        ISecurityTxtContentService service, 
        ILogger<SecurityTxtApiController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/securitytxt/list/")]
    public IActionResult ApiList()
    {
        var model = new SecurityTxtListViewModel
        {
            List = _service.GetAll()
        };

        return CreateSuccessJson(model);
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/securitytxt/[action]")]
    public IActionResult Get(string id)
    {
        if (!Guid.TryParse(id, out var parsedId))
        {
            throw new ArgumentException("Id cannot be parsed as a valid GUID.", nameof(id));
        }

        var model = _service.Get(parsedId);
        if (model is null)
        {
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Content = "Security.txt configuration not found.",
                ContentType = "text/plain"
            };
        }

        return CreateSuccessJson(model);
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/securitytxt/[action]")]
    public async Task<IActionResult> Save(SaveSecurityTxtModel formSubmitModel)
    {
        try
        {
            if (_service.DoesConflictExists(formSubmitModel))
            {
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Content = "A Security.txt configuration already exists for this site and host combination.",
                    ContentType = "text/plain"
                };
            }
            
            await _service.SaveAsync(formSubmitModel, User.Identity?.Name);

            return new OkResult();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to save security.txt content for {siteName}", formSubmitModel.SiteName);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = exception.Message,
                ContentType = "text/plain"
            };
        }
    }

    [HttpDelete]
    [Route("/stott.security.optimizely/api/securitytxt/[action]/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            if (Guid.Empty.Equals(id))
            {
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = "Id must not be empty.",
                    ContentType = "text/plain"
                };
            }

            await _service.DeleteAsync(id, User.Identity?.Name);

            return new OkResult();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to delete this security.txt configuration.");
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Content = exception.Message,
                ContentType = "text/plain"
            };
        }
    }
}