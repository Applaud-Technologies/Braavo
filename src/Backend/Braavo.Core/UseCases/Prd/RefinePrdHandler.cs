using Braavo.Core.Interfaces;
using Braavo.Core.Models;
using MediatR;

namespace Braavo.Core.UseCases.Prd;

public class RefinePrdHandler : IRequestHandler<RefinePrdCommand, RefinePrdResponse>
{
    private readonly ILlmProvider _llm;
    private readonly IDocumentRepository _documents;

    private const string RefineSystemPrompt = """
        You are refining an existing PRD. The user will provide the current PRD content
        and an instruction for what to change or add.

        Return the COMPLETE updated PRD with the requested changes incorporated.
        Maintain the existing structure and formatting.
        Do not remove existing content unless explicitly asked.
        """;

    public RefinePrdHandler(ILlmProvider llm, IDocumentRepository documents)
    {
        _llm = llm;
        _documents = documents;
    }

    public async Task<RefinePrdResponse> Handle(RefinePrdCommand request, CancellationToken ct)
    {
        var document = await _documents.GetByIdAsync(request.DocumentId, ct);
        if (document is null)
            return new RefinePrdResponse("", 0, false, "Document not found");

        if (document.CreatedBy.Value != request.UserId)
            return new RefinePrdResponse("", 0, false, "Forbidden");

        var prompt = $"""
            Current PRD:
            {document.Content}

            ---

            User instruction: {request.Instruction}

            Please update the PRD according to the instruction above.
            """;

        var llmRequest = new LlmRequest(prompt, RefineSystemPrompt, 4096, 0.7);
        var response = await _llm.GenerateAsync(llmRequest, ct);

        if (!response.Success)
            return new RefinePrdResponse("", document.Version, false, response.Error);

        document.UpdateContent(response.Content);
        await _documents.UpdateAsync(document, ct);

        return new RefinePrdResponse(response.Content, document.Version, true);
    }
}
