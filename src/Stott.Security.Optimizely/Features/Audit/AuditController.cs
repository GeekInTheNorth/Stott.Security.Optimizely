namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class AuditController : BaseController
{
    private readonly IAuditRepository _repository;

    private readonly ILogger<AuditController> _logger;

    public AuditController(
        IAuditRepository repository, 
        ILogger<AuditController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> List([FromQuery] AuditRequestModel requestModel)
    {
        try
        {
            var from = requestModel.From ?? DateTime.Today.AddDays(-7);
            var to = requestModel.To ?? DateTime.Today.AddDays(1).AddMinutes(-1);
            var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
            
            var model = await _repository.GetAsync(from, to, requestModel.ActionedBy, requestModel.RecordType, requestModel.OperationType);

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve audit log.");
            throw;
        }
    }

    [HttpGet]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Users()
    {
        try
        {
            var model = await _repository.GetUsersAsync();

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve audit log.");
            throw;
        }
    }
}