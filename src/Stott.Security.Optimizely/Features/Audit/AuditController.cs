namespace Stott.Security.Optimizely.Features.Audit;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Audit.Models;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class AuditController : BaseController
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
    public async Task<IActionResult> List([FromQuery] AuditRequestModel requestModel)
    {
        try
        {
            var dateFrom = requestModel.DateFrom ?? DateTime.Today;
            var dateTo = requestModel.DateTo ?? DateTime.Today;
            var from = requestModel.From < 0 ? 0 : requestModel.From;
            var take = requestModel.Take <= 0 ? 10 : requestModel.Take;
            
            var model = await _repository.GetAsync(
                dateFrom,
                dateTo,
                requestModel.ActionedBy,
                requestModel.RecordType,
                requestModel.OperationType,
                from,
                take);

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve audit log.");
            throw;
        }
    }

    [HttpGet]
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