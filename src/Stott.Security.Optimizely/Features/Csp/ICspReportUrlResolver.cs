namespace Stott.Security.Optimizely.Features.Csp;

public interface ICspReportUrlResolver
{
    string GetHost();

    string GetReportToPath();
}