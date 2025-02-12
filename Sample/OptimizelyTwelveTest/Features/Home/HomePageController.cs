namespace OptimizelyTwelveTest.Features.Home
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using OptimizelyTwelveTest.Features.Common;
    using OptimizelyTwelveTest.Features.Security;

    using Stott.Security.Optimizely.Features.Csp.Settings.Service;

    public class HomePageController : PageControllerBase<HomePage>
    {
        private readonly ICspSettingsService _cspSettingsService;

        public HomePageController(ICspSettingsService cspSettingsService)
        {
            _cspSettingsService = cspSettingsService;
        }

        [SecurityHeaderAction]
        public async Task<IActionResult> Index(HomePage currentPage, bool resetReportMode)
        {
            var model = new HomePageViewModel { CurrentPage = currentPage };

            if (resetReportMode)
            {
                var currentSettings = await _cspSettingsService.GetAsync();
                currentSettings.IsReportOnly = true;
                await _cspSettingsService.SaveAsync(currentSettings, "Mark Stott");
            }

            return View(model);
        }
    }
}
