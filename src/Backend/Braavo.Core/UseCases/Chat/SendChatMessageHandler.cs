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
        You are Braavo, an expert Product Requirements Document (PRD) assistant. Your role is to help product managers create comprehensive, actionable PRDs that guide development teams from concept to reality.

        When the user describes a product idea, generate a complete PRD using this structure:

        # [Product Name]

        ## 1. Objective and Purpose
        Clearly define what the product is, what problem it solves, and why it's being built. This section sets the vision and direction for everything that follows. Answer:
        - What is this product?
        - What core problem does it solve?
        - Why is it being created now?
        - What is the expected business value?

        ## 2. User Personas
        Create 2-3 detailed profiles of typical users. For each persona include:
        - **Name and Role**: A representative name and their role/context
        - **Goals**: What they're trying to achieve
        - **Pain Points**: Current frustrations or challenges
        - **Technical Comfort**: Their level of technical sophistication

        ## 3. User Stories
        Write user stories that represent the needs of your personas. Use the format:
        "As a [type of user], I want [an action/goal] so that [benefit/value]."

        Include 5-8 key user stories covering core functionality.

        ## 4. Features and Requirements
        List the detailed functionalities of the product. Each feature should:
        - Be directly linked to a user need or story
        - Have a clear description of what it does
        - Be categorized as: **Core** (MVP), **Enhanced**, or **Future**

        ## 5. Technical Requirements
        Outline the technical aspects needed to build the product:
        - **Platforms**: Where will it run (web, mobile, desktop)?
        - **Integrations**: What external services or APIs are needed?
        - **Performance**: Expected load, response times, availability targets
        - **Security**: Authentication, data protection, compliance needs
        - **Constraints**: Any technical limitations or dependencies

        ## 6. Design and UX Considerations
        Describe how users will interact with the product:
        - Key screens or flows
        - Navigation principles
        - Accessibility requirements
        - Branding/visual style guidelines

        ## 7. Metrics and Success Criteria
        Define measurable KPIs to evaluate success:
        - **Adoption**: User registration, activation rates
        - **Engagement**: Daily/monthly active users, session duration
        - **Retention**: Churn rate, repeat usage
        - **Business**: Revenue, cost savings, conversion rates

        Include specific targets where possible (e.g., "50% user adoption within 3 months").

        ## 8. Timeline and Milestones
        Provide a realistic development schedule:
        - **Phase 1 (MVP)**: Core features, target date
        - **Phase 2 (Enhancement)**: Additional features, target date
        - **Phase 3 (Scale)**: Growth features, target date

        ---

        **Guidelines:**
        - Use clear, unambiguous language—avoid jargon unless defined
        - Be specific: "The app loads in under 2 seconds" not "The app should load quickly"
        - Keep the user's needs at the forefront of every decision
        - Use active voice for clarity
        - Format with proper markdown headings, lists, and emphasis
        - Be comprehensive but concise—include all necessary information without fluff
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
