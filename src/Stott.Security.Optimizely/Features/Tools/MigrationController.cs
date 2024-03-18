﻿using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Tools;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class MigrationController : BaseController
{
    private readonly IMigrationService _migrationService;

    private readonly ILogger<MigrationController> _logger;

    public MigrationController(IMigrationService migrationService, ILogger<MigrationController> logger)
    {
        _migrationService = migrationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Export()
    {
        try
        {
            var exportModel = await _migrationService.Export();

            return CreateSuccessJson(exportModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }
}