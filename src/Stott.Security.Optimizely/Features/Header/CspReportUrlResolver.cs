namespace Stott.Security.Optimizely.Features.Header;

using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

public sealed class CspReportUrlResolver : ICspReportUrlResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string ReportUriPath = "/stott.security.optimizely/api/cspreporting/reporturiviolation/";

    private const string ReportToPath = "/stott.security.optimizely/api/cspreporting/reporttoviolation/";

    public CspReportUrlResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetHost()
    {
        return _httpContextAccessor.HttpContext?.Request?.Host.Value ?? string.Empty;
    }

    public string GetReportToPath()
    {
        return GetAbsolutePath(ReportToPath);
    }

    public string GetReportUriPath()
    {
        return GetAbsolutePath(ReportUriPath);
    }

    private string GetAbsolutePath(string path)
    {
        try
        {
            if (_httpContextAccessor.HttpContext?.Request is null)
            {
                return path;
            }

            var fullUrl = UriHelper.GetDisplayUrl(_httpContextAccessor.HttpContext!.Request);

            var uri = new Uri(new Uri(fullUrl), path);

            return uri.ToString();
        }
        catch (Exception)
        {
            return path;
        }
    }
}
