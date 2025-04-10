namespace Stott.Security.Optimizely.Test.Features.Cors.Repository;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Cors;

public static class CorsSettingsMapperTestCases
{
    public static IEnumerable<TestCaseData> IsEnabledTestCases
    {
        get
        {
            yield return new TestCaseData(true);
            yield return new TestCaseData(false);
        }
    }

    public static IEnumerable<TestCaseData> MapToModelMethodMappingTestCases
    {
        get
        {
            yield return new TestCaseData("*", true, true, true, true, true, true, true, true, true);
            yield return new TestCaseData("GET", true, false, false, false, false, false, false, false, false);
            yield return new TestCaseData("HEAD", false, true, false, false, false, false, false, false, false);
            yield return new TestCaseData("CONNECT", false, false, true, false, false, false, false, false, false);
            yield return new TestCaseData("DELETE", false, false, false, true, false, false, false, false, false);
            yield return new TestCaseData("OPTIONS", false, false, false, false, true, false, false, false, false);
            yield return new TestCaseData("PATCH", false, false, false, false, false, true, false, false, false);
            yield return new TestCaseData("POST", false, false, false, false, false, false, true, false, false);
            yield return new TestCaseData("PUT", false, false, false, false, false, false, false, true, false);
            yield return new TestCaseData("TRACE", false, false, false, false, false, false, false, false, true);
            yield return new TestCaseData("get", true, false, false, false, false, false, false, false, false);
            yield return new TestCaseData("head", false, true, false, false, false, false, false, false, false);
            yield return new TestCaseData("connect", false, false, true, false, false, false, false, false, false);
            yield return new TestCaseData("delete", false, false, false, true, false, false, false, false, false);
            yield return new TestCaseData("options", false, false, false, false, true, false, false, false, false);
            yield return new TestCaseData("patch", false, false, false, false, false, true, false, false, false);
            yield return new TestCaseData("post", false, false, false, false, false, false, true, false, false);
            yield return new TestCaseData("put", false, false, false, false, false, false, false, true, false);
            yield return new TestCaseData("trace", false, false, false, false, false, false, false, false, true);
            yield return new TestCaseData("GET,HEAD", true, true, false, false, false, false, false, false, false);
            yield return new TestCaseData("CONNECT,DELETE", false, false, true, true, false, false, false, false, false);
            yield return new TestCaseData("OPTIONS,TRACE", false, false, false, false, true, false, false, false, true);
            yield return new TestCaseData("POST,PUT,PATCH", false, false, false, false, false, true, true, true, false);
        }
    }

    public static IEnumerable<TestCaseData> MapToModelHeaderTestCases
    {
        get
        {
            yield return new TestCaseData(null, new List<string>(0));
            yield return new TestCaseData(string.Empty, new List<string>(0));
            yield return new TestCaseData(" ", new List<string>(0));
            yield return new TestCaseData("Content-Type", new List<string> { "Content-Type" });
            yield return new TestCaseData("Content-Type,Accept", new List<string> { "Content-Type", "Accept" });
            yield return new TestCaseData("Content-Type,,Accept", new List<string> { "Content-Type", "Accept" });
        }
    }

    public static IEnumerable<TestCaseData> MapToModelOriginTestCases
    {
        get
        {
            yield return new TestCaseData(null, new List<string>(0));
            yield return new TestCaseData(string.Empty, new List<string>(0));
            yield return new TestCaseData(" ", new List<string>(0));
            yield return new TestCaseData("*", new List<string>(0));
            yield return new TestCaseData("https://www.example.com", new List<string> { "https://www.example.com" });
            yield return new TestCaseData("https://www.example.com,https://www.test.com", new List<string> { "https://www.example.com", "https://www.test.com" });
            yield return new TestCaseData("https://www.example.com,,https://www.test.com", new List<string> { "https://www.example.com", "https://www.test.com" });
        }
    }

    public static IEnumerable<TestCaseData> MapToModelMaxAgeTestCases
    {
        get
        {
            yield return new TestCaseData(-1, 1);
            yield return new TestCaseData(0, 1);
            yield return new TestCaseData(1, 1);
            yield return new TestCaseData(2, 2);
            yield return new TestCaseData(7199, 7199);
            yield return new TestCaseData(7200, 7200);
            yield return new TestCaseData(7201, 7200);
        }
    }

    public static IEnumerable<TestCaseData> MapToEntitiesHeaderTestCases
    {
        get
        {
            yield return new TestCaseData(new List<CorsConfigurationItem>(0), null);
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "Content-Type" } }, "Content-Type");
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "Content-Type" }, new() { Value = "Accept" } }, "Content-Type,Accept");
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "Content-Type" }, new() { Value = "invalid header$" } }, "Content-Type");
        }
    }

    public static IEnumerable<TestCaseData> MapToEntitiesOriginTestCases
    {
        get
        {
            yield return new TestCaseData(new List<CorsConfigurationItem>(0), "*");
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "https://www.example.com" } }, "https://www.example.com");
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "https://www.example.com" }, new() { Value = "https://www.test.com" } }, "https://www.example.com,https://www.test.com");
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "https://www.example.com" }, new() { Value = "/partial/path" } }, "https://www.example.com");
            yield return new TestCaseData(new List<CorsConfigurationItem> { new() { Value = "https://www.example.com" }, new() { Value = "not a url" } }, "https://www.example.com");
        }
    }

    public static IEnumerable<TestCaseData> MapToEntitiesMethodMappingTestCases
    {
        get
        {
            yield return new TestCaseData(true, true, true, true, true, true, true, true, true, "*");
            yield return new TestCaseData(true, false, false, false, false, false, false, false, false, "GET");
            yield return new TestCaseData(false, true, false, false, false, false, false, false, false, "HEAD");
            yield return new TestCaseData(false, false, true, false, false, false, false, false, false, "CONNECT");
            yield return new TestCaseData(false, false, false, true, false, false, false, false, false, "DELETE");
            yield return new TestCaseData(false, false, false, false, true, false, false, false, false, "OPTIONS");
            yield return new TestCaseData(false, false, false, false, false, true, false, false, false, "PATCH");
            yield return new TestCaseData(false, false, false, false, false, false, true, false, false, "POST");
            yield return new TestCaseData(false, false, false, false, false, false, false, true, false, "PUT");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, true, "TRACE");
            yield return new TestCaseData(true, true, false, false, false, false, false, false, false, "GET,HEAD");
            yield return new TestCaseData(false, false, true, true, false, false, false, false, false, "CONNECT,DELETE");
            yield return new TestCaseData(false, false, false, false, true, false, false, false, true, "OPTIONS,TRACE");
            yield return new TestCaseData(false, false, false, false, false, true, true, true, false, "PATCH,POST,PUT");
        }
    }
}