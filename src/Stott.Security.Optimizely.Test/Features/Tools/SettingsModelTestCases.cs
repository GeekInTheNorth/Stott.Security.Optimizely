using System;
using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

namespace Stott.Security.Optimizely.Test.Features.Tools;

public static class SettingsModelTestCases
{
    public static IEnumerable<TestCaseData> XContentTypeOptionsTestCases
    {
        get
        {
            return GetEnumTestCases<XContentTypeOptions>();
        }
    }

    public static IEnumerable<TestCaseData> XXssProtectionTestCases
    {
        get
        {
            return GetEnumTestCases<XssProtection>();
        }
    }

    public static IEnumerable<TestCaseData> ReferrerPolicyTestCases
    {
        get
        {
            return GetEnumTestCases<ReferrerPolicy>();
        }
    }

    public static IEnumerable<TestCaseData> XFrameOptionsTestCases
    {
        get
        {
            return GetEnumTestCases<XFrameOptions>();
        }
    }

    public static IEnumerable<TestCaseData> CrossOriginEmbedderPolicyTestCases
    {
        get
        {
            return GetEnumTestCases<CrossOriginEmbedderPolicy>();
        }
    }

    public static IEnumerable<TestCaseData> CrossOriginOpenerPolicyTestCases
    {
        get
        {
            return GetEnumTestCases<CrossOriginOpenerPolicy>();
        }
    }

    public static IEnumerable<TestCaseData> CrossOriginResourcePolicyTestCases
    {
        get
        {
            return GetEnumTestCases<CrossOriginResourcePolicy>();
        }
    }

    public static IEnumerable<TestCaseData> MaxAgeTestCases
    {
        get
        {
            yield return new TestCaseData(-1, 1);
            yield return new TestCaseData(0, 1);
            yield return new TestCaseData(1, 0);
            yield return new TestCaseData(CspConstants.TwoYearsInSeconds - 1, 0);
            yield return new TestCaseData(CspConstants.TwoYearsInSeconds, 0);
            yield return new TestCaseData(CspConstants.TwoYearsInSeconds + 1, 1);
        }
    }

    public static IEnumerable<TestCaseData> GetEnumTestCases<TEnum>()
    {
        yield return new TestCaseData(null, 1);
        yield return new TestCaseData(string.Empty, 1);
        yield return new TestCaseData("Invalid Value", 1);
        yield return new TestCaseData(" ", 1);

        var enumNames = Enum.GetNames(typeof(TEnum));
        foreach (var enumName in enumNames)
        {
            yield return new TestCaseData(enumName, 0);
        }
    }
}