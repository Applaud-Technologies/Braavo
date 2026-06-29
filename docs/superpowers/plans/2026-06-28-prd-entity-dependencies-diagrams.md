# PRD Builder Entity Dependencies & System Diagrams

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Enforce entity creation order (Personas → Features → User Stories) and add UML Component diagram generation.

**Architecture:** Add validation in create handlers to check for prerequisite entities. Add new diagram type enum value and LLM-based generation for UML Component diagrams. Update frontend to show dependency warnings and support the new diagram type.

**Tech Stack:** .NET 8.0, FastEndpoints, MediatR, EF Core, PostgreSQL, React 18, TypeScript 5, Redux Toolkit, Tailwind CSS

## Global Constraints

- All endpoints require JWT authentication
- Backend port: 5153, Frontend port: 5173
- Follow existing rustic theme (stone/amber colors)
- Use existing repository patterns (IPersonaRepository, IFeatureRepository)
- Use existing test patterns (NSubstitute, FluentAssertions)
- TDD: Write failing test first, then implementation

---

### Task 1: Add Feature Creation Dependency Check

**Files:**
- Modify: `src/Backend/Braavo.Core/UseCases/Features/CreateFeatureCommand.cs`
- Test: `src/Backend/tests/Braavo.UnitTests/UseCases/CreateFeatureHandlerTests.cs`

**Interfaces:**
- Consumes: `IPersonaRepository.GetByProductIdAsync(Guid productId, CancellationToken ct)`
- Produces: `CreateFeatureResult` with new error message "At least one persona is required before creating features"

- [ ] **Step 1: Write the failing test**

Create `src/Backend/tests/Braavo.UnitTests/UseCases/CreateFeatureHandlerTests.cs`:

```csharp
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Features;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class CreateFeatureHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateFeatureHandler _handler;

    public CreateFeatureHandlerTests()
    {
        _handler = new CreateFeatureHandler(_productRepo, _featureRepo, _personaRepo, _mediator);
    }

    [Fact]
    public async Task Handle_NoPersonasExist_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Persona>());

        var command = new CreateFeatureCommand(product.Id, userId, "Feature", "Description");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("At least one persona is required before creating features");
    }

    [Fact]
    public async Task Handle_PersonaExists_CreatesFeature()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var persona = Persona.Create(product.Id, "User", "Developer");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });

        var command = new CreateFeatureCommand(product.Id, userId, "Feature", "Description");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.FeatureId.Should().NotBeEmpty();
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~CreateFeatureHandlerTests"`
Expected: FAIL - constructor mismatch (missing IPersonaRepository parameter)

- [ ] **Step 3: Update CreateFeatureHandler to inject IPersonaRepository**

Modify `src/Backend/Braavo.Core/UseCases/Features/CreateFeatureCommand.cs`:

```csharp
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Features;

public record CreateFeatureCommand(
    Guid ProductId,
    Guid UserId,
    string Name,
    string Description,
    FeaturePhase Phase = FeaturePhase.Mvp,
    EffortSize? Effort = null,
    Guid? ParentId = null,
    int SortOrder = 0
) : IRequest<CreateFeatureResult>;

public record CreateFeatureResult(
    Guid FeatureId,
    bool Success,
    string? Error = null
);

public class CreateFeatureHandler : IRequestHandler<CreateFeatureCommand, CreateFeatureResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IMediator _mediator;

    public CreateFeatureHandler(
        IProductRepository productRepo,
        IFeatureRepository featureRepo,
        IPersonaRepository personaRepo,
        IMediator mediator)
    {
        _productRepo = productRepo;
        _featureRepo = featureRepo;
        _personaRepo = personaRepo;
        _mediator    = mediator;
    }

    public async Task<CreateFeatureResult> Handle(CreateFeatureCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new CreateFeatureResult(Guid.Empty, false, "Product not found");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);
        if (personas.Count == 0)
            return new CreateFeatureResult(Guid.Empty, false, "At least one persona is required before creating features");

        if (string.IsNullOrWhiteSpace(request.Name))
            return new CreateFeatureResult(Guid.Empty, false, "Feature name is required");

        if (string.IsNullOrWhiteSpace(request.Description))
            return new CreateFeatureResult(Guid.Empty, false, "Feature description is required");

        var feature = Feature.Create(request.ProductId, request.Name, request.Description);
        feature.ChangePhase(request.Phase);
        feature.Update(request.Name, request.Description, request.Effort);
        feature.UpdateSortOrder(request.SortOrder);

        if (request.ParentId.HasValue)
            feature.SetParent(request.ParentId);

        await _featureRepo.AddAsync(feature, ct);

        await _mediator.Send(new RecalculateCompletionCommand(request.ProductId), ct);

        return new CreateFeatureResult(feature.Id, true);
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~CreateFeatureHandlerTests"`
Expected: PASS (2 tests)

- [ ] **Step 5: Run all backend tests to ensure no regressions**

Run: `cd src/Backend && dotnet test`
Expected: All tests pass

- [ ] **Step 6: Commit**

```bash
git add src/Backend/Braavo.Core/UseCases/Features/CreateFeatureCommand.cs
git add src/Backend/tests/Braavo.UnitTests/UseCases/CreateFeatureHandlerTests.cs
git commit -m "feat: require persona before creating features"
```

---

### Task 2: Add User Story Creation Dependency Check

**Files:**
- Modify: `src/Backend/Braavo.Core/UseCases/UserStories/CreateUserStoryCommand.cs`
- Test: `src/Backend/tests/Braavo.UnitTests/UseCases/CreateUserStoryHandlerTests.cs`

**Interfaces:**
- Consumes: `IFeatureRepository.GetByProductIdAsync(Guid productId, CancellationToken ct)`
- Produces: `CreateUserStoryResult` with new error message "At least one feature is required before creating user stories"

- [ ] **Step 1: Write the failing test**

Create `src/Backend/tests/Braavo.UnitTests/UseCases/CreateUserStoryHandlerTests.cs`:

```csharp
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.UserStories;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class CreateUserStoryHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IUserStoryRepository _storyRepo = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateUserStoryHandler _handler;

    public CreateUserStoryHandlerTests()
    {
        _handler = new CreateUserStoryHandler(_productRepo, _storyRepo, _featureRepo, _mediator);
    }

    [Fact]
    public async Task Handle_NoFeaturesExist_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());

        var command = new CreateUserStoryCommand(product.Id, userId, "As a user", "I want to login", "So I can access my account");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("At least one feature is required before creating user stories");
    }

    [Fact]
    public async Task Handle_FeatureExists_CreatesStory()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var feature = Feature.Create(product.Id, "Login", "User authentication");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { feature });

        var command = new CreateUserStoryCommand(product.Id, userId, "As a user", "I want to login", "So I can access my account");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.StoryId.Should().NotBeEmpty();
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~CreateUserStoryHandlerTests"`
Expected: FAIL - constructor mismatch (missing IFeatureRepository parameter)

- [ ] **Step 3: Update CreateUserStoryHandler to inject IFeatureRepository**

Modify `src/Backend/Braavo.Core/UseCases/UserStories/CreateUserStoryCommand.cs`:

```csharp
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
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~CreateUserStoryHandlerTests"`
Expected: PASS (2 tests)

- [ ] **Step 5: Run all backend tests to ensure no regressions**

Run: `cd src/Backend && dotnet test`
Expected: All tests pass

- [ ] **Step 6: Commit**

```bash
git add src/Backend/Braavo.Core/UseCases/UserStories/CreateUserStoryCommand.cs
git add src/Backend/tests/Braavo.UnitTests/UseCases/CreateUserStoryHandlerTests.cs
git commit -m "feat: require feature before creating user stories"
```

---

### Task 3: Add Dependency Check API Endpoint

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/Products/GetDependencyStatusQuery.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Products/GetDependencyStatusEndpoint.cs`
- Test: `src/Backend/tests/Braavo.UnitTests/UseCases/GetDependencyStatusHandlerTests.cs`

**Interfaces:**
- Consumes: `IPersonaRepository`, `IFeatureRepository`, `IUserStoryRepository`
- Produces: `DependencyStatusResult` with `HasPersonas`, `HasFeatures`, `HasStories` booleans and `CanCreateFeatures`, `CanCreateStories` booleans

- [ ] **Step 1: Write the failing test**

Create `src/Backend/tests/Braavo.UnitTests/UseCases/GetDependencyStatusHandlerTests.cs`:

```csharp
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GetDependencyStatusHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly IUserStoryRepository _storyRepo = Substitute.For<IUserStoryRepository>();
    private readonly GetDependencyStatusHandler _handler;

    public GetDependencyStatusHandlerTests()
    {
        _handler = new GetDependencyStatusHandler(_productRepo, _personaRepo, _featureRepo, _storyRepo);
    }

    [Fact]
    public async Task Handle_EmptyProduct_ReturnsCorrectStatus()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Persona>());
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());

        var result = await _handler.Handle(new GetDependencyStatusQuery(product.Id, userId), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.HasPersonas.Should().BeFalse();
        result.HasFeatures.Should().BeFalse();
        result.HasStories.Should().BeFalse();
        result.CanCreateFeatures.Should().BeFalse();
        result.CanCreateStories.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_HasPersonas_CanCreateFeatures()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var persona = Persona.Create(product.Id, "User", "Dev");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<Feature>());
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());

        var result = await _handler.Handle(new GetDependencyStatusQuery(product.Id, userId), CancellationToken.None);

        result.HasPersonas.Should().BeTrue();
        result.CanCreateFeatures.Should().BeTrue();
        result.CanCreateStories.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_HasPersonasAndFeatures_CanCreateStories()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        var persona = Persona.Create(product.Id, "User", "Dev");
        var feature = Feature.Create(product.Id, "Login", "Auth");

        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { persona });
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(new[] { feature });
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(Array.Empty<UserStory>());

        var result = await _handler.Handle(new GetDependencyStatusQuery(product.Id, userId), CancellationToken.None);

        result.HasPersonas.Should().BeTrue();
        result.HasFeatures.Should().BeTrue();
        result.CanCreateFeatures.Should().BeTrue();
        result.CanCreateStories.Should().BeTrue();
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~GetDependencyStatusHandlerTests"`
Expected: FAIL - GetDependencyStatusQuery not found

- [ ] **Step 3: Create the query and handler**

Create `src/Backend/Braavo.Core/UseCases/Products/GetDependencyStatusQuery.cs`:

```csharp
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public record GetDependencyStatusQuery(Guid ProductId, Guid UserId) : IRequest<DependencyStatusResult>;

public record DependencyStatusResult(
    bool Success,
    bool HasPersonas = false,
    bool HasFeatures = false,
    bool HasStories = false,
    bool CanCreateFeatures = false,
    bool CanCreateStories = false,
    string? Error = null
);

public class GetDependencyStatusHandler : IRequestHandler<GetDependencyStatusQuery, DependencyStatusResult>
{
    private readonly IProductRepository _productRepo;
    private readonly IPersonaRepository _personaRepo;
    private readonly IFeatureRepository _featureRepo;
    private readonly IUserStoryRepository _storyRepo;

    public GetDependencyStatusHandler(
        IProductRepository productRepo,
        IPersonaRepository personaRepo,
        IFeatureRepository featureRepo,
        IUserStoryRepository storyRepo)
    {
        _productRepo = productRepo;
        _personaRepo = personaRepo;
        _featureRepo = featureRepo;
        _storyRepo = storyRepo;
    }

    public async Task<DependencyStatusResult> Handle(GetDependencyStatusQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new DependencyStatusResult(false, Error: "Product not found");

        var personas = await _personaRepo.GetByProductIdAsync(request.ProductId, ct);
        var features = await _featureRepo.GetByProductIdAsync(request.ProductId, ct);
        var stories = await _storyRepo.GetByProductIdAsync(request.ProductId, ct);

        var hasPersonas = personas.Count > 0;
        var hasFeatures = features.Count > 0;
        var hasStories = stories.Count > 0;

        return new DependencyStatusResult(
            Success: true,
            HasPersonas: hasPersonas,
            HasFeatures: hasFeatures,
            HasStories: hasStories,
            CanCreateFeatures: hasPersonas,
            CanCreateStories: hasFeatures
        );
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~GetDependencyStatusHandlerTests"`
Expected: PASS (3 tests)

- [ ] **Step 5: Create the API endpoint**

Create `src/Backend/Braavo.Api/Endpoints/Products/GetDependencyStatusEndpoint.cs`:

```csharp
using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public class GetDependencyStatusEndpoint : EndpointWithoutRequest<DependencyStatusResponse>
{
    private readonly IMediator _mediator;

    public GetDependencyStatusEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/products/{id}/dependencies");
        Roles("User");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new GetDependencyStatusQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new DependencyStatusResponse(
            result.HasPersonas,
            result.HasFeatures,
            result.HasStories,
            result.CanCreateFeatures,
            result.CanCreateStories
        ), ct);
    }
}

public record DependencyStatusResponse(
    bool HasPersonas,
    bool HasFeatures,
    bool HasStories,
    bool CanCreateFeatures,
    bool CanCreateStories
);
```

- [ ] **Step 6: Run all backend tests**

Run: `cd src/Backend && dotnet test`
Expected: All tests pass

- [ ] **Step 7: Commit**

```bash
git add src/Backend/Braavo.Core/UseCases/Products/GetDependencyStatusQuery.cs
git add src/Backend/Braavo.Api/Endpoints/Products/GetDependencyStatusEndpoint.cs
git add src/Backend/tests/Braavo.UnitTests/UseCases/GetDependencyStatusHandlerTests.cs
git commit -m "feat: add dependency status API endpoint"
```

---

### Task 4: Add UML Component Diagram Type

**Files:**
- Modify: `src/Backend/Braavo.Core/UseCases/Diagrams/GenerateDiagramCommand.cs`
- Modify: `src/Backend/Braavo.Core/UseCases/Diagrams/GenerateDiagramHandler.cs`
- Test: `src/Backend/tests/Braavo.UnitTests/UseCases/GenerateDiagramHandlerTests.cs`

**Interfaces:**
- Consumes: Existing `DiagramType` enum, `ILlmProvider`, `IDocumentRepository`
- Produces: `DiagramType.Component` enum value, Mermaid flowchart output representing component architecture

- [ ] **Step 1: Add Component to DiagramType enum**

Modify `src/Backend/Braavo.Core/UseCases/Diagrams/GenerateDiagramCommand.cs`:

```csharp
using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public enum DiagramType
{
    Flowchart,
    Sequence,
    ClassDiagram,
    EntityRelationship,
    UserJourney,
    Component
}

public record GenerateDiagramCommand(
    Guid DocumentId,
    DiagramType Type,
    Guid UserId,
    string? Focus = null
) : IRequest<DiagramResponse>;

public record DiagramResponse(
    string MermaidCode,
    DiagramType Type,
    bool Success,
    string? Error = null
);
```

- [ ] **Step 2: Add Component case to handler**

Modify `src/Backend/Braavo.Core/UseCases/Diagrams/GenerateDiagramHandler.cs` `GetSystemPrompt` method to add the Component case:

```csharp
private static string GetSystemPrompt(DiagramType type) => type switch
{
    DiagramType.Flowchart => """
        Generate a Mermaid.js flowchart diagram based on the PRD content.
        Use 'flowchart TD' for top-down layout.
        Include main user flows and decision points.
        Return ONLY the Mermaid code, no explanation.
        """,
    DiagramType.Sequence => """
        Generate a Mermaid.js sequence diagram based on the PRD content.
        Show interactions between users and system components.
        Return ONLY the Mermaid code, no explanation.
        """,
    DiagramType.ClassDiagram => """
        Generate a Mermaid.js class diagram based on the PRD content.
        Identify main entities and their relationships.
        Return ONLY the Mermaid code, no explanation.
        """,
    DiagramType.EntityRelationship => """
        Generate a Mermaid.js ER diagram based on the PRD content.
        Show database entities and their relationships.
        Return ONLY the Mermaid code, no explanation.
        """,
    DiagramType.UserJourney => """
        Generate a Mermaid.js user journey diagram based on the PRD content.
        Map out the main user experience flow.
        Return ONLY the Mermaid code, no explanation.
        """,
    DiagramType.Component => """
        Generate a Mermaid.js flowchart diagram representing a UML-style component architecture.
        Use 'flowchart TB' for top-to-bottom layout.
        Identify system components (frontend, backend, services, databases, external APIs).
        Show dependencies between components with labeled arrows.
        Group related components in subgraphs.
        Use descriptive node names like 'WebApp[Web Application]', 'API[REST API]', 'DB[(Database)]'.
        Return ONLY the Mermaid code, no explanation.
        """,
    _ => "Generate a Mermaid.js diagram. Return ONLY the Mermaid code."
};
```

- [ ] **Step 3: Add test for Component diagram type**

Add to `src/Backend/tests/Braavo.UnitTests/UseCases/GenerateDiagramHandlerTests.cs`:

```csharp
[Fact]
public async Task Handle_ComponentDiagram_ReturnsFlowchartWithComponents()
{
    var userId = Guid.NewGuid();
    var document = Document.Create(
        "Test PRD",
        "# Overview\nA web application with React frontend, .NET API, and PostgreSQL database.",
        DocumentType.Prd,
        UserId.From(userId)
    );

    _documents.GetByIdAsync(document.Id, Arg.Any<CancellationToken>()).Returns(document);
    _llm.GenerateAsync(Arg.Any<LlmRequest>(), Arg.Any<CancellationToken>())
        .Returns(new LlmResponse(true, """
            ```mermaid
            flowchart TB
                subgraph Frontend
                    WebApp[React Web App]
                end
                subgraph Backend
                    API[.NET API]
                end
                subgraph Data
                    DB[(PostgreSQL)]
                end
                WebApp --> API
                API --> DB
            ```
            """));

    var result = await _handler.Handle(
        new GenerateDiagramCommand(document.Id, DiagramType.Component, userId),
        CancellationToken.None
    );

    result.Success.Should().BeTrue();
    result.Type.Should().Be(DiagramType.Component);
    result.MermaidCode.Should().Contain("flowchart");
}
```

- [ ] **Step 4: Run tests**

Run: `cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FullyQualifiedName~GenerateDiagramHandlerTests"`
Expected: All tests pass

- [ ] **Step 5: Commit**

```bash
git add src/Backend/Braavo.Core/UseCases/Diagrams/GenerateDiagramCommand.cs
git add src/Backend/Braavo.Core/UseCases/Diagrams/GenerateDiagramHandler.cs
git add src/Backend/tests/Braavo.UnitTests/UseCases/GenerateDiagramHandlerTests.cs
git commit -m "feat: add UML Component diagram type"
```

---

### Task 5: Frontend Dependency Status API Client

**Files:**
- Create: `src/Frontend/src/api/dependencies.ts`
- Create: `src/Frontend/src/store/slices/dependenciesSlice.ts`
- Modify: `src/Frontend/src/store/store.ts`

**Interfaces:**
- Consumes: `GET /api/products/{id}/dependencies` endpoint
- Produces: `dependenciesApi.get(productId)`, Redux slice with `fetchDependencies` thunk

- [ ] **Step 1: Create API client**

Create `src/Frontend/src/api/dependencies.ts`:

```typescript
import { api } from './client';

export interface DependencyStatus {
  hasPersonas: boolean;
  hasFeatures: boolean;
  hasStories: boolean;
  canCreateFeatures: boolean;
  canCreateStories: boolean;
}

export const dependenciesApi = {
  get: (productId: string) =>
    api.get<DependencyStatus>(`/products/${productId}/dependencies`),
};
```

- [ ] **Step 2: Create Redux slice**

Create `src/Frontend/src/store/slices/dependenciesSlice.ts`:

```typescript
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { dependenciesApi, type DependencyStatus } from '../../api/dependencies';

interface DependenciesState {
  status: DependencyStatus | null;
  loading: boolean;
  error: string | null;
}

const initialState: DependenciesState = {
  status: null,
  loading: false,
  error: null,
};

export const fetchDependencies = createAsyncThunk(
  'dependencies/fetch',
  async (productId: string) => {
    const response = await dependenciesApi.get(productId);
    return response.data;
  }
);

const dependenciesSlice = createSlice({
  name: 'dependencies',
  initialState,
  reducers: {
    clearDependencies: (state) => {
      state.status = null;
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchDependencies.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchDependencies.fulfilled, (state, action) => {
        state.loading = false;
        state.status = action.payload;
      })
      .addCase(fetchDependencies.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch dependencies';
      });
  },
});

export const { clearDependencies } = dependenciesSlice.actions;
export default dependenciesSlice.reducer;
```

- [ ] **Step 3: Register slice in store**

Modify `src/Frontend/src/store/store.ts` to add the dependencies reducer:

Add import:
```typescript
import dependenciesReducer from './slices/dependenciesSlice';
```

Add to reducer object:
```typescript
dependencies: dependenciesReducer,
```

- [ ] **Step 4: Verify frontend builds**

Run: `cd src/Frontend && npm run build`
Expected: Build succeeds

- [ ] **Step 5: Commit**

```bash
git add src/Frontend/src/api/dependencies.ts
git add src/Frontend/src/store/slices/dependenciesSlice.ts
git add src/Frontend/src/store/store.ts
git commit -m "feat: add frontend dependency status API and slice"
```

---

### Task 6: Add Dependency Warnings to FeaturesPage

**Files:**
- Modify: `src/Frontend/src/pages/FeaturesPage.tsx`

**Interfaces:**
- Consumes: `fetchDependencies` thunk, `RootState.dependencies`
- Produces: Warning banner when `!canCreateFeatures`, disabled "Add Feature" button

- [ ] **Step 1: Update FeaturesPage to show dependency warning**

Modify `src/Frontend/src/pages/FeaturesPage.tsx`:

Add imports at top:
```typescript
import { fetchDependencies } from '../store/slices/dependenciesSlice';
```

Add selector inside component:
```typescript
const { status: depStatus } = useAppSelector((state: RootState) => state.dependencies);
```

Add to useEffect:
```typescript
useEffect(() => {
  if (id) {
    dispatch(fetchProduct(id));
    dispatch(fetchFeatures(id));
    dispatch(fetchDependencies(id));
  }
}, [dispatch, id]);
```

Add warning banner in JSX (before the FeatureBoard):
```typescript
{depStatus && !depStatus.canCreateFeatures && (
  <div className="mb-6 p-4 bg-amber-50 border border-amber-200 rounded-xl">
    <div className="flex items-start gap-3">
      <svg className="w-5 h-5 text-amber-600 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
      </svg>
      <div>
        <p className="font-medium text-amber-800">Personas Required</p>
        <p className="text-sm text-amber-700 mt-1">
          Create at least one persona before adding features.{' '}
          <Link to={`/products/${id}/personas`} className="underline hover:text-amber-900">
            Go to Personas
          </Link>
        </p>
      </div>
    </div>
  </div>
)}
```

Disable Add Feature buttons when `!depStatus?.canCreateFeatures`:
```typescript
const canAddFeature = depStatus?.canCreateFeatures ?? false;
```

Pass disabled state to FeatureBoard or add onClick guard:
```typescript
const handleOpenCreate = (phase: FeaturePhase = 'Mvp') => {
  if (!canAddFeature) return;
  setEditingFeature(undefined);
  setDefaultPhase(phase);
  setEditorOpen(true);
};
```

- [ ] **Step 2: Verify frontend builds**

Run: `cd src/Frontend && npm run build`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add src/Frontend/src/pages/FeaturesPage.tsx
git commit -m "feat: show dependency warning on features page"
```

---

### Task 7: Add Dependency Warnings to StoriesPage

**Files:**
- Modify: `src/Frontend/src/pages/StoriesPage.tsx`

**Interfaces:**
- Consumes: `fetchDependencies` thunk, `RootState.dependencies`
- Produces: Warning banner when `!canCreateStories`, disabled "Add Story" button

- [ ] **Step 1: Update StoriesPage to show dependency warning**

Modify `src/Frontend/src/pages/StoriesPage.tsx`:

Add imports at top:
```typescript
import { fetchDependencies } from '../store/slices/dependenciesSlice';
```

Add selector inside component:
```typescript
const { status: depStatus } = useAppSelector((state: RootState) => state.dependencies);
```

Add to useEffect:
```typescript
useEffect(() => {
  if (id) {
    dispatch(fetchProduct(id));
    dispatch(fetchUserStories(id));
    dispatch(fetchPersonas(id));
    dispatch(fetchDependencies(id));
  }
}, [dispatch, id]);
```

Add warning banner in JSX (after the header, before the story list):
```typescript
{depStatus && !depStatus.canCreateStories && (
  <div className="mb-6 p-4 bg-amber-50 border border-amber-200 rounded-xl">
    <div className="flex items-start gap-3">
      <svg className="w-5 h-5 text-amber-600 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
      </svg>
      <div>
        <p className="font-medium text-amber-800">Features Required</p>
        <p className="text-sm text-amber-700 mt-1">
          Create at least one feature before adding user stories.{' '}
          <Link to={`/products/${id}/features`} className="underline hover:text-amber-900">
            Go to Features
          </Link>
        </p>
      </div>
    </div>
  </div>
)}
```

Guard add story handler:
```typescript
const canAddStory = depStatus?.canCreateStories ?? false;

const handleAddStory = () => {
  if (!canAddStory) return;
  // existing logic
};
```

- [ ] **Step 2: Verify frontend builds**

Run: `cd src/Frontend && npm run build`
Expected: Build succeeds

- [ ] **Step 3: Commit**

```bash
git add src/Frontend/src/pages/StoriesPage.tsx
git commit -m "feat: show dependency warning on stories page"
```

---

### Task 8: Add Component Diagram to DiagramsPage

**Files:**
- Modify: `src/Frontend/src/api/diagrams.ts`
- Modify: `src/Frontend/src/store/slices/diagramsSlice.ts`
- Modify: `src/Frontend/src/pages/DiagramsPage.tsx`

**Interfaces:**
- Consumes: `POST /api/diagrams/generate` with `type: "Component"`
- Produces: New "Component Architecture" section in DiagramsPage

- [ ] **Step 1: Add Component type to API**

Modify `src/Frontend/src/api/diagrams.ts` to add Component type if not already present.

Check for existing DiagramType and add 'Component' to the union type:
```typescript
export type DiagramType = 'Flowchart' | 'Sequence' | 'ClassDiagram' | 'EntityRelationship' | 'UserJourney' | 'Component';
```

- [ ] **Step 2: Add generateComponentDiagram thunk to diagramsSlice**

Modify `src/Frontend/src/store/slices/diagramsSlice.ts`:

Add state properties:
```typescript
componentCode: string | null;
componentLoading: boolean;
componentError: string | null;
```

Add thunk:
```typescript
export const generateComponentDiagram = createAsyncThunk(
  'diagrams/generateComponent',
  async ({ documentId }: { documentId: string }) => {
    const response = await diagramsApi.generate(documentId, 'Component');
    return response.data;
  }
);
```

Add reducers for componentCode state and extra reducers for the thunk.

- [ ] **Step 3: Add Component section to DiagramsPage**

Modify `src/Frontend/src/pages/DiagramsPage.tsx`:

Add to selector:
```typescript
const {
  // ... existing
  componentCode,
  componentLoading,
  componentError,
} = useAppSelector((state: RootState) => state.diagrams);
```

Add generate handler:
```typescript
const handleGenerateComponent = () => {
  if (currentProduct?.prdDocumentId) {
    dispatch(generateComponentDiagram({ documentId: currentProduct.prdDocumentId }));
  }
};
```

Add JSX section for Component Architecture diagram (similar pattern to existing sections):
```typescript
<div className="bg-white rounded-2xl shadow-sm border border-stone-200 p-6">
  <div className="flex items-center justify-between mb-4">
    <h2 className="text-lg font-semibold text-stone-900">Component Architecture</h2>
    <button
      onClick={handleGenerateComponent}
      disabled={componentLoading || !currentProduct?.prdDocumentId}
      className="px-4 py-2 bg-amber-600 text-white rounded-lg hover:bg-amber-700 disabled:opacity-50 disabled:cursor-not-allowed text-sm font-medium"
    >
      {componentLoading ? 'Generating...' : 'Generate'}
    </button>
  </div>
  <DiagramSection
    code={componentCode}
    loading={componentLoading}
    error={componentError}
    diagramId="component-diagram"
    filename="component-architecture"
    placeholder="Click Generate to create a component architecture diagram"
  />
</div>
```

- [ ] **Step 4: Verify frontend builds**

Run: `cd src/Frontend && npm run build`
Expected: Build succeeds

- [ ] **Step 5: Commit**

```bash
git add src/Frontend/src/api/diagrams.ts
git add src/Frontend/src/store/slices/diagramsSlice.ts
git add src/Frontend/src/pages/DiagramsPage.tsx
git commit -m "feat: add component architecture diagram to frontend"
```

---

### Task 9: Integration Testing

- [ ] **Step 1: Start backend**

Run: `cd src/Backend && dotnet run --project Braavo.Api`
Expected: Server starts on port 5153

- [ ] **Step 2: Start frontend**

Run: `cd src/Frontend && npm run dev`
Expected: Dev server starts on port 5173

- [ ] **Step 3: Test persona dependency enforcement**

1. Create a new product
2. Navigate to Features page
3. Verify warning banner appears: "Personas Required"
4. Verify Add Feature buttons are disabled
5. Navigate to Personas page
6. Create a persona
7. Navigate back to Features page
8. Verify warning banner is gone
9. Verify Add Feature works

- [ ] **Step 4: Test feature dependency enforcement**

1. Navigate to User Stories page
2. Verify warning banner appears: "Features Required"
3. Verify Add Story button is disabled
4. Navigate to Features page
5. Create a feature
6. Navigate back to User Stories page
7. Verify warning banner is gone
8. Verify Add Story works

- [ ] **Step 5: Test Component diagram generation**

1. Navigate to Diagrams page
2. Find Component Architecture section
3. Click Generate
4. Verify diagram renders

- [ ] **Step 6: Run all tests**

Run: `cd src/Backend && dotnet test`
Run: `cd src/Frontend && npm run build`

- [ ] **Step 7: Final commit**

```bash
git add -A
git commit -m "test: verify entity dependencies and component diagrams"
```

---

## Summary

This plan delivers:
1. **Entity dependencies** - Features require Personas, User Stories require Features
2. **Dependency status API** - GET /api/products/{id}/dependencies endpoint
3. **UML Component diagrams** - New diagram type using LLM-generated Mermaid
4. **Frontend warnings** - Dependency warning banners on Features and Stories pages
5. **Feature mind map** - Already exists via GenerateFeatureHierarchyCommand
