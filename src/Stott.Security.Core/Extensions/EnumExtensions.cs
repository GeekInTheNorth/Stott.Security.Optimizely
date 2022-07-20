namespace Stott.Security.Core.Extensions;

using System;

using Stott.Security.Core.Attributes;

public static class EnumExtensions
{
    public static string GetSecurityHeaderValue(this Enum enumValue)
    {
        return ((SecurityHeaderValueAttribute)Attribute.GetCustomAttribute(enumValue.GetType().GetField(enumValue.ToString()), typeof(SecurityHeaderValueAttribute))).SecurityHeaderValue;
    }
}
