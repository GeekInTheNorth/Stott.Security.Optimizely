using System;

using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Route;

/// <inheritdoc cref="ISecurityRouteHelper"/>
public sealed class SecurityRouteHelper(
    IContentRouteHelper contentRouteHelper,
    IContentLoader contentLoader,
    IUrlResolver urlResolver,
    IHttpContextAccessor contextAccessor,
    SecurityRouteConfiguration configuration) : ISecurityRouteHelper
{
    private SecurityRouteData? _currentData;

    public SecurityRouteData GetRouteData()
    {
        if (_currentData is not null)
        {
            return _currentData;
        }

        var path = contextAccessor.HttpContext?.Request?.Path ?? new PathString(string.Empty);
        _currentData = IsHeadersApiRequest(path) ? GetDataForPreviewApi() : GetDataForRequest(path);

        return _currentData;
    }

    /// <summary>
    /// Gets data for a standard headed request.
    /// </summary>
    /// <returns></returns>
    private SecurityRouteData GetDataForRequest(PathString path)
    {
        var content = contentRouteHelper.Content;

        return new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(path, content)
        };
    }

    /// <summary>
    /// Gets data for a compiled headers api request.
    /// </summary>
    /// <returns></returns>
    private SecurityRouteData GetDataForPreviewApi()
    { 
        var content = GetContentFromQuery();
        var url = urlResolver.GetUrl(content) ?? string.Empty;

        return new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(new PathString(url), content)
        };
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
}