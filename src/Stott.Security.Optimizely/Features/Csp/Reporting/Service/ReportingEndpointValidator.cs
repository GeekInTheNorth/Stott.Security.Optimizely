using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Reporting.Models;

namespace Stott.Security.Optimizely.Features.Csp.Reporting.Service;

public sealed class ReportingEndpointValidator : IReportingEndpointValidator
{
    private readonly IHttpClientFactory _clientFactory;

    public ReportingEndpointValidator(IHttpClientFactory httpClientFactory)
    {
        _clientFactory = httpClientFactory;
    }

    public bool IsValidReportToEndPoint(string? uri, out string? errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(uri) || !Uri.IsWellFormedUriString(uri, UriKind.Absolute))
        {
            errorMessage = "Report-To Endpoint is not a valid Url.";

            return false;
        }

        var model = new List<ReportToWrapper>
        {
            new()
            {
                ReportType = "csp-violation",
                Age = 9214,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36",
                CspReport = new ReportToBody
                {
                    BlockedUri = "https://www.example.com",
                    DocumentUri = "https://www.example.com",
                    EffectiveDirective = CspConstants.Directives.DefaultSource,
                    StatusCode = 200,
                    Disposition = "report",
                    OriginalPolicy = "default-src 'self';"
                }
            }
        };

        var isValid = IsEndPointValid(uri, "application/reports+json", model);

        if (!isValid)
        {
            errorMessage = "Report-To Endpoint has not returned a valid response within a timely fashion.";
        }

        return isValid;
    }

    private bool IsEndPointValid<TReport>(string uri, string acceptType, TReport model)
    {
        try
        {
            var jsonContent = JsonContent.Create(model, typeof(TReport), new MediaTypeHeaderValue(acceptType));
            var client = _clientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var response = client.PostAsync(uri, jsonContent).GetAwaiter().GetResult();

            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}