namespace Braavo.Core.Models;

public record LlmRequest(
    string Prompt,
    string? SystemPrompt = null,
    int MaxTokens = 4096,
    double Temperature = 0.7
);
