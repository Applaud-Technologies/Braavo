namespace Braavo.Core.Models;

public record LlmResponse(
    string Content,
    int PromptTokens,
    int CompletionTokens,
    bool Success,
    string? Error = null
);
