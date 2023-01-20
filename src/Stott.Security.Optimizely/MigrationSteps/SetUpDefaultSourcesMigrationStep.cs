namespace Stott.Security.Optimizely.MigrationSteps;

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
            CspConstants.Directives.DefaultSource,
            CspConstants.Directives.FrameSource,
            CspConstants.Directives.ImageSource,
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.StyleSource,
            CspConstants.Directives.FontSource
        };

        var unsafeInlineRequirements = new List<string>
        {
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.StyleSource
        };

        var applicationInsightsRequirements = new List<string>
        {
            CspConstants.Directives.ConnectSource,
            CspConstants.Directives.ScriptSource
        };

        var scriptSourceOnly = new List<string> { CspConstants.Directives.ScriptSource };

        repository.SaveAsync(Guid.Empty, CspConstants.Sources.Self, selfRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.UnsafeInline, unsafeInlineRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.UnsafeEval, scriptSourceOnly, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://dc.services.visualstudio.com", applicationInsightsRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://*.msecnd.net", scriptSourceOnly, "System").Wait();
    }
}