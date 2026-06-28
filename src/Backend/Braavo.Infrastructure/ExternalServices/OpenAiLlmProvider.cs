using System.Runtime.CompilerServices;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace Braavo.Infrastructure.ExternalServices;

public class OpenAiLlmProvider : ILlmProvider
{
    private readonly ChatClient _client;

    public OpenAiLlmProvider(IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API key not configured");
        var model = config["OpenAI:Model"] ?? "gpt-4o";
        _client = new ChatClient(model, apiKey);
    }

    public async Task<LlmResponse> GenerateAsync(LlmRequest request, CancellationToken ct = default)
    {
        try
        {
            var messages = new List<ChatMessage>();

            if (!string.IsNullOrEmpty(request.SystemPrompt))
                messages.Add(new SystemChatMessage(request.SystemPrompt));

            messages.Add(new UserChatMessage(request.Prompt));

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = request.MaxTokens,
                Temperature = (float)request.Temperature
            };

            var completion = await _client.CompleteChatAsync(messages, options, ct);

            return new LlmResponse(
                Content: completion.Value.Content[0].Text,
                PromptTokens: completion.Value.Usage.InputTokenCount,
                CompletionTokens: completion.Value.Usage.OutputTokenCount,
                Success: true
            );
        }
        catch (Exception ex)
        {
            return new LlmResponse("", 0, 0, false, ex.Message);
        }
    }

    public Task<IAsyncEnumerable<string>> StreamAsync(LlmRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(StreamChatAsync(request, ct));
    }

    private async IAsyncEnumerable<string> StreamChatAsync(LlmRequest request, [EnumeratorCancellation] CancellationToken ct)
    {
        var messages = new List<ChatMessage>();

        if (!string.IsNullOrEmpty(request.SystemPrompt))
            messages.Add(new SystemChatMessage(request.SystemPrompt));

        messages.Add(new UserChatMessage(request.Prompt));

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = request.MaxTokens,
            Temperature = (float)request.Temperature
        };

        await foreach (var update in _client.CompleteChatStreamingAsync(messages, options, ct))
        {
            foreach (var part in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(part.Text))
                    yield return part.Text;
            }
        }
    }
}
