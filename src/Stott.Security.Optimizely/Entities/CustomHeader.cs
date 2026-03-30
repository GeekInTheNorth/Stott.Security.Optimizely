#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.CustomHeaders;

/// <summary>
/// Represents a custom HTTP header configuration.
/// </summary>
[Table("tbl_StottV7_CustomHeader")]
[Index(nameof(HeaderName), nameof(AppId), nameof(HostName), IsUnique = false, Name = "idx_StottV7_CustomHeader_LookUp")]
public class CustomHeader : IAuditableEntity, ICustomHeader
{
    /// <summary>
    /// Gets or sets the unique identifier for the custom header.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the HTTP header.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string HeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the behavior for the header (Add or Remove).
    /// </summary>
    public CustomHeaderBehavior Behavior { get; set; }

    /// <summary>
    /// Gets or sets the value of the header (required when Behavior is Add).
    /// </summary>
    public string HeaderValue { get; set; }

    /// <summary>
    /// Gets or sets the application identifier for multi-site support. Null indicates global scope.
    /// </summary>
    [MaxLength(200)]
    public string AppId { get; set; }

    /// <summary>
    /// Gets or sets the host name for multi-site support. Null indicates all hosts within the application.
    /// </summary>
    [MaxLength(200)]
    public string HostName { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the header was last modified.
    /// </summary>
    public DateTime Modified { get; set; }

    /// <summary>
    /// Gets or sets the username of the person who last modified the header.
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;
}
