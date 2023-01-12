namespace Stott.Security.Optimizely.Attributes;

using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class SecurityHeaderValueAttribute : Attribute
{
    public string? SecurityHeaderValue { get; }

    public SecurityHeaderValueAttribute(string? securityHeaderValue)
    {
        SecurityHeaderValue = securityHeaderValue;
    }
}