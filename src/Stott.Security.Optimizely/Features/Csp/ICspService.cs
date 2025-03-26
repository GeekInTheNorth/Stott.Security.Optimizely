using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Csp;

public interface ICspService
{
    Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders(IContentSecurityPolicyPage? currentPage);
}