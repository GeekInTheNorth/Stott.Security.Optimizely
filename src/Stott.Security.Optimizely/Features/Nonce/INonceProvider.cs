namespace Stott.Security.Optimizely.Features.Nonce;

public interface INonceProvider
{
    string? GetNonce();
}