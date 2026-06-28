using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Export;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class ExportMarkdownHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IUserStoryRepository _userStoryRepo = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly ITimelineRepository _timelineRepo = Substitute.For<ITimelineRepository>();
    private readonly ExportMarkdownHandler _handler;

    public ExportMarkdownHandlerTests()
    {
        _handler = new ExportMarkdownHandler(
            _productRepo,
            _personaRepo,
            _userStoryRepo,
            _featureRepo,
            _timelineRepo);
    }

    [Fact]
    public async Task Handle_WithValidProduct_ReturnsMarkdownContent()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("My App", "A great app", UserId.From(userId));
        product.UpdateOverview("Empower users", "Hard problem", "Best value");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Persona>());
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<TimelinePhase>());

        var result = await _handler.Handle(new ExportMarkdownQuery(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Content.Should().NotBeNullOrEmpty();
        result.FileName.Should().EndWith("-prd.md");

        var markdown = System.Text.Encoding.UTF8.GetString(result.Content!);
        markdown.Should().Contain("# My App - Product Requirements Document");
        markdown.Should().Contain("Empower users");
        markdown.Should().Contain("Hard problem");
    }

    [Fact]
    public async Task Handle_WithWrongOwner_ReturnsForbidden()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var product = Product.Create("My App", "Desc", UserId.From(ownerId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _handler.Handle(new ExportMarkdownQuery(product.Id, otherUserId), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Forbidden");
    }

    [Fact]
    public async Task Handle_WithMissingProduct_ReturnsNotFound()
    {
        _productRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _handler.Handle(new ExportMarkdownQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Product not found");
    }

    [Fact]
    public async Task Handle_WithPersonasAndFeatures_IncludesThemInMarkdown()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("CRM Tool", "For sales teams", UserId.From(userId));

        var persona = Persona.Create(product.Id, "Sales Rep", "Field Sales",
            goals: ["Close deals faster", "Track pipeline"]);

        var story = UserStory.Create(product.Id, "a sales rep", "see my pipeline", "I can track progress");

        var feature = Feature.Create(product.Id, "Pipeline Dashboard", "Visual pipeline tracker");

        var phase = TimelinePhase.Create(product.Id, "MVP", 8, sortOrder: 0);

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { story });
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { feature });
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { phase });

        var result = await _handler.Handle(new ExportMarkdownQuery(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeTrue();
        var markdown = System.Text.Encoding.UTF8.GetString(result.Content!);

        markdown.Should().Contain("## User Personas");
        markdown.Should().Contain("### Sales Rep");
        markdown.Should().Contain("**Role:** Field Sales");
        markdown.Should().Contain("Close deals faster");

        markdown.Should().Contain("## User Stories");
        markdown.Should().Contain("As a** a sales rep");

        markdown.Should().Contain("## Features");
        markdown.Should().Contain("Pipeline Dashboard");

        markdown.Should().Contain("## Timeline");
        markdown.Should().Contain("### MVP");
        markdown.Should().Contain("8 weeks");
    }

    [Fact]
    public async Task Handle_FileNameIsSanitized()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("My Cool App: v2.0", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Persona>());
        _userStoryRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());
        _timelineRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<TimelinePhase>());

        var result = await _handler.Handle(new ExportMarkdownQuery(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.FileName.Should().NotContain(" ");
        result.FileName.Should().EndWith("-prd.md");
    }
}
