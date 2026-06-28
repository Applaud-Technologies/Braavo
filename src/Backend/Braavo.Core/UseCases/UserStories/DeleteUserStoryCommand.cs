using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record DeleteUserStoryCommand(
    Guid StoryId,
    Guid ProductId,
    Guid UserId
) : IRequest<DeleteUserStoryResult>;

public record DeleteUserStoryResult(bool Success, string? Error = null);

public class DeleteUserStoryHandler : IRequestHandler<DeleteUserStoryCommand, DeleteUserStoryResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IUserStoryRepository _storyRepo;

    public DeleteUserStoryHandler(IProductRepository productRepo, IUserStoryRepository storyRepo)
    {
        _productRepo = productRepo;
        _storyRepo = storyRepo;
    }

    public async Task<DeleteUserStoryResult> Handle(DeleteUserStoryCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new DeleteUserStoryResult(false, "Product not found");

        var story = await _storyRepo.GetByIdAsync(request.StoryId, ct);
        if (story is null || story.ProductId != request.ProductId)
            return new DeleteUserStoryResult(false, "User story not found");

        await _storyRepo.DeleteAsync(request.StoryId, ct);

        return new DeleteUserStoryResult(true);
    }
}
