using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Tools.Models;

public sealed class CspSourceMigrationModel
{
    public string? Source { get; set; }

    public List<string>? Directives { get; set; }
}