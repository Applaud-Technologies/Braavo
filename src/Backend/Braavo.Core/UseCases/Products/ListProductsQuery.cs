using MediatR;

namespace Braavo.Core.UseCases.Products;

public record ListProductsQuery(Guid UserId) : IRequest<IReadOnlyList<ProductSummaryDto>>;

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Description,
    string Status,
    int CompletionPercentage,
    DateTime UpdatedAt
);
