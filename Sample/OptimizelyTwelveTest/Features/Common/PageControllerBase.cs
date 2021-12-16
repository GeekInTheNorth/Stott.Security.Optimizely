namespace OptimizelyTwelveTest.Features.Common
{
    using EPiServer.Core;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell.Security;
    using EPiServer.Web.Mvc;
    using EPiServer.Web.Routing;

    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    public abstract class PageControllerBase<T> : PageController<T>
        where T : PageData
    {
        protected Injected<UISignInManager> UiSignInManager;
        protected Injected<UrlResolver> UrlResolver;

        /// <summary>
        /// Signs out the current user and redirects to the Index action of the same controller.
        /// </summary>
        /// <remarks>
        /// There's a log out link in the footer which should redirect the user to the same page.
        /// As we don't have a specific user/account/login controller but rely on the login URL for
        /// forms authentication for login functionality we add an action for logging out to all
        /// controllers inheriting from this class.
        /// </remarks>
        public async Task<IActionResult> Logout()
        {
            await UiSignInManager.Service.SignOutAsync();
            return Redirect(UrlResolver.Service.GetUrl(PageContext.ContentLink, PageContext.LanguageID));
        }
    }
}
