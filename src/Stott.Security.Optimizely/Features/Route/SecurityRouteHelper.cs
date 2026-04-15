using System;

using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Configuration;
using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Route;

/// <inheritdoc cref="ISecurityRouteHelper"/>
public sealed class SecurityRouteHelper : ISecurityRouteHelper
{
    private readonly IPageRouteHelper _pageRouteHelper;

    private readonly IContentLoader _contentLoader;

    private readonly IUrlResolver _urlResolver;

    private readonly IHttpContextAccessor _contextAccessor;

    private readonly ISiteDefinitionResolver _siteDefinitionResolver;

    private readonly SecurityConfiguration _configuration;

    private SecurityRouteData? _currentData;

    public SecurityRouteHelper(
        IPageRouteHelper pageRouteHelper,
        IContentLoader contentLoader,
        IUrlResolver urlResolver,
        IHttpContextAccessor contextAccessor,
        ISiteDefinitionResolver siteDefinitionResolver,
        SecurityConfiguration configuration)
    {
        _pageRouteHelper = pageRouteHelper;
        _contentLoader = contentLoader;
        _urlResolver = urlResolver;
        _contextAccessor = contextAccessor;
        _siteDefinitionResolver = siteDefinitionResolver;
        _configuration = configuration;
    }

    public SecurityRouteData GetRouteData()
    {
        if (_currentData is not null)
        {
            return _currentData;
        }

        var path = _contextAccessor.HttpContext?.Request?.Path ?? new PathString(string.Empty);
        _currentData = IsHeadersApiRequest(path) ? GetDataForPreviewApi() : GetDataForRequest(path);

        return _currentData;
    }

    /// <summary>
    /// Gets data for a standard headed request.
    /// </summary>
    private SecurityRouteData GetDataForRequest(PathString path)
    {
        var content = _pageRouteHelper.Content;
        var routeData = new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(path, content)
        };

        return AssignSiteIdAndHostBasedOnRequest(routeData);
    }

    /// <summary>
    /// Gets data for a compiled headers api request. Supports explicit site/host overrides
    /// via query parameters so the preview page can render headers for any configured context.
    /// </summary>
    private SecurityRouteData GetDataForPreviewApi()
    {
        var content = GetContentFromQuery();
        var url = _urlResolver.GetUrl(content) ?? string.Empty;
        var routeData = new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(new PathString(url), content)
        };

        if (TryGetQueryValue("siteId", out var siteIdValue) &&
            Guid.TryParse(siteIdValue, out var explicitSiteId))
        {
            routeData.SiteId = explicitSiteId == Guid.Empty ? null : explicitSiteId;
            if (TryGetQueryValue("hostName", out var hostNameValue))
            {
                routeData.HostName = hostNameValue.GetSanitizedHostDomain();
            }

            return routeData;
        }

        if (TryGetQueryValue("isPreview", out _))
        {
            return routeData;
        }

        return AssignSiteIdAndHostBasedOnRequest(routeData);
    }

    /// <summary>
    /// Populates SiteId and HostName from the current HttpRequest using the CMS 12 site resolver.
    /// </summary>
    private SecurityRouteData AssignSiteIdAndHostBasedOnRequest(SecurityRouteData routeData)
    {
        var request = _contextAccessor.HttpContext?.Request;
        if (request is null)
        {
            return routeData;
        }

        try
        {
            var site = _siteDefinitionResolver.GetByHostname(request.Host.Host, true);
            var siteId = site?.Id;
            routeData.SiteId = siteId.HasValue && siteId.Value != Guid.Empty ? siteId : null;
        }
        catch
        {
            // Resolution may fail for requests outside the site pipeline (e.g. admin paths during startup)
            routeData.SiteId = null;
        }

        routeData.HostName = request.Host.Value.GetSanitizedHostDomain();

        return routeData;
    }

    /// <summary>
    /// Gets the security route type based on content path and content.
    /// </summary>
    private SecurityRouteType GetSecurityRouteType(PathString contentPath, IContent? content)
    {
        var isOnExclusionList = false;
        if (_configuration is { ExclusionPaths.Count: > 0 } && !string.IsNullOrWhiteSpace(contentPath))
        {
            foreach (var exclusionPath in _configuration.ExclusionPaths)
            {
                if (contentPath.StartsWithSegments(exclusionPath, StringComparison.OrdinalIgnoreCase))
                {
                    isOnExclusionList = true;
                    break;
                }
            }
        }

        if (content is IContentSecurityPolicyPage { ContentSecurityPolicySources.Count: > 0 })
        {
            return isOnExclusionList ? SecurityRouteType.ContentSpecificNoNonceOrHash : SecurityRouteType.ContentSpecific;
        }

        return isOnExclusionList ? SecurityRouteType.NoNonceOrHash : SecurityRouteType.Default;
    }

    /// <summary>
    /// Determines if the current request is for the compiled headers api.
    /// </summary>
    private static bool IsHeadersApiRequest(PathString path)
    {
        return path.StartsWithSegments("/stott.security.optimizely/api/compiled-headers", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the content from the querystring "pageId" parameter for a compiled headers api request.
    /// </summary>
    private IContent? GetContentFromQuery()
    {
        var context = _contextAccessor?.HttpContext;
        if (context?.Request?.Query is null)
        {
            return null;
        }

        if (context.Request.Query.TryGetValue("pageId", out var pageIdString) &&
            int.TryParse(pageIdString, out var pageId) &&
            _contentLoader.TryGet<IContent>(new ContentReference(pageId), out var content))
        {
            return content;
        }

        return null;
    }

    private bool TryGetQueryValue(string queryName, out string queryValue)
    {
        queryValue = string.Empty;
        var query = _contextAccessor.HttpContext?.Request?.Query;
        if (query is null)
        {
            return false;
        }

        if (query.TryGetValue(queryName, out var testValue) && !string.IsNullOrWhiteSpace(testValue))
        {
            queryValue = testValue.ToString();
            return true;
        }

        return false;
    }
}
