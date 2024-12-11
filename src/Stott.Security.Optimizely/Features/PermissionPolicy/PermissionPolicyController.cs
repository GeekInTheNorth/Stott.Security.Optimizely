using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/permission-policy/[action]")]
public sealed class PermissionPolicyController : BaseController
{
    private readonly IPermissionPolicyService _permissionPolicyService;

    public PermissionPolicyController(IPermissionPolicyService permissionPolicyService)
    {
        _permissionPolicyService = permissionPolicyService;
    }

    [HttpGet]
    public IActionResult List(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        var allItems = _permissionPolicyService.GetAll(sourceFilter, enabledFilter);

        return CreateSuccessJson(allItems);
    }
}