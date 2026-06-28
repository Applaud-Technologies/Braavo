using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public class GetProductHandler : IRequestHandler<GetProductQuery, GetProductResponse?>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IUserStoryRepository _storyRepo;
    private readonly IFeatureRepository _featureRepo;

    public GetProductHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        IUserStoryRepository storyRepo,
        IFeatureRepository featureRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
        _storyRepo = storyRepo;
        _featureRepo = featureRepo;
    }

    public async Task<GetProductResponse?> Handle(GetProductQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return null;

        var personas = await _personaRepo.GetByProductIdAsync(product.Id, ct);
        var stories = await _storyRepo.GetByProductIdAsync(product.Id, ct);
        var features = await _featureRepo.GetByProductIdAsync(product.Id, ct);

        return new GetProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Status.ToString(),
            product.Version,
            product.CompletionPercentage,
            product.Vision,
            product.ProblemStatement,
            product.ValueProposition,
            product.TargetMarket,
            product.BusinessGoals,
            product.CreatedAt,
            product.UpdatedAt,
            personas.Select(p => new PersonaDto(
                p.Id, p.Name, p.Role, p.TechnicalLevel.ToString(),
                p.Goals, p.PainPoints, p.Quote
            )).ToList(),
            stories.Select(s => new UserStoryDto(
                s.Id, s.PersonaId, s.AsA, s.IWant, s.SoThat,
                s.Priority.ToString(), s.AcceptanceCriteria
            )).ToList(),
            features.Select(f => new FeatureDto(
                f.Id, f.ParentId, f.Name, f.Description,
                f.Phase.ToString(), f.Effort?.ToString(), f.LinkedStoryIds
            )).ToList()
        );
    }
}
