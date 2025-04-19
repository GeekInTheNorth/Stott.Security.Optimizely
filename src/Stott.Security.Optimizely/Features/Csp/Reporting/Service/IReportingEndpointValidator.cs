namespace Stott.Security.Optimizely.Features.Csp.Reporting.Service;

public interface IReportingEndpointValidator
{
    bool IsValidReportToEndPoint(string? uri, out string? errorMessage);
}