using System;

namespace Stott.Security.Optimizely.Features.Audit;

public interface IAuditableEntity
{
    Guid Id { get; set; }
}
