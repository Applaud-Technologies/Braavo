using MediatR;

namespace Braavo.Core.UseCases.Products;

public record GetProductQuery(Guid ProductId, Guid UserId) : IRequest<GetProductResponse?>;

public record GetProductResponse(
    Guid Id,
    string Name,
    string Description,
    string Status,
    int Version,
    int CompletionPercentage,
    string Vision,
    string ProblemStatement,
    string ValueProposition,
    string[] TargetMarket,
    string[] BusinessGoals,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<PersonaDto> Personas,
    IReadOnlyList<UserStoryDto> UserStories,
    IReadOnlyList<FeatureDto> Features
);

public record PersonaDto(
    Guid Id,
    string Name,
    string Role,
    string TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
);

public record UserStoryDto(
    Guid Id,
    Guid? PersonaId,
    string AsA,
    string IWant,
    string SoThat,
    string Priority,
    string[] AcceptanceCriteria
);

public record FeatureDto(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Description,
    string Phase,
    string? Effort,
    Guid[] LinkedStoryIds
);
