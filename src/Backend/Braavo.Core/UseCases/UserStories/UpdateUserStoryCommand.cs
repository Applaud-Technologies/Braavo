using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record UpdateUserStoryCommand(
    Guid StoryId,
    Guid ProductId,
    Guid UserId,
    string AsA,
    string IWant,
    string SoThat,
    StoryPriority Priority,
    string[] AcceptanceCriteria,
    Guid? PersonaId = null
) : IRequest<UpdateUserStoryResult>;

public record UpdateUserStoryResult(bool Success, string? Error = null);

public class UpdateUserStoryHandler : IRequestHandler<UpdateUserStoryCommand, UpdateUserStoryResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IUserStoryRepository _storyRepo;
    private readonly IMediator _mediator;

    public UpdateUserStoryHandler(IProductRepository productRepo, IUserStoryRepository storyRepo, IMediator mediator)
    {
        _productRepo = productRepo;
        _storyRepo   = storyRepo;
        _mediator    = mediator;
    }

    public async Task<UpdateUserStoryResult> Handle(UpdateUserStoryCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new UpdateUserStoryResult(false, "Product not found");

        var story = await _storyRepo.GetByIdAsync(request.StoryId, ct);
        if (story is null || story.ProductId != request.ProductId)
            return new UpdateUserStoryResult(false, "User story not found");

        if (string.IsNullOrWhiteSpace(request.AsA))
            return new UpdateUserStoryResult(false, "AsA is required");

        if (string.IsNullOrWhiteSpace(request.IWant))
            return new UpdateUserStoryResult(false, "IWant is required");

        if (string.IsNullOrWhiteSpace(request.SoThat))
            return new UpdateUserStoryResult(false, "SoThat is required");

        story.Update(request.AsA, request.IWant, request.SoThat, request.Priority);
        story.SetAcceptanceCriteria(request.AcceptanceCriteria);
        story.LinkToPersona(request.PersonaId);

        await _storyRepo.UpdateAsync(story, ct);

        await _mediator.Send(new RecalculateCompletionCommand(request.ProductId), ct);

        return new UpdateUserStoryResult(true);
    }
}
