using System;
using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Test.Features.Tools;

public static class SettingsModelTestCases
{
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