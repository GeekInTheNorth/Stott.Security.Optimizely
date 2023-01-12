namespace Stott.Security.Optimizely.Extensions;

using System;

using Stott.Security.Optimizely.Attributes;

public static class EnumExtensions
{
    public static string GetSecurityHeaderValue(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo == null)
        {
            return enumValue.ToString();
        }

        var attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(SecurityHeaderValueAttribute)) as SecurityHeaderValueAttribute;
        if (attribute == null)
        {
            return enumValue.ToString();
        }

        return attribute.SecurityHeaderValue ?? enumValue.ToString();
    }
}