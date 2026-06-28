using Braavo.Core.Models;

namespace Braavo.Core.Interfaces;

public interface ILlmProvider
{
    Task<LlmResponse> GenerateAsync(LlmRequest request, CancellationToken ct = default);
    Task<IAsyncEnumerable<string>> StreamAsync(LlmRequest request, CancellationToken ct = default);
}
