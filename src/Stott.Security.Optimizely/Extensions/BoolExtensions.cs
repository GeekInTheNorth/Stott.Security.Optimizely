namespace Stott.Security.Optimizely.Extensions;

internal static class BoolExtensions
{
    internal static string ToYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }
}