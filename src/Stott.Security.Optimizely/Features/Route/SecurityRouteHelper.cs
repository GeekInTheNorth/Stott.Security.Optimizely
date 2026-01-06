using System.Collections.Generic;

using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Route;

public sealed class SecurityRouteHelper : ISecurityRouteHelper
{
    private readonly IList<string> _exclusionPaths;

    private SecurityRouteType currentRoute;

    public SecurityRouteHelper(IList<string>? exclusionPaths)
    {
        _exclusionPaths = exclusionPaths ?? new List<string>(0);
    }

    public SecurityRouteType GetRouteType()
    {
        if (currentRoute == SecurityRouteType.Unknown)
        {
            currentRoute = DetermineSecurityRoute();
        }

        return currentRoute;
    }

    private SecurityRouteType DetermineSecurityRoute()
    {
        if (_exclusionPaths is { Count: > 0 })
        {
            var contextAccessor = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();
            var context = contextAccessor.HttpContext;
            if (context?.Request?.Path is not null)
            {
                foreach (var exclusionPath in _exclusionPaths)
                {
                    if (context.Request.Path.StartsWithSegments(exclusionPath))
                    {
                        return SecurityRouteType.NoNonceOrHash;
                    }
                }
            }
        }

        var pageRouteHelper = ServiceLocator.Current.GetInstance<IPageRouteHelper>();
        if (pageRouteHelper.Content is IContentSecurityPolicyPage { ContentSecurityPolicySources.Count: > 0 })
        {
            return SecurityRouteType.ContentSpecific;
        }

        return SecurityRouteType.Default;
    }
}