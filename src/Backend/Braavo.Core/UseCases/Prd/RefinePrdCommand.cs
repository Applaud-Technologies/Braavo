using MediatR;

namespace Braavo.Core.UseCases.Prd;

public record RefinePrdCommand(
    Guid DocumentId,
    string Instruction,
    Guid UserId
) : IRequest<RefinePrdResponse>;

public record RefinePrdResponse(
    string Content,
    int NewVersion,
    bool Success,
    string? Error = null
);
