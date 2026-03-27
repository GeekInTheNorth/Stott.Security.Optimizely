using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Applications;
using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Configuration;
using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Route;

/// <inheritdoc cref="ISecurityRouteHelper"/>
public sealed class SecurityRouteHelper(
    IContentRouteHelper contentRouteHelper,
    IContentLoader contentLoader,
    IUrlResolver urlResolver,
    IHttpContextAccessor contextAccessor,
    IApplicationResolver applicationResolver,
    SecurityConfiguration configuration) : ISecurityRouteHelper
{
    private SecurityRouteData? _currentData;

    public async Task<SecurityRouteData> GetRouteDataAsync()
    {
        if (_currentData is not null)
        {
            return _currentData;
        }

        var path = contextAccessor.HttpContext?.Request?.Path ?? new PathString(string.Empty);

        if (IsHeadersApiRequest(path))
        {
            _currentData = await GetDataForPreviewApi();    
        }
        else
        {
            _currentData = await GetDataForRequest(path);    
        }

        return _currentData;
    }

    private async Task<SecurityRouteData> AssignAppIdAndHostBasedOnRequest(SecurityRouteData routeData)
    {
        // Resolve application context
        try
        {
            var application = await applicationResolver.GetByContextAsync();
            routeData.AppId = application?.Name;
        }
        catch
        {
            // Application resolution may fail for non-site requests (e.g., admin paths)
            routeData.AppId = null;
        }

        routeData.HostName = contextAccessor.HttpContext?.Request?.Host.Value;

        return routeData;
    }

    /// <summary>
    /// Gets data for a standard headed request.
    /// </summary>
    /// <returns></returns>
    private async Task<SecurityRouteData> GetDataForRequest(PathString path)
    {
        var content = contentRouteHelper.Content;
        var routeData = new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(path, content)
        };

        return await AssignAppIdAndHostBasedOnRequest(routeData);
    }

    /// <summary>
    /// Gets data for a compiled headers api request.
    /// </summary>
    /// <returns></returns>
    private async Task<SecurityRouteData> GetDataForPreviewApi()
    {
        var content = GetContentFromQuery();
        var url = urlResolver.GetUrl(content) ?? string.Empty;
        var routeData = new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(new PathString(url), content)
        };

        if (TryGetQueryValue("appId", out var appIdValue))
        {
            routeData.AppId = appIdValue.ToString();
            if (TryGetQueryValue("hostName", out var hostNameValue))
            {
                routeData.HostName = hostNameValue.GetSanitizedHostDomain();
            }
        }
        else if (!TryGetQueryValue("isPreview", out _))
        {
            return await AssignAppIdAndHostBasedOnRequest(routeData);
        }

        return routeData;
    }

    /// <summary>
    /// Gets the security route type based on content path and content.
    /// </summary>
    /// <param name="contentPath"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    private SecurityRouteType GetSecurityRouteType(PathString contentPath, IContent? content)
    {
        var isOnExclusionList = false;
        if (configuration is { ExclusionPaths.Count: > 0 } && !string.IsNullOrWhiteSpace(contentPath))
        {
            foreach (var exclusionPath in configuration.ExclusionPaths)
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
    /// <returns></returns>
    private static bool IsHeadersApiRequest(PathString path)
    {
        return path.StartsWithSegments("/stott.security.optimizely/api/compiled-headers", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the content from the querystring "pageId" parameter for a compiled headers api request.
    /// </summary>
    /// <returns></returns>
    private IContent? GetContentFromQuery()
    {
        var context = contextAccessor?.HttpContext;
        if (context?.Request?.Query is null)
        {
            return null;
        }

        if (context.Request.Query.TryGetValue("pageId", out var pageIdString) &&
            int.TryParse(pageIdString, out var pageId) &&
            contentLoader.TryGet<IContent>(new ContentReference(pageId), out var content))
        {
            return content;
        }

        return null;
    }

    private bool TryGetQueryValue(string queryName, out string queryValue)
    {
        queryValue = string.Empty;
        var query = contextAccessor.HttpContext?.Request?.Query;
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
