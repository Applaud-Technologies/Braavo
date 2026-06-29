using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record CreateUserStoryCommand(
    Guid ProductId,
    Guid UserId,
    string AsA,
    string IWant,
    string SoThat,
    Guid? PersonaId = null
) : IRequest<CreateUserStoryResult>;

public record CreateUserStoryResult(
    Guid StoryId,
    bool Success,
    string? Error = null
);

public class CreateUserStoryHandler : IRequestHandler<CreateUserStoryCommand, CreateUserStoryResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IUserStoryRepository _storyRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IMediator _mediator;

    public CreateUserStoryHandler(
        IProductRepository productRepo,
        IUserStoryRepository storyRepo,
        IFeatureRepository featureRepo,
        IMediator mediator)
    {
        _productRepo = productRepo;
        _storyRepo   = storyRepo;
        _featureRepo = featureRepo;
        _mediator    = mediator;
    }

    public async Task<CreateUserStoryResult> Handle(CreateUserStoryCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new CreateUserStoryResult(Guid.Empty, false, "Product not found");

        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);
        if (features.Count == 0)
            return new CreateUserStoryResult(Guid.Empty, false, "At least one feature is required before creating user stories");

        if (string.IsNullOrWhiteSpace(request.AsA))
            return new CreateUserStoryResult(Guid.Empty, false, "AsA is required");

        if (string.IsNullOrWhiteSpace(request.IWant))
            return new CreateUserStoryResult(Guid.Empty, false, "IWant is required");

        if (string.IsNullOrWhiteSpace(request.SoThat))
            return new CreateUserStoryResult(Guid.Empty, false, "SoThat is required");

        var story = UserStory.Create(request.ProductId, request.AsA, request.IWant, request.SoThat);

        if (request.PersonaId.HasValue)
            story.LinkToPersona(request.PersonaId);

        await _storyRepo.AddAsync(story, ct);

        await _mediator.Send(new RecalculateCompletionCommand(request.ProductId), ct);

        return new CreateUserStoryResult(story.Id, true);
    }
}
