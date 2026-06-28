using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class ValidateSectionsHandlerTests
{
    private readonly IProductRepository   _productRepo   = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository   _personaRepo   = Substitute.For<IPersonaRepository>();
    private readonly IUserStoryRepository _userStoryRepo = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository   _featureRepo   = Substitute.For<IFeatureRepository>();
    private readonly ITimelineRepository  _timelineRepo  = Substitute.For<ITimelineRepository>();

    private readonly ValidateSectionsHandler _handler;

    public ValidateSectionsHandlerTests()
    {
        _handler = new ValidateSectionsHandler(
            _productRepo,
            _personaRepo,
            _userStoryRepo,
            _featureRepo,
            _timelineRepo);
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private static Product MakeProduct(
        string vision           = "",
        string problemStatement = "")
    {
        var p = Product.Create("Test", "Desc", UserId.New());
        if (!string.IsNullOrWhiteSpace(vision) || !string.IsNullOrWhiteSpace(problemStatement))
            p.UpdateOverview(vision, problemStatement, "");
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
    public async Task Handle_ProductNotFound_ReturnsNull()
    {
        _productRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns((Product?)null);

        var result = await _handler.Handle(
            new ValidateSectionsQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_EmptyProduct_AllSectionsInvalid()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Validations.Should().HaveCount(5);
        result.Validations.Should().AllSatisfy(v => v.IsValid.Should().BeFalse());
    }

    [Fact]
    public async Task Handle_Overview_ValidWhenBothFieldsPresent()
    {
        var product = MakeProduct(vision: "My vision", problemStatement: "My problem");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var overview = result!.Validations.Single(v => v.Section == "Overview");
        overview.IsValid.Should().BeTrue();
        overview.Warnings.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Overview_InvalidWhenVisionMissing()
    {
        var product = MakeProduct(vision: "", problemStatement: "My problem");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var overview = result!.Validations.Single(v => v.Section == "Overview");
        overview.IsValid.Should().BeFalse();
        overview.Warnings.Should().ContainSingle(w => w.Contains("Vision"));
    }

    [Fact]
    public async Task Handle_Overview_InvalidWhenProblemStatementMissing()
    {
        var product = MakeProduct(vision: "My vision", problemStatement: "");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var overview = result!.Validations.Single(v => v.Section == "Overview");
        overview.IsValid.Should().BeFalse();
        overview.Warnings.Should().ContainSingle(w => w.Contains("Problem statement"));
    }

    [Fact]
    public async Task Handle_Personas_ValidWhenAtLeastOneWithNameAndRole()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new[] { Persona.Create(product.Id, "Alice", "Developer") }
                             .ToList().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<UserStory>().AsReadOnly());
        _featureRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Feature>().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                      .Returns(new List<TimelinePhase>().AsReadOnly());

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var personas = result!.Validations.Single(v => v.Section == "Personas");
        personas.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Personas_InvalidWhenEmpty()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var personas = result!.Validations.Single(v => v.Section == "Personas");
        personas.IsValid.Should().BeFalse();
        personas.Warnings.Should().ContainSingle(w => w.Contains("persona"));
    }

    [Fact]
    public async Task Handle_UserStories_ValidWhenAtLeastOneCompleteStory()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Persona>().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                      .Returns(new[] { UserStory.Create(product.Id, "As a user", "I want X", "So that Y") }
                               .ToList().AsReadOnly());
        _featureRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Feature>().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                      .Returns(new List<TimelinePhase>().AsReadOnly());

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var stories = result!.Validations.Single(v => v.Section == "UserStories");
        stories.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserStories_InvalidWhenEmpty()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var stories = result!.Validations.Single(v => v.Section == "UserStories");
        stories.IsValid.Should().BeFalse();
        stories.Warnings.Should().ContainSingle(w => w.Contains("user story"));
    }

    [Fact]
    public async Task Handle_Features_ValidWhenAtLeastOneMvpFeature()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Persona>().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<UserStory>().AsReadOnly());
        // MVP feature (default phase)
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new[] { Feature.Create(product.Id, "Login", "User login") }
                             .ToList().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                      .Returns(new List<TimelinePhase>().AsReadOnly());

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var features = result!.Validations.Single(v => v.Section == "Features");
        features.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Features_InvalidWhenNoMvpFeature()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var features = result!.Validations.Single(v => v.Section == "Features");
        features.IsValid.Should().BeFalse();
        features.Warnings.Should().ContainSingle(w => w.Contains("MVP"));
    }

    [Fact]
    public async Task Handle_Timeline_ValidWhenAtLeastOnePhaseWithDuration()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Persona>().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<UserStory>().AsReadOnly());
        _featureRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Feature>().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                     .Returns(new[] { TimelinePhase.Create(product.Id, "MVP", 4) }
                              .ToList().AsReadOnly());

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var timeline = result!.Validations.Single(v => v.Section == "Timeline");
        timeline.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Timeline_InvalidWhenEmpty()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        SetupEmptyRepositories();

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var timeline = result!.Validations.Single(v => v.Section == "Timeline");
        timeline.IsValid.Should().BeFalse();
        timeline.Warnings.Should().ContainSingle(w => w.Contains("phase"));
    }

    [Fact]
    public async Task Handle_Timeline_InvalidWhenAllPhasesHaveZeroDuration()
    {
        var product = MakeProduct();
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Persona>().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(new List<UserStory>().AsReadOnly());
        _featureRepo.GetByProductIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                     .Returns(new List<Feature>().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                     .Returns(new[] { TimelinePhase.Create(product.Id, "MVP", 0) }
                              .ToList().AsReadOnly());

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        var timeline = result!.Validations.Single(v => v.Section == "Timeline");
        timeline.IsValid.Should().BeFalse();
        timeline.Warnings.Should().ContainSingle(w => w.Contains("duration"));
    }

    [Fact]
    public async Task Handle_FullyValidProduct_AllSectionsValid()
    {
        var product = MakeProduct(vision: "My vision", problemStatement: "My problem");
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new[] { Persona.Create(product.Id, "Alice", "Developer") }
                             .ToList().AsReadOnly());
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                      .Returns(new[] { UserStory.Create(product.Id, "As a user", "I want X", "So that Y") }
                               .ToList().AsReadOnly());
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                    .Returns(new[] { Feature.Create(product.Id, "Login", "Desc") }
                             .ToList().AsReadOnly());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>())
                     .Returns(new[] { TimelinePhase.Create(product.Id, "MVP", 4) }
                              .ToList().AsReadOnly());

        var result = await _handler.Handle(
            new ValidateSectionsQuery(product.Id, product.OwnerId.Value), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Validations.Should().HaveCount(5);
        result.Validations.Should().AllSatisfy(v =>
        {
            v.IsValid.Should().BeTrue();
            v.Warnings.Should().BeEmpty();
        });
    }
}
