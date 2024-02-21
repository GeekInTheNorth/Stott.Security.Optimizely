namespace Stott.Security.Optimizely.Features.Reporting.Service;

public interface IReportingEndpointValidator
{
    bool IsValidReportToEndPoint(string? uri, out string? errorMessage);

    bool IsValidReportUriEndPoint(string? uri, out string? errorMessage);
}