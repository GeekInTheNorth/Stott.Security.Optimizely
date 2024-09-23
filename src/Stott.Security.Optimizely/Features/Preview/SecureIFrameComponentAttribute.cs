using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Authorization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ViewComposition;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Preview;

/// <summary>
/// This override for <see cref="IFrameComponentAttribute"/> resolves the allowed roles based on the authorization policy for the Stott Security module.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SecureIFrameComponentAttribute : IFrameComponentAttribute
{
    public SecureIFrameComponentAttribute() : base()
    {
        try
        {
            var authorizationOptions = ServiceLocator.Current.GetService(typeof(IOptions<AuthorizationOptions>)) as IOptions<AuthorizationOptions>;
            var policy = authorizationOptions?.Value?.GetPolicy(CspConstants.AuthorizationPolicy);
            var roles = policy?.Requirements?.OfType<RolesAuthorizationRequirement>().ToList();
            var roleNames = roles?.SelectMany(x => x.AllowedRoles).ToList() ?? new List<string> { Roles.WebAdmins, Roles.CmsAdmins, Roles.WebAdmins };

            AllowedRoles = string.Join(',', roleNames);
        }
        catch(Exception)
        {
        }
    }
}