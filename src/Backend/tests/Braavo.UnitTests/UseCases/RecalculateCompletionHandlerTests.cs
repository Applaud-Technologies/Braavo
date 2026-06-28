using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class RecalculateCompletionHandlerTests
{
    private readonly IProductRepository    _productRepo    = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository    _personaRepo    = Substitute.For<IPersonaRepository>();
    private readonly IUserStoryRepository  _userStoryRepo  = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository    _featureRepo    = Substitute.For<IFeatureRepository>();
    private readonly ITimelineRepository   _timelineRepo   = Substitute.For<ITimelineRepository>();

    private readonly RecalculateCompletionHandler _handler;

    public RecalculateCompletionHandlerTests()
    {
        _handler = new RecalculateCompletionHandler(
            _productRepo,
            _personaRepo,
            _userStoryRepo,
            _featureRepo,
            _timelineRepo);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static Product MakeProduct(
        string vision           = "",
        string problemStatement = "",
        string valueProposition = "",
        string[]? targetMarket  = null)
    {
        var p = Product.Create("Test", "Desc", UserId.New());
        if (!string.IsNullOrWhiteSpace(vision) ||
            !string.IsNullOrWhiteSpace(problemStatement) ||
            !string.IsNullOrWhiteSpace(valueProposition))
        {
            p.UpdateOverview(vision, problemStatement, valueProposition);
        }
        if (targetMarket is { Length: > 0 })
            p.UpdateTargetMarket(targetMarket);
        return p;
    }

    private void SetupEmptyRepositories()
    {
        _personaRepo  .GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<Persona>().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<UserStory>().AsReadOnly());
        _featureRepo  .GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<Feature>().AsReadOnly());
        _timelineRepo .GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<TimelinePhase>().AsReadOnly());
    }

    // ── tests ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsError()
    {
        _productRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns((Product?)null);

        var result = await _handler.Handle(
            new RecalculateCompletionCommand(Guid.NewGuid()), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_EmptyProduct_Returns0Percent()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new RecalculateCompletionCommand(product.Id), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.CompletionPercentage.Should().Be(0);
    }

    [Fact]
    public async Task Handle_AllTextSectionsPresent_NoRelatedEntities_Returns40Percent()
    {
        // vision(10) + problemStatement(10) + valueProposition(10) + targetMarket(10) = 40
        var product = MakeProduct(
            vision:           "Make tasks easy",
            problemStatement: "Tasks are hard",
            valueProposition: "Simple UI",
            targetMarket:     ["SMBs"]);

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new RecalculateCompletionCommand(product.Id), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.CompletionPercentage.Should().Be(40);
    }

    [Fact]
    public async Task Handle_FullyCompleteProduct_Returns100Percent()
    {
        var product = MakeProduct(
            vision:           "Make tasks easy",
            problemStatement: "Tasks are hard",
            valueProposition: "Simple UI",
            targetMarket:     ["SMBs"]);

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        // At least 1 persona
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new[] { Persona.Create(product.Id, "Alice", "Developer") }
                             .ToList().AsReadOnly());

        // At least 3 user stories
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                      .Returns(Enumerable.Range(0, 3)
                               .Select(_ => UserStory.Create(product.Id, "As a user", "do X", "so that Y"))
                               .ToList().AsReadOnly());

        // At least 3 features
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(Enumerable.Range(0, 3)
                             .Select(_ => Feature.Create(product.Id, "Feature", "Desc"))
                             .ToList().AsReadOnly());

        // At least 1 timeline phase
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                     .Returns(new[] { TimelinePhase.Create(product.Id, "MVP", 4) }
                              .ToList().AsReadOnly());

        var result = await _handler.Handle(
            new RecalculateCompletionCommand(product.Id), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.CompletionPercentage.Should().Be(100);
    }

    [Fact]
    public async Task Handle_Persists_UpdatedCompletionPercentage()
    {
        var product = MakeProduct(vision: "My vision");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        await _handler.Handle(new RecalculateCompletionCommand(product.Id), CancellationToken.None);

        await _productRepo.Received(1).UpdateAsync(
            Arg.Is<Product>(p => p.CompletionPercentage == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Only2UserStories_DoesNotGrantUserStoryPoints()
    {
        var product = MakeProduct(vision: "Vision");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new List<Persona>().AsReadOnly());
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new List<Feature>().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                     .Returns(new List<TimelinePhase>().AsReadOnly());

        // Only 2 user stories — threshold is 3
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                      .Returns(Enumerable.Range(0, 2)
                               .Select(_ => UserStory.Create(product.Id, "As a user", "do X", "so that Y"))
                               .ToList().AsReadOnly());

        var result = await _handler.Handle(
            new RecalculateCompletionCommand(product.Id), CancellationToken.None);

        // Only vision (10%) should count
        result.CompletionPercentage.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Only2Features_DoesNotGrantFeaturePoints()
    {
        var product = MakeProduct(vision: "Vision");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new List<Persona>().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                      .Returns(new List<UserStory>().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                     .Returns(new List<TimelinePhase>().AsReadOnly());

        // Only 2 features — threshold is 3
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(Enumerable.Range(0, 2)
                             .Select(_ => Feature.Create(product.Id, "Feature", "Desc"))
                             .ToList().AsReadOnly());

        var result = await _handler.Handle(
            new RecalculateCompletionCommand(product.Id), CancellationToken.None);

        // Only vision (10%) should count
        result.CompletionPercentage.Should().Be(10);
    }
}
