using System;
using System.Collections.Generic;
using System.Linq;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Csp.Dtos;

namespace Stott.Security.Optimizely.Features.Csp;

/// <summary>
/// This class is intended to split a CSP header into multiple headers
/// to avoid exceeding the maximum header size.
/// It will group directives together based on their type.
/// It will also ensure that the default-src sources are used as the primary fallback for any
/// directice inheritance chain to avoid issues with the most restrictive header.
/// This is where a restrictive default-src in one header will override
/// a more permissive directive in another header.
/// </summary>
public static class CspOptimizer
{
    private static readonly string[] FrameSourceDirectives = new[]
    {
        CspConstants.Directives.FencedFrameSource,
        CspConstants.Directives.FrameSource,
        CspConstants.Directives.WorkerSource,
        CspConstants.Directives.ChildSource
    };

    private static readonly string[] ScriptSourceDirectives = new[]
    {
        CspConstants.Directives.ScriptSourceElement,
        CspConstants.Directives.ScriptSourceAttribute,
        CspConstants.Directives.ScriptSource
    };

    private static readonly string[] StyleSourceDirectives = new[]
    {
        CspConstants.Directives.StyleSourceElement,
        CspConstants.Directives.StyleSourceAttribute,
        CspConstants.Directives.StyleSource
    };

    private static readonly string[] OtherFetchDirectives = new[]
    {
        CspConstants.Directives.ConnectSource,
        CspConstants.Directives.FontSource,
        CspConstants.Directives.ImageSource,
        CspConstants.Directives.ManifestSource,
        CspConstants.Directives.MediaSource,
        CspConstants.Directives.ObjectSource,
        CspConstants.Directives.PreFetchSource
    };

    private static readonly string[] StandaloneDirectives = new[]
    {
        CspConstants.Directives.BaseUri,
        CspConstants.Directives.FormAction,
        CspConstants.Directives.FrameAncestors,
        CspConstants.Directives.UpgradeInsecureRequests,
        CspConstants.Directives.Sandbox
    };

    internal static List<List<CspDirectiveDto>> GroupDirectives(List<CspDirectiveDto> cspDirectives)
    {
        if (!ExceedsSize(cspDirectives, CspConstants.SplitThreshold))
        {
            return new List<List<CspDirectiveDto>> { cspDirectives };
        }

        var forceSimplification = ExceedsSize(cspDirectives, CspConstants.SimplifyThreshold);
        var optimizedDirectives = new List<List<CspDirectiveDto>>();

        // Get Default Source to use as a fallback, assume 'self' if not present
        var defaultSrc = cspDirectives.FirstOrDefault(d => d.Directive == CspConstants.Directives.DefaultSource)
            ?? new CspDirectiveDto(CspConstants.Directives.DefaultSource, new List<string> { CspConstants.Sources.Self });

        var reportTo = cspDirectives.FirstOrDefault(d => d.Directive == CspConstants.Directives.ReportTo);

        optimizedDirectives.Add(GetGroupedFetchDirectives(cspDirectives, defaultSrc, reportTo, FrameSourceDirectives, CspConstants.Directives.ChildSource, forceSimplification));
        optimizedDirectives.Add(GetGroupedFetchDirectives(cspDirectives, defaultSrc, reportTo, ScriptSourceDirectives, CspConstants.Directives.ScriptSource, forceSimplification));
        optimizedDirectives.Add(GetGroupedFetchDirectives(cspDirectives, defaultSrc, reportTo, StyleSourceDirectives, CspConstants.Directives.StyleSource, forceSimplification));
        optimizedDirectives.AddRange(GetGroupedFetchDirectives(cspDirectives, defaultSrc, reportTo, OtherFetchDirectives));
        optimizedDirectives.AddRange(GetGroupedStandaloneDirectives(cspDirectives, reportTo, StandaloneDirectives));

        if (ExceedsSize(optimizedDirectives, CspConstants.TerminalThreshold))
        {
            optimizedDirectives.Clear();
        }
            
        return optimizedDirectives;
    }

    private static List<CspDirectiveDto> GetGroupedFetchDirectives(
        List<CspDirectiveDto> cspDirectives,
        CspDirectiveDto defaultSource,
        CspDirectiveDto? reportTo,
        string[] directiveNames,
        string primaryFallback,
        bool forceSimplification)
    {
        var matchingDirectives = cspDirectives
            .Where(d => directiveNames.Contains(d.Directive))
            .ToList();
        var allSources = matchingDirectives.SelectMany(d => d.Sources).Distinct().ToList();

        matchingDirectives.TryAdd(reportTo);

        if (forceSimplification || ExceedsSize(matchingDirectives, CspConstants.SplitThreshold))
        {
            matchingDirectives.Clear();
            matchingDirectives.Add(new CspDirectiveDto(primaryFallback, allSources));
            matchingDirectives.TryAdd(reportTo);
        }

        if (matchingDirectives.Count == 0 || !matchingDirectives.Any(x => x.Directive == primaryFallback))
        {
            matchingDirectives.Add(new CspDirectiveDto(primaryFallback, defaultSource.Sources));
        }

        return matchingDirectives;
    }

    private static List<List<CspDirectiveDto>> GetGroupedFetchDirectives(
        List<CspDirectiveDto> cspDirectives,
        CspDirectiveDto defaultSource,
        CspDirectiveDto? reportTo,
        string[] directiveNames)
    {
        var matchingDirectives = new List<CspDirectiveDto>();
        foreach (var directiveName in directiveNames)
        {
            var directive = cspDirectives.FirstOrDefault(d => d.Directive == directiveName)
                        ?? new CspDirectiveDto(directiveName, defaultSource.Sources);
            matchingDirectives.Add(directive);
        }

        matchingDirectives.TryAdd(reportTo);

        return GroupWithHeaderSizeLimits(matchingDirectives);
    }

    private static List<List<CspDirectiveDto>> GetGroupedStandaloneDirectives(List<CspDirectiveDto> cspDirectives, CspDirectiveDto? reportTo, string[] directiveNames)
    {
        var matchingDirectives = cspDirectives
            .Where(d => directiveNames.Contains(d.Directive))
            .ToList();

        matchingDirectives.TryAdd(reportTo);

        return GroupWithHeaderSizeLimits(matchingDirectives);
    }

    private static List<List<CspDirectiveDto>> GroupWithHeaderSizeLimits(List<CspDirectiveDto> cspDirectives)
    {
        if (!ExceedsSize(cspDirectives, CspConstants.SplitThreshold))
        {
            return new List<List<CspDirectiveDto>> { cspDirectives };
        }

        // Report-to must be in each header
        var reportTo = cspDirectives.FirstOrDefault(d => d.Directive == CspConstants.Directives.ReportTo);
        var otherDirectives = cspDirectives.Where(d => d.Directive != CspConstants.Directives.ReportTo).ToList();

        // At this point we will break header size with just these directives
        // I want to group these into as few a groups as possible where no group
        // has a sum of PredictedSize that exceeds the max header size
        // This is a greedy algorithm, it will not always produce the optimal solution
        // but it will produce a solution that is better than the original
        var groupedDirectives = new List<List<CspDirectiveDto>>();
        var currentGroup = CreateNewDirectiveList(reportTo);
        var currentGroupSize = 0;

        foreach (var directive in otherDirectives)
        {
            if (currentGroupSize + directive.PredictedSize > CspConstants.SplitThreshold)
            {
                groupedDirectives.Add(currentGroup);
                currentGroup = CreateNewDirectiveList(reportTo);
                currentGroupSize = 0;
            }

            currentGroup.Add(directive);
            currentGroupSize += directive.PredictedSize;
        }

        if (currentGroup.Count > 0)
        {
            groupedDirectives.Add(currentGroup);
        }

        return groupedDirectives;
    }

    private static List<CspDirectiveDto> CreateNewDirectiveList(CspDirectiveDto? reportTo)
    {
        return reportTo is null ? new List<CspDirectiveDto>() : new List<CspDirectiveDto> { reportTo };
    }

    private static bool ExceedsSize(IList<CspDirectiveDto> directives, int limit)
    {
        // Linq .Sum() will enumerate the entire collection while this method has the potential to exit early.
        var totalSize = 0;
        foreach (var directive in directives)
        {
            totalSize += directive.PredictedSize;
            if (totalSize > limit)
            {
                return true;
            }
        }
        return false;
    }

    private static bool ExceedsSize(List<List<CspDirectiveDto>> directiveGroups, int limit)
    {
        // Linq .Sum() will enumerate the entire collection while this method has the potential to exit early.
        var totalSize = 0;
        foreach (var directiveGroup in directiveGroups)
        {
            foreach (var directive in directiveGroup)
            {
                totalSize += directive.PredictedSize;
                if (totalSize > limit)
                {
                    return true;
                }
            }
        }

        return false;
    }
}