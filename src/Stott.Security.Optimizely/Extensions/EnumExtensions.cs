namespace Stott.Security.Optimizely.Extensions;

using System;

using Stott.Security.Optimizely.Attributes;

public static class EnumExtensions
{
    public static string GetSecurityHeaderValue(this Enum enumValue)
    {
        return ((SecurityHeaderValueAttribute)Attribute.GetCustomAttribute(enumValue.GetType().GetField(enumValue.ToString()), typeof(SecurityHeaderValueAttribute))).SecurityHeaderValue;
    }
}
