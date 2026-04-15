namespace Stott.Security.Optimizely.Common;

/// <summary>
/// Lightweight state model used to cache whether a given context (Site/Host)
/// has an explicit record stored against it rather than relying on inheritance.
/// </summary>
public class ContextStateModel
{
    public bool Exists { get; set; }
}
