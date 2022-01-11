using System;

namespace Stott.Optimizely.Csp.Features.Permissions.Delete
{
    public interface IDeleteCspPermissionsCommand
    {
        void Execute(Guid id);
    }
}
