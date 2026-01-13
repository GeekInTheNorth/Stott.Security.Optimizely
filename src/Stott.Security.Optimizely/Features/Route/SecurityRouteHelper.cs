using System;

using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Route;

/// <inheritdoc cref="ISecurityRouteHelper"/>
public sealed class SecurityRouteHelper : ISecurityRouteHelper
{
    private readonly IPageRouteHelper _pageRouteHelper;

    private readonly IContentLoader _contentLoader;

    private readonly IUrlResolver _urlResolver;

    private readonly IHttpContextAccessor _contextAccessor;

    private readonly SecurityRouteConfiguration _configuration;

    private SecurityRouteData? _currentData;

    public SecurityRouteHelper(
        IPageRouteHelper pageRouteHelper,
        IContentLoader contentLoader,
        IUrlResolver urlResolver,
        IHttpContextAccessor contextAccessor,
        SecurityRouteConfiguration configuration)
    {
        _pageRouteHelper = pageRouteHelper;
        _contentLoader = contentLoader;
        _urlResolver = urlResolver;
        _contextAccessor = contextAccessor;
        _configuration = configuration;
    }

    public SecurityRouteData GetRouteData()
    {
        if (_currentData is not null)
        {
            return _currentData;
        }

        _currentData = IsHeadersApiRequest() ? GetDataForPreviewApi() : GetDataForRequest();

        return _currentData;
    }

    /// <summary>
    /// Gets data for a standard headed request.
    /// </summary>
    /// <returns></returns>
    private SecurityRouteData GetDataForRequest()
    {
        var content = _pageRouteHelper.Content;

        return new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(_contextAccessor.HttpContext?.Request?.Path, content)
        };
    }

    /// <summary>
    /// Gets data for a compiled headers api request.
    /// </summary>
    /// <returns></returns>
    private SecurityRouteData GetDataForPreviewApi()
    { 
        var content = GetContentFromQuery();
        var url = _urlResolver.GetUrl(content);

        return new SecurityRouteData
        {
            Content = content,
            RouteType = GetSecurityRouteType(url, content)
        };
    }

    /// <summary>
    /// Gets the security route type based on content path and content.
    /// </summary>
    /// <param name="contentPath"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    private SecurityRouteType GetSecurityRouteType(string? contentPath, IContent? content)
    {
        var isOnExclusionList = false;
        if (_configuration is { ExclusionPaths.Count: > 0 } && !string.IsNullOrWhiteSpace(contentPath))
        {
            foreach (var exclusionPath in _configuration.ExclusionPaths)
            {
                if (contentPath.StartsWith(exclusionPath, StringComparison.OrdinalIgnoreCase))
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
    private bool IsHeadersApiRequest()
    {
        var context = _contextAccessor?.HttpContext;
        if (context?.Request?.Path is null)
        {
            return false;
        }

        return context.Request.Path.StartsWithSegments("/stott.security.optimizely/api/compiled-headers", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the content from the querystring "pageId" parameter for a compiled headers api request.
    /// </summary>
    /// <returns></returns>
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
}