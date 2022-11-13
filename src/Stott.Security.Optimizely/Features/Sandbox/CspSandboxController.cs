namespace Stott.Security.Optimizely.Features.Sandbox;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Sandbox.Repository;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class CspSandboxController : BaseController
{
    private readonly ICspSandboxRepository _repository;

    private readonly ILogger<CspSandboxController> _logger;

    public CspSandboxController(
        ICspSandboxRepository repository,
        ILogger<CspSandboxController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var model = await _repository.Get();

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP sandbox settings.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Save(SandboxModel model)
    {
        try
        {
            await _repository.Save(model);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP sandbox settings.");
            throw;
        }
    }
}