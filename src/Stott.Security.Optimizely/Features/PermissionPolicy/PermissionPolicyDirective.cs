namespace Stott.Security.Optimizely.Features.PermissionPolicy;

/// <summary>
/// Describes a single Permissions-Policy directive along with the metadata required to present it to a user.
/// </summary>
/// <param name="Name">The directive name as defined by the Permissions Policy specification.</param>
/// <param name="Title">A human friendly name for the directive.</param>
/// <param name="Description">A human friendly explanation of what the directive controls.</param>
/// <param name="IsDeprecated">Indicates the directive has been deprecated or removed from the specification.</param>
public sealed record PermissionPolicyDirective(string Name, string Title, string Description, bool IsDeprecated = false);
