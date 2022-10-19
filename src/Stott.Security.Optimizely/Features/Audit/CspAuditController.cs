namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class CspAuditController : BaseController
{
    private readonly IAuditRepository _repository;

    private readonly ILogger<CspAuditController> _logger;

    public CspAuditController(
        IAuditRepository repository, 
        ILogger<CspAuditController> logger)
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
            
            var model = await _repository.Get(from, to, requestModel.ActionedBy, requestModel.RecordType, requestModel.OperationType);

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve audit log.");
            throw;
        }
    }
}