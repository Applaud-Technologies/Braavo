using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Chat;

public class SendChatMessageHandler : IRequestHandler<SendChatMessageCommand, ChatResponse>
{
    private readonly ILlmProvider _llm;
    private readonly IDocumentRepository _documents;

    private const string SystemPrompt = """
        You are Braavo, an AI assistant that helps product managers create PRDs.
        When given a product idea, generate a structured PRD with these sections:
        - Overview
        - Problem Statement
        - Target Users
        - Features (numbered list)
        - Success Metrics
        - User Stories (as "As a [user], I want [goal] so that [benefit]")

        Use markdown formatting. Be concise but comprehensive.
        """;

    public SendChatMessageHandler(ILlmProvider llm, IDocumentRepository documents)
    {
        _llm = llm;
        _documents = documents;
    }

    public async Task<ChatResponse> Handle(SendChatMessageCommand request, CancellationToken ct)
    {
        var llmRequest = new LlmRequest(
            Prompt: request.Message,
            SystemPrompt: SystemPrompt,
            MaxTokens: 4096,
            Temperature: 0.7
        );

        var response = await _llm.GenerateAsync(llmRequest, ct);

        if (!response.Success)
            return new ChatResponse("", null, false, response.Error);

        // Create or update document with generated content
        Guid documentId;
        if (request.DocumentId.HasValue)
        {
            var doc = await _documents.GetByIdAsync(request.DocumentId.Value, ct);
            if (doc is not null)
            {
                doc.UpdateContent(response.Content);
                await _documents.UpdateAsync(doc, ct);
                documentId = doc.Id;
            }
            else
            {
                documentId = await CreateNewDocument(request, response.Content, ct);
            }
        }
        else
        {
            documentId = await CreateNewDocument(request, response.Content, ct);
        }

        return new ChatResponse(response.Content, documentId, true);
    }

    private async Task<Guid> CreateNewDocument(SendChatMessageCommand request, string content, CancellationToken ct)
    {
        var title = ExtractTitle(content) ?? "Untitled PRD";
        var doc = Document.Create(title, DocumentType.Prd, request.ProjectId, UserId.From(request.UserId));
        doc.UpdateContent(content);
        await _documents.AddAsync(doc, ct);
        return doc.Id;
    }

    private static string? ExtractTitle(string content)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var firstLine = lines.FirstOrDefault()?.Trim();
        if (firstLine?.StartsWith('#') == true)
            return firstLine.TrimStart('#').Trim();
        return null;
    }
}
