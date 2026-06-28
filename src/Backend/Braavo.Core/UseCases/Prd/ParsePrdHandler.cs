using MediatR;
using Braavo.Core.Interfaces;
using Braavo.Core.Models;

namespace Braavo.Core.UseCases.Prd;

public class ParsePrdHandler : IRequestHandler<ParsePrdCommand, PrdContent?>
{
    private readonly IDocumentRepository _documents;

    public ParsePrdHandler(IDocumentRepository documents) => _documents = documents;

    public async Task<PrdContent?> Handle(ParsePrdCommand request, CancellationToken cancellationToken)
    {
        var document = await _documents.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null) return null;
        return PrdParser.Parse(document.Content);
    }
}
