using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepo;

    public CreateProductHandler(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new CreateProductResponse(Guid.Empty, false, "Product name is required");
        }

        var product = Product.Create(
            request.Name,
            request.Description,
            UserId.From(request.UserId)
        );

        if (request.Categories is { Length: > 0 })
        {
            product.UpdateCategories(request.Categories);
        }

        await _productRepo.AddAsync(product, ct);

        return new CreateProductResponse(product.Id, true);
    }
}
