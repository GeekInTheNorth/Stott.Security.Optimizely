using System;

namespace Stott.Security.Core.Features.Permissions.List
{
    public class CspPermissionListModel
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public string Directives { get; set; }
    }
}
