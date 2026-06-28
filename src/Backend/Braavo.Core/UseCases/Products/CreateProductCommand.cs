using MediatR;

namespace Braavo.Core.UseCases.Products;

public record CreateProductCommand(
    string Name,
    string Description,
    Guid UserId,
    string[]? Categories = null
) : IRequest<CreateProductResponse>;

public record CreateProductResponse(
    Guid ProductId,
    bool Success,
    string? Error = null
);
