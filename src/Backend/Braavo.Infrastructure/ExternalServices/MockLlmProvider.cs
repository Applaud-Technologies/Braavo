using Braavo.Core.Interfaces;
using Braavo.Core.Models;

namespace Braavo.Infrastructure.ExternalServices;

public class MockLlmProvider : ILlmProvider
{
    public Task<LlmResponse> GenerateAsync(LlmRequest request, CancellationToken ct = default)
    {
        var response = new LlmResponse(
            Content: GenerateMockPrd(request.Prompt),
            PromptTokens: request.Prompt.Length / 4,
            CompletionTokens: 500,
            Success: true
        );
        return Task.FromResult(response);
    }

    public Task<IAsyncEnumerable<string>> StreamAsync(LlmRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(StreamMockResponse(request.Prompt));
    }

    private static async IAsyncEnumerable<string> StreamMockResponse(string prompt)
    {
        var content = GenerateMockPrd(prompt);
        var words = content.Split(' ');
        foreach (var word in words)
        {
            yield return word + " ";
            await Task.Delay(20);
        }
    }

    private static string GenerateMockPrd(string prompt)
    {
        return $"""
        # Product Requirements Document

        ## Overview
        Based on your idea: "{prompt.Substring(0, Math.Min(50, prompt.Length))}..."

        ## Problem Statement
        Users need a solution that addresses the core challenge described.

        ## Target Users
        - Primary: End users who will benefit from this solution
        - Secondary: Administrators and power users

        ## Features
        1. **Core Feature** - The main functionality
        2. **Supporting Feature** - Enhances the core experience
        3. **Analytics** - Track usage and outcomes

        ## Success Metrics
        - User adoption rate > 50%
        - Task completion time reduced by 30%
        - User satisfaction score > 4.0/5.0

        ## User Stories
        - As a user, I want to easily accomplish my goal
        - As a user, I want to track my progress
        - As an admin, I want to manage user access
        """;
    }
}
