using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public class NonceSettings
{
    public bool IsEnabled { get; set; }

    public List<string>? Directives { get; set; }
}