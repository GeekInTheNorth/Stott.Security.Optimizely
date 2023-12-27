namespace Stott.Security.Optimizely.Features.Header;

public interface ICspReportUrlResolver
{
    string GetHost();

    string GetReportUriPath();

    string GetReportToPath();
}