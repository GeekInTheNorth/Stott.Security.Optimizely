namespace Stott.Security.Optimizely.MigrationSteps;

using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.DataAbstraction.Migration;
using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

public class SetUpDefaultSourcesMigrationStep : MigrationStep
{
    private bool _hasStarted = false;

    public override void AddChanges()
    {
        try
        {
            if (_hasStarted)
            {
                return;
            }

            _hasStarted = true;

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
            CspConstants.Directives.DefaultSource,
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

        var unsafeEvalRequirements = new List<string> { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement };
        var fontRequirements = new List<string> { CspConstants.Directives.FontSource };
        var imageRequirements = new List<string> { CspConstants.Directives.ImageSource };

        var optimizelyRequirements = new List<string>
        {
            CspConstants.Directives.ConnectSource,
            CspConstants.Directives.FrameSource,
            CspConstants.Directives.ImageSource,
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.ScriptSourceElement,
            CspConstants.Directives.StyleSource,
            CspConstants.Directives.StyleSourceElement
        };

        repository.SaveAsync(Guid.Empty, CspConstants.Sources.Self, selfRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.UnsafeInline, unsafeInlineRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.UnsafeEval, unsafeEvalRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://*.cloudfront.net/graphik/", fontRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://*.cloudfront.net/lato/", fontRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, CspConstants.Sources.SchemeData, imageRequirements, "System").Wait();
        repository.SaveAsync(Guid.Empty, "https://*.optimizely.com/", optimizelyRequirements, "System").Wait();
    }
}