﻿namespace Stott.Security.Optimizely.MigrationSteps;

using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.DataAbstraction.Migration;
using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Permissions.Repository;

public class SetUpDefaultSourcesMigrationStep : MigrationStep
{
    public override void AddChanges()
    {
        try
        {
            CreateCmsDefaults();
        }
        catch(Exception)
        {
        }
    }

    private static void CreateCmsDefaults()
    {
        var repository = ServiceLocator.Current.GetInstance<ICspPermissionRepository>();

        var sources = repository.GetAsync().Result;
        if (sources == null || sources.Any())
        {
            return;
        }

        var selfRequirements = new List<string>
        {
            CspConstants.Directives.ChildSource,
            CspConstants.Directives.ConnectSource,
            CspConstants.Directives.FontSource,
            CspConstants.Directives.FrameSource,
            CspConstants.Directives.ImageSource,
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.ScriptSourceElement,
            CspConstants.Directives.StyleSource,
            CspConstants.Directives.StyleSourceElement
        };

        var unsafeInlineRequirements = new List<string>
        {
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.ScriptSourceElement,
            CspConstants.Directives.StyleSource,
            CspConstants.Directives.StyleSourceElement
        };

        var noneRequirements = new List<string> { CspConstants.Directives.DefaultSource };
        var unsafeEvalRequirements = new List<string> { CspConstants.Directives.ScriptSource };
        var fontRequirements = new List<string> { CspConstants.Directives.FontSource };
        var imageRequirements = new List<string> { CspConstants.Directives.ImageSource };

        repository.SaveAsync(Guid.Empty, CspConstants.Sources.None, noneRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.Self, selfRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.UnsafeInline, unsafeInlineRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.UnsafeEval, unsafeEvalRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://*.cloudfront.net/graphik/", fontRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://*.cloudfront.net/lato/", fontRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.SchemeData, imageRequirements, "System").Wait();
    }
}