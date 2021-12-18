using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Features.Permissions.List;

namespace Stott.Optimizely.Csp.Features.Permissions
{
    public class CspPermissionsController : Controller
    {
        private readonly ICspPermissionsViewModelBuilder _viewModelBuilder;

        public CspPermissionsController(ICspPermissionsViewModelBuilder viewModelBuilder)
        {
            _viewModelBuilder = viewModelBuilder;
        }

        [HttpGet]
        [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            var model = _viewModelBuilder.Build();

            return View(model);
        }
    }
}
