using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ProductSummaryDto>>
{
    private readonly IProductRepository _productRepo;

    public ListProductsHandler(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IReadOnlyList<ProductSummaryDto>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        var products = await _productRepo.GetByOwnerIdAsync(UserId.From(request.UserId), ct);

        return products.Select(p => new ProductSummaryDto(
            p.Id,
            p.Name,
            p.Description,
            p.Status.ToString(),
            p.CompletionPercentage,
            p.UpdatedAt
        )).ToList();
    }
}
