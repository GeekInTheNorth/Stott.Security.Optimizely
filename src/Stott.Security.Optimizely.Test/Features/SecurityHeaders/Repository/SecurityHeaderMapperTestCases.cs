using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

namespace Stott.Security.Optimizely.Test.Features.SecurityHeaders.Repository;

public static class SecurityHeaderMapperTestCases
{
    public static IEnumerable<TestCaseData> XContentTypeTestCases
    {
        get
        {
            yield return new TestCaseData(XContentTypeOptions.None, null, XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.None, string.Empty, XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.None, " ", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.None, "none", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.None, "NONE", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.None, "None", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.None, "nosniff", XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.None, "NOSNIFF", XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.None, "NoSniff", XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, null, XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, string.Empty, XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, " ", XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, "none", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, "NONE", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, "None", XContentTypeOptions.None);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, "nosniff", XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, "NOSNIFF", XContentTypeOptions.NoSniff);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, "NoSniff", XContentTypeOptions.NoSniff);
        }
    }

    public static IEnumerable<TestCaseData> XssProtectionTestCases
    {
        get
        {
            yield return new TestCaseData(XssProtection.None, null, XssProtection.None);
            yield return new TestCaseData(XssProtection.None, string.Empty, XssProtection.None);
            yield return new TestCaseData(XssProtection.None, " ", XssProtection.None);
            yield return new TestCaseData(XssProtection.None, "none", XssProtection.None);
            yield return new TestCaseData(XssProtection.None, "NONE", XssProtection.None);
            yield return new TestCaseData(XssProtection.None, "None", XssProtection.None);
            yield return new TestCaseData(XssProtection.None, "enabled", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.None, "ENABLED", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.None, "Enabled", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.None, "enabledwithblocking", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.None, "ENABLEDWITHBLOCKING", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.None, "EnabledWithBlocking", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.Enabled, null, XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.Enabled, string.Empty, XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.Enabled, " ", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.Enabled, "none", XssProtection.None);
            yield return new TestCaseData(XssProtection.Enabled, "NONE", XssProtection.None);
            yield return new TestCaseData(XssProtection.Enabled, "None", XssProtection.None);
            yield return new TestCaseData(XssProtection.Enabled, "enabled", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.Enabled, "ENABLED", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.Enabled, "Enabled", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.Enabled, "enabledwithblocking", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.Enabled, "ENABLEDWITHBLOCKING", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.Enabled, "EnabledWithBlocking", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, null, XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, string.Empty, XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, " ", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "none", XssProtection.None);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "NONE", XssProtection.None);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "None", XssProtection.None);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "enabled", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "ENABLED", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "Enabled", XssProtection.Enabled);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "enabledwithblocking", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "ENABLEDWITHBLOCKING", XssProtection.EnabledWithBlocking);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, "EnabledWithBlocking", XssProtection.EnabledWithBlocking);
        }
    }

    public static IEnumerable<TestCaseData> ReferrerPolicyTestCases
    {
        get
        {
            yield return new TestCaseData(ReferrerPolicy.None, null, ReferrerPolicy.None);
            yield return new TestCaseData(ReferrerPolicy.None, string.Empty, ReferrerPolicy.None);
            yield return new TestCaseData(ReferrerPolicy.None, "invalid", ReferrerPolicy.None);
            yield return new TestCaseData(ReferrerPolicy.None, " ", ReferrerPolicy.None);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrer.ToString().ToLower(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrer.ToString().ToUpper(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrer.ToString(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToLower(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToUpper(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrerWhenDowngrade.ToString(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.Origin.ToString().ToLower(), ReferrerPolicy.Origin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.Origin.ToString().ToUpper(), ReferrerPolicy.Origin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.Origin.ToString(), ReferrerPolicy.Origin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.OriginWhenCrossOrigin.ToString().ToLower(), ReferrerPolicy.OriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.OriginWhenCrossOrigin.ToString().ToUpper(), ReferrerPolicy.OriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.OriginWhenCrossOrigin.ToString(), ReferrerPolicy.OriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.SameOrigin.ToString().ToLower(), ReferrerPolicy.SameOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.SameOrigin.ToString().ToUpper(), ReferrerPolicy.SameOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.SameOrigin.ToString(), ReferrerPolicy.SameOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.StrictOrigin.ToString().ToLower(), ReferrerPolicy.StrictOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.StrictOrigin.ToString().ToUpper(), ReferrerPolicy.StrictOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.StrictOrigin.ToString(), ReferrerPolicy.StrictOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.StrictOriginWhenCrossOrigin.ToString().ToLower(), ReferrerPolicy.StrictOriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.StrictOriginWhenCrossOrigin.ToString().ToUpper(), ReferrerPolicy.StrictOriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.StrictOriginWhenCrossOrigin.ToString(), ReferrerPolicy.StrictOriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrer.ToString().ToLower(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrer.ToString().ToUpper(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrer.ToString(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToLower(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToUpper(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.None, ReferrerPolicy.NoReferrerWhenDowngrade.ToString(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, null, ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, string.Empty, ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, "invalid", ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, " ", ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrer.ToString().ToLower(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrer.ToString().ToUpper(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrer.ToString(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToLower(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToUpper(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrerWhenDowngrade.ToString(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.Origin.ToString().ToLower(), ReferrerPolicy.Origin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.Origin.ToString().ToUpper(), ReferrerPolicy.Origin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.Origin.ToString(), ReferrerPolicy.Origin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.OriginWhenCrossOrigin.ToString().ToLower(), ReferrerPolicy.OriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.OriginWhenCrossOrigin.ToString().ToUpper(), ReferrerPolicy.OriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.OriginWhenCrossOrigin.ToString(), ReferrerPolicy.OriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.SameOrigin.ToString().ToLower(), ReferrerPolicy.SameOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.SameOrigin.ToString().ToUpper(), ReferrerPolicy.SameOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.SameOrigin.ToString(), ReferrerPolicy.SameOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.StrictOrigin.ToString().ToLower(), ReferrerPolicy.StrictOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.StrictOrigin.ToString().ToUpper(), ReferrerPolicy.StrictOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.StrictOrigin.ToString(), ReferrerPolicy.StrictOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.StrictOriginWhenCrossOrigin.ToString().ToLower(), ReferrerPolicy.StrictOriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.StrictOriginWhenCrossOrigin.ToString().ToUpper(), ReferrerPolicy.StrictOriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.StrictOriginWhenCrossOrigin.ToString(), ReferrerPolicy.StrictOriginWhenCrossOrigin);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrer.ToString().ToLower(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrer.ToString().ToUpper(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrer.ToString(), ReferrerPolicy.NoReferrer);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToLower(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrerWhenDowngrade.ToString().ToUpper(), ReferrerPolicy.NoReferrerWhenDowngrade);
            yield return new TestCaseData(ReferrerPolicy.NoReferrer, ReferrerPolicy.NoReferrerWhenDowngrade.ToString(), ReferrerPolicy.NoReferrerWhenDowngrade);
        }
    }

    public static IEnumerable<TestCaseData> XFrameOptionsTestCases
    {
        get
        {
            yield return new TestCaseData(XFrameOptions.None, null, XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, string.Empty, XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, "invalid", XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, " ", XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, XFrameOptions.None.ToString().ToLower(), XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, XFrameOptions.None.ToString().ToUpper(), XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, XFrameOptions.None.ToString(), XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.None, XFrameOptions.SameOrigin.ToString().ToLower(), XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.None, XFrameOptions.SameOrigin.ToString().ToUpper(), XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.None, XFrameOptions.SameOrigin.ToString(), XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, null, XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, string.Empty, XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, "invalid", XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, " ", XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, XFrameOptions.None.ToString().ToLower(), XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.SameOrigin, XFrameOptions.None.ToString().ToUpper(), XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.SameOrigin, XFrameOptions.None.ToString(), XFrameOptions.None);
            yield return new TestCaseData(XFrameOptions.SameOrigin, XFrameOptions.SameOrigin.ToString().ToLower(), XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, XFrameOptions.SameOrigin.ToString().ToUpper(), XFrameOptions.SameOrigin);
            yield return new TestCaseData(XFrameOptions.SameOrigin, XFrameOptions.SameOrigin.ToString(), XFrameOptions.SameOrigin);
        }
    }

    public static IEnumerable<TestCaseData> CrossOriginEmbedderPolicyTestCases
    {
        get
        {
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, null, CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, string.Empty, CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, "invalid", CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, " ", CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.None.ToString().ToLower(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.None.ToString().ToUpper(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.None.ToString(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.RequireCorp.ToString().ToLower(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.RequireCorp.ToString().ToUpper(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.RequireCorp.ToString(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.UnsafeNone.ToString().ToLower(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, CrossOriginEmbedderPolicy.UnsafeNone.ToString(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, null, CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, string.Empty, CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, "invalid", CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, " ", CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.None.ToString().ToLower(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.None.ToString().ToUpper(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.None.ToString(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.RequireCorp.ToString().ToLower(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.RequireCorp.ToString().ToUpper(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.RequireCorp.ToString(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.UnsafeNone.ToString().ToLower(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginEmbedderPolicy.UnsafeNone.ToString(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, null, CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, string.Empty, CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, "invalid", CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, " ", CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.None.ToString().ToLower(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.None.ToString().ToUpper(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.None.ToString(), CrossOriginEmbedderPolicy.None);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.RequireCorp.ToString().ToLower(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.RequireCorp.ToString().ToUpper(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.RequireCorp.ToString(), CrossOriginEmbedderPolicy.RequireCorp);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.UnsafeNone.ToString().ToLower(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginEmbedderPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, CrossOriginEmbedderPolicy.UnsafeNone.ToString(), CrossOriginEmbedderPolicy.UnsafeNone);
        }
    }

    public static IEnumerable<TestCaseData> CrossOriginOpenerPolicyTestCases
    {
        get
        {
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, null, CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, string.Empty, CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, "invalid", CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, " ", CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.None.ToString().ToLower(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.None.ToString().ToUpper(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.None.ToString(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.SameOrigin.ToString().ToLower(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.SameOrigin.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.SameOrigin.ToString(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToLower(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToLower(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.None, CrossOriginOpenerPolicy.UnsafeNone.ToString(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, null, CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, string.Empty, CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, "invalid", CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, " ", CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.None.ToString().ToLower(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.None.ToString().ToUpper(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.None.ToString(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.SameOrigin.ToString().ToLower(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.SameOrigin.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.SameOrigin.ToString(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToLower(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToLower(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOrigin, CrossOriginOpenerPolicy.UnsafeNone.ToString(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, null, CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, string.Empty, CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, "invalid", CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, " ", CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.None.ToString().ToLower(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.None.ToString().ToUpper(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.None.ToString(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.SameOrigin.ToString().ToLower(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.SameOrigin.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.SameOrigin.ToString(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToLower(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToLower(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.SameOriginAllowPopups, CrossOriginOpenerPolicy.UnsafeNone.ToString(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, null, CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, string.Empty, CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, "invalid", CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, " ", CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.None.ToString().ToLower(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.None.ToString().ToUpper(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.None.ToString(), CrossOriginOpenerPolicy.None);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.SameOrigin.ToString().ToLower(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.SameOrigin.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.SameOrigin.ToString(), CrossOriginOpenerPolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToLower(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString().ToUpper(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.SameOriginAllowPopups.ToString(), CrossOriginOpenerPolicy.SameOriginAllowPopups);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToLower(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.UnsafeNone.ToString().ToUpper(), CrossOriginOpenerPolicy.UnsafeNone);
            yield return new TestCaseData(CrossOriginOpenerPolicy.UnsafeNone, CrossOriginOpenerPolicy.UnsafeNone.ToString(), CrossOriginOpenerPolicy.UnsafeNone);
        }
    }

    public static IEnumerable<TestCaseData> CrossOriginResourcePolicyTestCases
    {
        get
        {
            yield return new TestCaseData(CrossOriginResourcePolicy.None, null, CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, string.Empty, CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, "invalid", CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, " ", CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.None.ToString().ToLower(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.None.ToString().ToUpper(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.None.ToString(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.SameSite.ToString().ToLower(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.SameSite.ToString().ToUpper(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.SameSite.ToString(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.SameOrigin.ToString().ToLower(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.SameOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.SameOrigin.ToString(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.CrossOrigin.ToString().ToLower(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.CrossOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.None, CrossOriginResourcePolicy.CrossOrigin.ToString(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, null, CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, string.Empty, CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, "invalid", CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, " ", CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.None.ToString().ToLower(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.None.ToString().ToUpper(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.None.ToString(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.SameSite.ToString().ToLower(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.SameSite.ToString().ToUpper(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.SameSite.ToString(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.SameOrigin.ToString().ToLower(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.SameOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.SameOrigin.ToString(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.CrossOrigin.ToString().ToLower(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.CrossOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameSite, CrossOriginResourcePolicy.CrossOrigin.ToString(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, null, CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, string.Empty, CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, "invalid", CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, " ", CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.None.ToString().ToLower(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.None.ToString().ToUpper(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.None.ToString(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.SameSite.ToString().ToLower(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.SameSite.ToString().ToUpper(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.SameSite.ToString(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.SameOrigin.ToString().ToLower(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.SameOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.SameOrigin.ToString(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.CrossOrigin.ToString().ToLower(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.CrossOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.SameOrigin, CrossOriginResourcePolicy.CrossOrigin.ToString(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, null, CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, string.Empty, CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, "invalid", CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, " ", CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.None.ToString().ToLower(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.None.ToString().ToUpper(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.None.ToString(), CrossOriginResourcePolicy.None);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.SameSite.ToString().ToLower(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.SameSite.ToString().ToUpper(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.SameSite.ToString(), CrossOriginResourcePolicy.SameSite);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.SameOrigin.ToString().ToLower(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.SameOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.SameOrigin.ToString(), CrossOriginResourcePolicy.SameOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.CrossOrigin.ToString().ToLower(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.CrossOrigin.ToString().ToUpper(), CrossOriginResourcePolicy.CrossOrigin);
            yield return new TestCaseData(CrossOriginResourcePolicy.CrossOrigin, CrossOriginResourcePolicy.CrossOrigin.ToString(), CrossOriginResourcePolicy.CrossOrigin);
        }
    }

    public static IEnumerable<TestCaseData> BooleanTestCases
    {
        get
        {
            yield return new TestCaseData(false, false, false);
            yield return new TestCaseData(false, true, true);
            yield return new TestCaseData(true, false, false);
            yield return new TestCaseData(true, true, true);
        }
    }

    public static IEnumerable<TestCaseData> MaxAgeTestCases
    {
        get
        {
            yield return new TestCaseData(-1, -1, 1);
            yield return new TestCaseData(-1, 0, 1);
            yield return new TestCaseData(-1, 1, 1);
            yield return new TestCaseData(-1, 2, 2);
            yield return new TestCaseData(0, -1, 1);
            yield return new TestCaseData(0, 0, 1);
            yield return new TestCaseData(0, 1, 1);
            yield return new TestCaseData(0, 2, 2);
            yield return new TestCaseData(1, -1, 1);
            yield return new TestCaseData(1, 0, 1);
            yield return new TestCaseData(1, 1, 1);
            yield return new TestCaseData(1, 2, 2);
            yield return new TestCaseData(1, 63071999, 63071999);
            yield return new TestCaseData(1, 63072000, 63072000);
            yield return new TestCaseData(1, 63072001, 63072000);
        }
    }
}
