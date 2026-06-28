using MediatR;

namespace Braavo.Core.UseCases.Wireframes;

public record GenerateWireframeCommand(
    Guid DocumentId,
    Guid UserId,
    string? ScreenName = null,
    string Fidelity = "medium"
) : IRequest<WireframeResponse>;

public record WireframeResponse(
    string Html,
    string ScreenName,
    bool Success,
    string? Error = null
);
