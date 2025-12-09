using System;
using System.Net;

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
    public IActionResult Get(string id, string siteId)
    {
        if (!Guid.TryParse(id, out var parsedId))
        {
            throw new ArgumentException("Id cannot be parsed as a valid GUID.", nameof(id));
        }

        if (!Guid.TryParse(siteId, out var parsedSiteId) || Guid.Empty.Equals(parsedSiteId))
        {
            throw new ArgumentException("SiteId cannot be parsed as a valid GUID.", nameof(siteId));
        }

        var model = Guid.Empty.Equals(parsedId) ? _service.GetDefault(parsedSiteId) : _service.Get(parsedId);

        return CreateSuccessJson(model);
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/securitytxt/[action]")]
    public IActionResult Save(SaveSecurityTxtModel formSubmitModel)
    {
        try
        {
            if (_service.DoesConflictExists(formSubmitModel))
            {
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Content = "An Security.txt configuration already exists for this site and host combination.",
                    ContentType = "text/plain"
                };
            }
            _service.Save(formSubmitModel, User.Identity?.Name);

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
    public IActionResult Delete(Guid id)
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

            _service.Delete(id, User.Identity?.Name);

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