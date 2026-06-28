using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Core.UseCases.Products;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record ListUserStoriesQuery(Guid ProductId, Guid UserId) : IRequest<ListUserStoriesResult>;

public record ListUserStoriesResult(
    bool Success,
    IReadOnlyList<UserStoryDto>? Stories = null,
    string? Error = null
);

public class ListUserStoriesHandler : IRequestHandler<ListUserStoriesQuery, ListUserStoriesResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IUserStoryRepository _storyRepo;

    public ListUserStoriesHandler(IProductRepository productRepo, IUserStoryRepository storyRepo)
    {
        _productRepo = productRepo;
        _storyRepo = storyRepo;
    }

    public async Task<ListUserStoriesResult> Handle(ListUserStoriesQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new ListUserStoriesResult(false, Error: "Product not found");

        var stories = await _storyRepo.GetByProductIdAsync(request.ProductId, ct);

        var dtos = stories.Select(s => new UserStoryDto(
            s.Id, s.PersonaId, s.AsA, s.IWant, s.SoThat,
            s.Priority.ToString(), s.AcceptanceCriteria
        )).ToList();

        return new ListUserStoriesResult(true, dtos);
    }
}
