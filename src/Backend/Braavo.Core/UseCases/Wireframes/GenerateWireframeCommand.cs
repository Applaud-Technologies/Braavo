using MediatR;

namespace Braavo.Core.UseCases.Wireframes;

public record GenerateWireframeCommand(
    Guid DocumentId,
    string? ScreenName,
    string Fidelity = "low"
) : IRequest<WireframeResponse>;

public record WireframeResponse(
    string Html,
    string ScreenName,
    bool Success,
    string? Error = null
);
