namespace Stott.Security.Optimizely.Features.Audit;

using System;

public interface IAuditableEntity
{
    Guid Id { get; set; }

    DateTime Modified { get; set; }

    string ModifiedBy { get; set; }
}