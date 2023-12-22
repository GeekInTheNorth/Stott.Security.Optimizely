namespace Stott.Security.Optimizely.Features.Header;

public interface ICspReportUrlResolver
{
    string GetReportUriPath();

    string GetReportToPath();
}