namespace Stott.Security.Optimizely.Extensions;

public static class BoolExtensions
{
    public static string ToYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }
}