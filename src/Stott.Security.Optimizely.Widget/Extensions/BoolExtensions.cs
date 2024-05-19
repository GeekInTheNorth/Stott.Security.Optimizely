namespace Stott.Security.Optimizely.Widget.Extensions;

public static class BoolExtensions
{
    public static string ToYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }
}