# Guided PRD Builder Phase 1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the foundation for a guided PRD builder with new Product entity, structured sections (Personas, User Stories, Features), version history, and basic CRUD APIs.

**Architecture:** Clean Architecture with Product as the aggregate root. Each PRD section (Personas, Stories, Features) stored in separate tables with foreign keys to Product. Version history tracked via ProductVersion table capturing snapshots. Frontend adds new routes and pages for product management.

**Tech Stack:** .NET 8.0, FastEndpoints, MediatR, EF Core 8.0, PostgreSQL, React 18, TypeScript 5, Redux Toolkit

## Global Constraints

- .NET 8.0 (net8.0 TFM) for all backend projects
- FastEndpoints for HTTP endpoints (not controllers)
- MediatR for CQRS command/query handling
- All entities use private setters with factory methods
- All endpoints require authentication except where noted
- Frontend uses TypeScript strict mode
- TDD: write failing test first, then implement

---

## File Structure

### Backend - New Files

```
src/Backend/Braavo.Core/
├── Entities/
│   ├── Product.cs                    # Main aggregate root
│   ├── Persona.cs                    # User persona entity
│   ├── UserStory.cs                  # User story entity
│   ├── Feature.cs                    # Feature entity
│   └── ProductVersion.cs             # Version history entity
├── Interfaces/
│   ├── IProductRepository.cs         # Product data access
│   ├── IPersonaRepository.cs         # Persona data access
│   ├── IUserStoryRepository.cs       # User story data access
│   └── IFeatureRepository.cs         # Feature data access
└── UseCases/
    └── Products/
        ├── CreateProductCommand.cs   # Create new product
        ├── CreateProductHandler.cs
        ├── GetProductQuery.cs        # Get product with sections
        ├── GetProductHandler.cs
        ├── ListProductsQuery.cs      # List user's products
        └── ListProductsHandler.cs

src/Backend/Braavo.Infrastructure/
├── Data/
│   └── Configurations/
│       ├── ProductConfiguration.cs
│       ├── PersonaConfiguration.cs
│       ├── UserStoryConfiguration.cs
│       ├── FeatureConfiguration.cs
│       └── ProductVersionConfiguration.cs
└── Repositories/
    ├── ProductRepository.cs
    ├── PersonaRepository.cs
    ├── UserStoryRepository.cs
    └── FeatureRepository.cs

src/Backend/Braavo.Api/Endpoints/
└── Products/
    ├── CreateProductEndpoint.cs
    ├── GetProductEndpoint.cs
    ├── ListProductsEndpoint.cs
    ├── UpdateProductEndpoint.cs
    └── DeleteProductEndpoint.cs

tests/Braavo.UnitTests/
├── Entities/
│   └── ProductTests.cs
└── UseCases/
    ├── CreateProductHandlerTests.cs
    └── GetProductHandlerTests.cs
```

### Frontend - New Files

```
src/Frontend/src/
├── pages/
│   ├── WelcomePage.tsx               # New welcome/dashboard
│   ├── ProductListPage.tsx           # List of user's products
│   ├── CreateProductPage.tsx         # New product wizard
│   └── ProductBuilderPage.tsx        # Main PRD builder shell
├── components/
│   └── products/
│       ├── ProductCard.tsx           # Product list item
│       ├── ProductWizard.tsx         # Creation wizard steps
│       └── BuilderSidebar.tsx        # Section navigation
└── store/
    └── slices/
        └── productsSlice.ts          # Products state management
```

---

### Task 1: Product Entity and Repository Interface

**Files:**
- Create: `src/Backend/Braavo.Core/Entities/Product.cs`
- Create: `src/Backend/Braavo.Core/Interfaces/IProductRepository.cs`
- Test: `tests/Braavo.UnitTests/Entities/ProductTests.cs`

**Interfaces:**
- Consumes: `UserId` value object from `Braavo.Core.ValueObjects`
- Produces: `Product` entity with `Create()`, `UpdateOverview()`, `CreateVersion()` methods; `IProductRepository` interface

- [ ] **Step 1: Write the failing test**

```csharp
// tests/Braavo.UnitTests/Entities/ProductTests.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using FluentAssertions;

namespace Braavo.UnitTests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var userId = UserId.New();
        var name = "TaskFlow";
        var description = "A task management app";

        var product = Product.Create(name, description, userId);

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.OwnerId.Should().Be(userId);
        product.Status.Should().Be(ProductStatus.Draft);
        product.Version.Should().Be(1);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateOverview_UpdatesFieldsAndIncrementsVersion()
    {
        var product = Product.Create("Test", "Desc", UserId.New());
        var originalVersion = product.Version;

        product.UpdateOverview("New Vision", "New Problem", "New Value Prop");

        product.Vision.Should().Be("New Vision");
        product.ProblemStatement.Should().Be("New Problem");
        product.ValueProposition.Should().Be("New Value Prop");
        product.Version.Should().Be(originalVersion + 1);
    }

    [Fact]
    public void CreateVersion_CapturesCurrentState()
    {
        var product = Product.Create("Test", "Desc", UserId.New());
        product.UpdateOverview("Vision", "Problem", "Value");

        var version = product.CreateVersion("Initial draft");

        version.ProductId.Should().Be(product.Id);
        version.VersionNumber.Should().Be(product.Version);
        version.Snapshot.Should().Contain("Vision");
        version.Comment.Should().Be("Initial draft");
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "ProductTests" -v n
```
Expected: FAIL with "Product not found" or similar

- [ ] **Step 3: Create Product entity**

```csharp
// src/Backend/Braavo.Core/Entities/Product.cs
using System.Text.Json;
using Braavo.Core.ValueObjects;

namespace Braavo.Core.Entities;

public enum ProductStatus
{
    Draft,
    InProgress,
    Review,
    Final
}

public class Product
{
    public Guid Id { get; private set; }
    public UserId OwnerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string[] Categories { get; private set; } = [];
    public ProductStatus Status { get; private set; }
    public int Version { get; private set; }
    public int CompletionPercentage { get; private set; }
    
    // Overview section
    public string Vision { get; private set; } = string.Empty;
    public string ProblemStatement { get; private set; } = string.Empty;
    public string ValueProposition { get; private set; } = string.Empty;
    public string[] TargetMarket { get; private set; } = [];
    public string[] BusinessGoals { get; private set; } = [];
    
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product() { }

    public static Product Create(string name, string description, UserId ownerId)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name,
            Description = description,
            Status = ProductStatus.Draft,
            Version = 1,
            CompletionPercentage = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateOverview(string vision, string problemStatement, string valueProposition)
    {
        Vision = vision;
        ProblemStatement = problemStatement;
        ValueProposition = valueProposition;
        IncrementVersion();
    }

    public void UpdateTargetMarket(string[] targetMarket)
    {
        TargetMarket = targetMarket;
        IncrementVersion();
    }

    public void UpdateBusinessGoals(string[] businessGoals)
    {
        BusinessGoals = businessGoals;
        IncrementVersion();
    }

    public void UpdateStatus(ProductStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCompletionPercentage(int percentage)
    {
        CompletionPercentage = Math.Clamp(percentage, 0, 100);
        UpdatedAt = DateTime.UtcNow;
    }

    public ProductVersion CreateVersion(string comment)
    {
        var snapshot = JsonSerializer.Serialize(new
        {
            Name,
            Description,
            Vision,
            ProblemStatement,
            ValueProposition,
            TargetMarket,
            BusinessGoals,
            Status = Status.ToString()
        });

        return ProductVersion.Create(Id, Version, snapshot, comment, OwnerId);
    }

    private void IncrementVersion()
    {
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

- [ ] **Step 4: Create ProductVersion entity**

```csharp
// src/Backend/Braavo.Core/Entities/ProductVersion.cs
using Braavo.Core.ValueObjects;

namespace Braavo.Core.Entities;

public class ProductVersion
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int VersionNumber { get; private set; }
    public string Snapshot { get; private set; } = string.Empty;
    public string Comment { get; private set; } = string.Empty;
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ProductVersion() { }

    public static ProductVersion Create(Guid productId, int versionNumber, string snapshot, string comment, UserId createdBy)
    {
        return new ProductVersion
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            VersionNumber = versionNumber,
            Snapshot = snapshot,
            Comment = comment,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

- [ ] **Step 5: Create IProductRepository interface**

```csharp
// src/Backend/Braavo.Core/Interfaces/IProductRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;

namespace Braavo.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByOwnerIdAsync(UserId ownerId, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task AddVersionAsync(ProductVersion version, CancellationToken ct = default);
    Task<IReadOnlyList<ProductVersion>> GetVersionsAsync(Guid productId, CancellationToken ct = default);
}
```

- [ ] **Step 6: Run tests to verify they pass**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "ProductTests" -v n
```
Expected: 3 tests PASS

- [ ] **Step 7: Commit**

```bash
git add -A && git commit -m "feat: add Product entity with version history support"
```

---

### Task 2: Persona Entity and Repository Interface

**Files:**
- Create: `src/Backend/Braavo.Core/Entities/Persona.cs`
- Create: `src/Backend/Braavo.Core/Interfaces/IPersonaRepository.cs`
- Test: `tests/Braavo.UnitTests/Entities/PersonaTests.cs`

**Interfaces:**
- Consumes: `Product.Id` (Guid)
- Produces: `Persona` entity with `Create()`, `Update()` methods; `IPersonaRepository` interface

- [ ] **Step 1: Write the failing test**

```csharp
// tests/Braavo.UnitTests/Entities/PersonaTests.cs
using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Entities;

public class PersonaTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var productId = Guid.NewGuid();
        var name = "Sarah";
        var role = "Team Lead";

        var persona = Persona.Create(productId, name, role);

        persona.Id.Should().NotBeEmpty();
        persona.ProductId.Should().Be(productId);
        persona.Name.Should().Be(name);
        persona.Role.Should().Be(role);
        persona.TechnicalLevel.Should().Be(TechnicalLevel.Medium);
        persona.Goals.Should().BeEmpty();
        persona.PainPoints.Should().BeEmpty();
    }

    [Fact]
    public void Update_UpdatesAllFields()
    {
        var persona = Persona.Create(Guid.NewGuid(), "Sarah", "Lead");
        var goals = new[] { "Track tasks", "Identify blockers" };
        var painPoints = new[] { "No visibility", "Manual updates" };

        persona.Update("Sarah Updated", "Senior Lead", TechnicalLevel.High, goals, painPoints, "I need better tools");

        persona.Name.Should().Be("Sarah Updated");
        persona.Role.Should().Be("Senior Lead");
        persona.TechnicalLevel.Should().Be(TechnicalLevel.High);
        persona.Goals.Should().BeEquivalentTo(goals);
        persona.PainPoints.Should().BeEquivalentTo(painPoints);
        persona.Quote.Should().Be("I need better tools");
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "PersonaTests" -v n
```
Expected: FAIL

- [ ] **Step 3: Create Persona entity**

```csharp
// src/Backend/Braavo.Core/Entities/Persona.cs
namespace Braavo.Core.Entities;

public enum TechnicalLevel
{
    Low,
    Medium,
    High
}

public class Persona
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public TechnicalLevel TechnicalLevel { get; private set; }
    public string[] Goals { get; private set; } = [];
    public string[] PainPoints { get; private set; } = [];
    public string[] Motivations { get; private set; } = [];
    public string Quote { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Persona() { }

    public static Persona Create(Guid productId, string name, string role)
    {
        return new Persona
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            Role = role,
            TechnicalLevel = TechnicalLevel.Medium,
            Goals = [],
            PainPoints = [],
            Motivations = [],
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string role, TechnicalLevel technicalLevel, string[] goals, string[] painPoints, string quote)
    {
        Name = name;
        Role = role;
        TechnicalLevel = technicalLevel;
        Goals = goals;
        PainPoints = painPoints;
        Quote = quote;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

- [ ] **Step 4: Create IPersonaRepository interface**

```csharp
// src/Backend/Braavo.Core/Interfaces/IPersonaRepository.cs
using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IPersonaRepository
{
    Task<Persona?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Persona>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task AddAsync(Persona persona, CancellationToken ct = default);
    Task UpdateAsync(Persona persona, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

- [ ] **Step 5: Run tests to verify they pass**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "PersonaTests" -v n
```
Expected: 2 tests PASS

- [ ] **Step 6: Commit**

```bash
git add -A && git commit -m "feat: add Persona entity with goals and pain points"
```

---

### Task 3: UserStory Entity and Repository Interface

**Files:**
- Create: `src/Backend/Braavo.Core/Entities/UserStory.cs`
- Create: `src/Backend/Braavo.Core/Interfaces/IUserStoryRepository.cs`
- Test: `tests/Braavo.UnitTests/Entities/UserStoryTests.cs`

**Interfaces:**
- Consumes: `Product.Id`, `Persona.Id` (optional link)
- Produces: `UserStory` entity, `IUserStoryRepository` interface, `StoryPriority` enum

- [ ] **Step 1: Write the failing test**

```csharp
// tests/Braavo.UnitTests/Entities/UserStoryTests.cs
using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Entities;

public class UserStoryTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var productId = Guid.NewGuid();
        var asA = "team lead";
        var iWant = "to see all tasks in one dashboard";
        var soThat = "I can identify blockers quickly";

        var story = UserStory.Create(productId, asA, iWant, soThat);

        story.Id.Should().NotBeEmpty();
        story.ProductId.Should().Be(productId);
        story.AsA.Should().Be(asA);
        story.IWant.Should().Be(iWant);
        story.SoThat.Should().Be(soThat);
        story.Priority.Should().Be(StoryPriority.Should);
        story.AcceptanceCriteria.Should().BeEmpty();
    }

    [Fact]
    public void LinkToPersona_SetsPersonaId()
    {
        var story = UserStory.Create(Guid.NewGuid(), "user", "action", "benefit");
        var personaId = Guid.NewGuid();

        story.LinkToPersona(personaId);

        story.PersonaId.Should().Be(personaId);
    }

    [Fact]
    public void AddAcceptanceCriteria_AddsToList()
    {
        var story = UserStory.Create(Guid.NewGuid(), "user", "action", "benefit");

        story.AddAcceptanceCriteria("Given X, When Y, Then Z");

        story.AcceptanceCriteria.Should().Contain("Given X, When Y, Then Z");
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "UserStoryTests" -v n
```
Expected: FAIL

- [ ] **Step 3: Create UserStory entity**

```csharp
// src/Backend/Braavo.Core/Entities/UserStory.cs
namespace Braavo.Core.Entities;

public enum StoryPriority
{
    Must,
    Should,
    Could,
    Wont
}

public class UserStory
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? PersonaId { get; private set; }
    public string AsA { get; private set; } = string.Empty;
    public string IWant { get; private set; } = string.Empty;
    public string SoThat { get; private set; } = string.Empty;
    public StoryPriority Priority { get; private set; }
    public string[] AcceptanceCriteria { get; private set; } = [];
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserStory() { }

    public static UserStory Create(Guid productId, string asA, string iWant, string soThat)
    {
        return new UserStory
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            AsA = asA,
            IWant = iWant,
            SoThat = soThat,
            Priority = StoryPriority.Should,
            AcceptanceCriteria = [],
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string asA, string iWant, string soThat, StoryPriority priority)
    {
        AsA = asA;
        IWant = iWant;
        SoThat = soThat;
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToPersona(Guid? personaId)
    {
        PersonaId = personaId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAcceptanceCriteria(string criteria)
    {
        AcceptanceCriteria = [.. AcceptanceCriteria, criteria];
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAcceptanceCriteria(string[] criteria)
    {
        AcceptanceCriteria = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

- [ ] **Step 4: Create IUserStoryRepository interface**

```csharp
// src/Backend/Braavo.Core/Interfaces/IUserStoryRepository.cs
using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IUserStoryRepository
{
    Task<UserStory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserStory>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IReadOnlyList<UserStory>> GetByPersonaIdAsync(Guid personaId, CancellationToken ct = default);
    Task AddAsync(UserStory story, CancellationToken ct = default);
    Task UpdateAsync(UserStory story, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

- [ ] **Step 5: Run tests to verify they pass**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "UserStoryTests" -v n
```
Expected: 3 tests PASS

- [ ] **Step 6: Commit**

```bash
git add -A && git commit -m "feat: add UserStory entity with acceptance criteria"
```

---

### Task 4: Feature Entity and Repository Interface

**Files:**
- Create: `src/Backend/Braavo.Core/Entities/Feature.cs`
- Create: `src/Backend/Braavo.Core/Interfaces/IFeatureRepository.cs`
- Test: `tests/Braavo.UnitTests/Entities/FeatureTests.cs`

**Interfaces:**
- Consumes: `Product.Id`
- Produces: `Feature` entity, `IFeatureRepository` interface, `FeaturePhase` enum

- [ ] **Step 1: Write the failing test**

```csharp
// tests/Braavo.UnitTests/Entities/FeatureTests.cs
using Braavo.Core.Entities;
using FluentAssertions;

namespace Braavo.UnitTests.Entities;

public class FeatureTests
{
    [Fact]
    public void Create_WithValidData_SetsProperties()
    {
        var productId = Guid.NewGuid();
        var name = "Dashboard";
        var description = "Team task overview";

        var feature = Feature.Create(productId, name, description);

        feature.Id.Should().NotBeEmpty();
        feature.ProductId.Should().Be(productId);
        feature.Name.Should().Be(name);
        feature.Description.Should().Be(description);
        feature.Phase.Should().Be(FeaturePhase.Mvp);
        feature.LinkedStoryIds.Should().BeEmpty();
    }

    [Fact]
    public void LinkToStory_AddsStoryId()
    {
        var feature = Feature.Create(Guid.NewGuid(), "Feature", "Desc");
        var storyId = Guid.NewGuid();

        feature.LinkToStory(storyId);

        feature.LinkedStoryIds.Should().Contain(storyId);
    }

    [Fact]
    public void ChangePhase_UpdatesPhase()
    {
        var feature = Feature.Create(Guid.NewGuid(), "Feature", "Desc");

        feature.ChangePhase(FeaturePhase.Enhanced);

        feature.Phase.Should().Be(FeaturePhase.Enhanced);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FeatureTests" -v n
```
Expected: FAIL

- [ ] **Step 3: Create Feature entity**

```csharp
// src/Backend/Braavo.Core/Entities/Feature.cs
namespace Braavo.Core.Entities;

public enum FeaturePhase
{
    Mvp,
    Enhanced,
    Future
}

public enum EffortSize
{
    Small,
    Medium,
    Large
}

public class Feature
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public FeaturePhase Phase { get; private set; }
    public EffortSize? Effort { get; private set; }
    public Guid[] LinkedStoryIds { get; private set; } = [];
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Feature() { }

    public static Feature Create(Guid productId, string name, string description)
    {
        return new Feature
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            Description = description,
            Phase = FeaturePhase.Mvp,
            LinkedStoryIds = [],
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, EffortSize? effort)
    {
        Name = name;
        Description = description;
        Effort = effort;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePhase(FeaturePhase phase)
    {
        Phase = phase;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToStory(Guid storyId)
    {
        if (!LinkedStoryIds.Contains(storyId))
        {
            LinkedStoryIds = [.. LinkedStoryIds, storyId];
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UnlinkStory(Guid storyId)
    {
        LinkedStoryIds = LinkedStoryIds.Where(id => id != storyId).ToArray();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParent(Guid? parentId)
    {
        ParentId = parentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

- [ ] **Step 4: Create IFeatureRepository interface**

```csharp
// src/Backend/Braavo.Core/Interfaces/IFeatureRepository.cs
using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IFeatureRepository
{
    Task<Feature?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Feature>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IReadOnlyList<Feature>> GetByPhaseAsync(Guid productId, FeaturePhase phase, CancellationToken ct = default);
    Task AddAsync(Feature feature, CancellationToken ct = default);
    Task UpdateAsync(Feature feature, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

- [ ] **Step 5: Run tests to verify they pass**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "FeatureTests" -v n
```
Expected: 3 tests PASS

- [ ] **Step 6: Commit**

```bash
git add -A && git commit -m "feat: add Feature entity with phase and story linking"
```

---

### Task 5: EF Core Configurations and DbContext Update

**Files:**
- Create: `src/Backend/Braavo.Infrastructure/Data/Configurations/ProductConfiguration.cs`
- Create: `src/Backend/Braavo.Infrastructure/Data/Configurations/ProductVersionConfiguration.cs`
- Create: `src/Backend/Braavo.Infrastructure/Data/Configurations/PersonaConfiguration.cs`
- Create: `src/Backend/Braavo.Infrastructure/Data/Configurations/UserStoryConfiguration.cs`
- Create: `src/Backend/Braavo.Infrastructure/Data/Configurations/FeatureConfiguration.cs`
- Modify: `src/Backend/Braavo.Infrastructure/Data/BraavoDbContext.cs`

**Interfaces:**
- Consumes: `Product`, `ProductVersion`, `Persona`, `UserStory`, `Feature` entities
- Produces: EF Core configurations with table mappings, `DbSet<>` properties in context

- [ ] **Step 1: Create ProductConfiguration**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Configurations/ProductConfiguration.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.OwnerId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("owner_id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(p => p.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(p => p.Categories).HasColumnName("categories").HasColumnType("text[]");
        builder.Property(p => p.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(p => p.Version).HasColumnName("version");
        builder.Property(p => p.CompletionPercentage).HasColumnName("completion_percentage");
        
        builder.Property(p => p.Vision).HasColumnName("vision").HasColumnType("text");
        builder.Property(p => p.ProblemStatement).HasColumnName("problem_statement").HasColumnType("text");
        builder.Property(p => p.ValueProposition).HasColumnName("value_proposition").HasColumnType("text");
        builder.Property(p => p.TargetMarket).HasColumnName("target_market").HasColumnType("text[]");
        builder.Property(p => p.BusinessGoals).HasColumnName("business_goals").HasColumnType("text[]");
        
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(p => p.OwnerId);
    }
}
```

- [ ] **Step 2: Create ProductVersionConfiguration**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Configurations/ProductVersionConfiguration.cs
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
{
    public void Configure(EntityTypeBuilder<ProductVersion> builder)
    {
        builder.ToTable("product_versions");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("id");
        builder.Property(v => v.ProductId).HasColumnName("product_id");
        builder.Property(v => v.VersionNumber).HasColumnName("version_number");
        builder.Property(v => v.Snapshot).HasColumnName("snapshot").HasColumnType("jsonb");
        builder.Property(v => v.Comment).HasColumnName("comment").HasMaxLength(500);
        builder.Property(v => v.CreatedBy)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("created_by");
        builder.Property(v => v.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(v => v.ProductId);
        builder.HasIndex(v => new { v.ProductId, v.VersionNumber }).IsUnique();
    }
}
```

- [ ] **Step 3: Create PersonaConfiguration**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Configurations/PersonaConfiguration.cs
using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("personas");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.ProductId).HasColumnName("product_id");
        builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(p => p.Role).HasColumnName("role").HasMaxLength(255);
        builder.Property(p => p.TechnicalLevel).HasColumnName("technical_level").HasConversion<string>();
        builder.Property(p => p.Goals).HasColumnName("goals").HasColumnType("text[]");
        builder.Property(p => p.PainPoints).HasColumnName("pain_points").HasColumnType("text[]");
        builder.Property(p => p.Motivations).HasColumnName("motivations").HasColumnType("text[]");
        builder.Property(p => p.Quote).HasColumnName("quote").HasColumnType("text");
        builder.Property(p => p.SortOrder).HasColumnName("sort_order");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(p => p.ProductId);
    }
}
```

- [ ] **Step 4: Create UserStoryConfiguration**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Configurations/UserStoryConfiguration.cs
using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class UserStoryConfiguration : IEntityTypeConfiguration<UserStory>
{
    public void Configure(EntityTypeBuilder<UserStory> builder)
    {
        builder.ToTable("user_stories");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.ProductId).HasColumnName("product_id");
        builder.Property(s => s.PersonaId).HasColumnName("persona_id");
        builder.Property(s => s.AsA).HasColumnName("as_a").HasMaxLength(255).IsRequired();
        builder.Property(s => s.IWant).HasColumnName("i_want").HasColumnType("text").IsRequired();
        builder.Property(s => s.SoThat).HasColumnName("so_that").HasColumnType("text").IsRequired();
        builder.Property(s => s.Priority).HasColumnName("priority").HasConversion<string>();
        builder.Property(s => s.AcceptanceCriteria).HasColumnName("acceptance_criteria").HasColumnType("text[]");
        builder.Property(s => s.SortOrder).HasColumnName("sort_order");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.PersonaId);
    }
}
```

- [ ] **Step 5: Create FeatureConfiguration**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Configurations/FeatureConfiguration.cs
using Braavo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Braavo.Infrastructure.Data.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("features");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnName("id");
        builder.Property(f => f.ProductId).HasColumnName("product_id");
        builder.Property(f => f.ParentId).HasColumnName("parent_id");
        builder.Property(f => f.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(f => f.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(f => f.Phase).HasColumnName("phase").HasConversion<string>();
        builder.Property(f => f.Effort).HasColumnName("effort").HasConversion<string>();
        builder.Property(f => f.LinkedStoryIds).HasColumnName("linked_story_ids").HasColumnType("uuid[]");
        builder.Property(f => f.SortOrder).HasColumnName("sort_order");
        builder.Property(f => f.CreatedAt).HasColumnName("created_at");
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(f => f.ProductId);
        builder.HasIndex(f => f.ParentId);
        builder.HasIndex(f => f.Phase);
    }
}
```

- [ ] **Step 6: Update BraavoDbContext**

Add these lines to `src/Backend/Braavo.Infrastructure/Data/BraavoDbContext.cs`:

```csharp
// Add to the class body after existing DbSet properties:
public DbSet<Product> Products => Set<Product>();
public DbSet<ProductVersion> ProductVersions => Set<ProductVersion>();
public DbSet<Persona> Personas => Set<Persona>();
public DbSet<UserStory> UserStories => Set<UserStory>();
public DbSet<Feature> Features => Set<Feature>();
```

Add to OnModelCreating after existing ApplyConfiguration calls:
```csharp
modelBuilder.ApplyConfiguration(new ProductConfiguration());
modelBuilder.ApplyConfiguration(new ProductVersionConfiguration());
modelBuilder.ApplyConfiguration(new PersonaConfiguration());
modelBuilder.ApplyConfiguration(new UserStoryConfiguration());
modelBuilder.ApplyConfiguration(new FeatureConfiguration());
```

- [ ] **Step 7: Verify build succeeds**

```bash
cd src/Backend && dotnet build Braavo.slnx
```
Expected: Build succeeded

- [ ] **Step 8: Commit**

```bash
git add -A && git commit -m "feat: add EF Core configurations for Product, Persona, UserStory, Feature"
```

---

### Task 6: Repository Implementations

**Files:**
- Create: `src/Backend/Braavo.Infrastructure/Repositories/ProductRepository.cs`
- Create: `src/Backend/Braavo.Infrastructure/Repositories/PersonaRepository.cs`
- Create: `src/Backend/Braavo.Infrastructure/Repositories/UserStoryRepository.cs`
- Create: `src/Backend/Braavo.Infrastructure/Repositories/FeatureRepository.cs`
- Modify: `src/Backend/Braavo.Infrastructure/DependencyInjection.cs`

**Interfaces:**
- Consumes: `IProductRepository`, `IPersonaRepository`, `IUserStoryRepository`, `IFeatureRepository`, `BraavoDbContext`
- Produces: Repository implementations registered in DI

- [ ] **Step 1: Create ProductRepository**

```csharp
// src/Backend/Braavo.Infrastructure/Repositories/ProductRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly BraavoDbContext _context;

    public ProductRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Product>> GetByOwnerIdAsync(UserId ownerId, CancellationToken ct = default)
    {
        return await _context.Products
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await GetByIdAsync(id, ct);
        if (product is not null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task AddVersionAsync(ProductVersion version, CancellationToken ct = default)
    {
        await _context.ProductVersions.AddAsync(version, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ProductVersion>> GetVersionsAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.ProductVersions
            .Where(v => v.ProductId == productId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(ct);
    }
}
```

- [ ] **Step 2: Create PersonaRepository**

```csharp
// src/Backend/Braavo.Infrastructure/Repositories/PersonaRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class PersonaRepository : IPersonaRepository
{
    private readonly BraavoDbContext _context;

    public PersonaRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Persona?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Personas.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Persona>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Personas
            .Where(p => p.ProductId == productId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Persona persona, CancellationToken ct = default)
    {
        await _context.Personas.AddAsync(persona, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Persona persona, CancellationToken ct = default)
    {
        _context.Personas.Update(persona);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var persona = await GetByIdAsync(id, ct);
        if (persona is not null)
        {
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

- [ ] **Step 3: Create UserStoryRepository**

```csharp
// src/Backend/Braavo.Infrastructure/Repositories/UserStoryRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class UserStoryRepository : IUserStoryRepository
{
    private readonly BraavoDbContext _context;

    public UserStoryRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<UserStory?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.UserStories.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IReadOnlyList<UserStory>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.UserStories
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserStory>> GetByPersonaIdAsync(Guid personaId, CancellationToken ct = default)
    {
        return await _context.UserStories
            .Where(s => s.PersonaId == personaId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(ct);
    }

    public async Task AddAsync(UserStory story, CancellationToken ct = default)
    {
        await _context.UserStories.AddAsync(story, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UserStory story, CancellationToken ct = default)
    {
        _context.UserStories.Update(story);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var story = await GetByIdAsync(id, ct);
        if (story is not null)
        {
            _context.UserStories.Remove(story);
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

- [ ] **Step 4: Create FeatureRepository**

```csharp
// src/Backend/Braavo.Infrastructure/Repositories/FeatureRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly BraavoDbContext _context;

    public FeatureRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Feature?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Features.FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IReadOnlyList<Feature>> GetByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _context.Features
            .Where(f => f.ProductId == productId)
            .OrderBy(f => f.Phase)
            .ThenBy(f => f.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Feature>> GetByPhaseAsync(Guid productId, FeaturePhase phase, CancellationToken ct = default)
    {
        return await _context.Features
            .Where(f => f.ProductId == productId && f.Phase == phase)
            .OrderBy(f => f.SortOrder)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Feature feature, CancellationToken ct = default)
    {
        await _context.Features.AddAsync(feature, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Feature feature, CancellationToken ct = default)
    {
        _context.Features.Update(feature);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var feature = await GetByIdAsync(id, ct);
        if (feature is not null)
        {
            _context.Features.Remove(feature);
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

- [ ] **Step 5: Update DependencyInjection.cs**

Add to `src/Backend/Braavo.Infrastructure/DependencyInjection.cs` in the `AddInfrastructure` method:

```csharp
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IPersonaRepository, PersonaRepository>();
services.AddScoped<IUserStoryRepository, UserStoryRepository>();
services.AddScoped<IFeatureRepository, FeatureRepository>();
```

- [ ] **Step 6: Verify build succeeds**

```bash
cd src/Backend && dotnet build Braavo.slnx
```
Expected: Build succeeded

- [ ] **Step 7: Commit**

```bash
git add -A && git commit -m "feat: add repository implementations for Product, Persona, UserStory, Feature"
```

---

### Task 7: CreateProduct Command and Handler

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/Products/CreateProductCommand.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Products/CreateProductHandler.cs`
- Test: `tests/Braavo.UnitTests/UseCases/CreateProductHandlerTests.cs`

**Interfaces:**
- Consumes: `IProductRepository`, `Product.Create()`
- Produces: `CreateProductCommand`, `CreateProductResponse`, `CreateProductHandler`

- [ ] **Step 1: Write the failing test**

```csharp
// tests/Braavo.UnitTests/UseCases/CreateProductHandlerTests.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _handler = new CreateProductHandler(_productRepo);
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesProduct()
    {
        var command = new CreateProductCommand(
            Name: "TaskFlow",
            Description: "A task management app",
            UserId: Guid.NewGuid()
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.ProductId.Should().NotBeEmpty();
        await _productRepo.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyName_ReturnsError()
    {
        var command = new CreateProductCommand(
            Name: "",
            Description: "Description",
            UserId: Guid.NewGuid()
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Product name is required");
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "CreateProductHandlerTests" -v n
```
Expected: FAIL

- [ ] **Step 3: Create CreateProductCommand**

```csharp
// src/Backend/Braavo.Core/UseCases/Products/CreateProductCommand.cs
using MediatR;

namespace Braavo.Core.UseCases.Products;

public record CreateProductCommand(
    string Name,
    string Description,
    Guid UserId,
    string[]? Categories = null
) : IRequest<CreateProductResponse>;

public record CreateProductResponse(
    Guid ProductId,
    bool Success,
    string? Error = null
);
```

- [ ] **Step 4: Create CreateProductHandler**

```csharp
// src/Backend/Braavo.Core/UseCases/Products/CreateProductHandler.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IProductRepository _productRepo;

    public CreateProductHandler(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new CreateProductResponse(Guid.Empty, false, "Product name is required");
        }

        var product = Product.Create(
            request.Name,
            request.Description,
            UserId.From(request.UserId)
        );

        await _productRepo.AddAsync(product, ct);

        return new CreateProductResponse(product.Id, true);
    }
}
```

- [ ] **Step 5: Run tests to verify they pass**

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "CreateProductHandlerTests" -v n
```
Expected: 2 tests PASS

- [ ] **Step 6: Commit**

```bash
git add -A && git commit -m "feat: add CreateProduct command and handler"
```

---

### Task 8: GetProduct and ListProducts Queries

**Files:**
- Create: `src/Backend/Braavo.Core/UseCases/Products/GetProductQuery.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Products/GetProductHandler.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Products/ListProductsQuery.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Products/ListProductsHandler.cs`
- Test: `tests/Braavo.UnitTests/UseCases/GetProductHandlerTests.cs`

**Interfaces:**
- Consumes: `IProductRepository`, `IPersonaRepository`, `IUserStoryRepository`, `IFeatureRepository`
- Produces: `GetProductQuery`, `ListProductsQuery` with full DTOs including related entities

- [ ] **Step 1: Create GetProductQuery and response DTOs**

```csharp
// src/Backend/Braavo.Core/UseCases/Products/GetProductQuery.cs
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
```

- [ ] **Step 2: Create GetProductHandler**

```csharp
// src/Backend/Braavo.Core/UseCases/Products/GetProductHandler.cs
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
```

- [ ] **Step 3: Create ListProductsQuery and handler**

```csharp
// src/Backend/Braavo.Core/UseCases/Products/ListProductsQuery.cs
using MediatR;

namespace Braavo.Core.UseCases.Products;

public record ListProductsQuery(Guid UserId) : IRequest<IReadOnlyList<ProductSummaryDto>>;

public record ProductSummaryDto(
    Guid Id,
    string Name,
    string Description,
    string Status,
    int CompletionPercentage,
    DateTime UpdatedAt
);
```

```csharp
// src/Backend/Braavo.Core/UseCases/Products/ListProductsHandler.cs
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Products;

public class ListProductsHandler : IRequestHandler<ListProductsQuery, IReadOnlyList<ProductSummaryDto>>
{
    private readonly IProductRepository _productRepo;

    public ListProductsHandler(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IReadOnlyList<ProductSummaryDto>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        var products = await _productRepo.GetByOwnerIdAsync(UserId.From(request.UserId), ct);
        
        return products.Select(p => new ProductSummaryDto(
            p.Id,
            p.Name,
            p.Description,
            p.Status.ToString(),
            p.CompletionPercentage,
            p.UpdatedAt
        )).ToList();
    }
}
```

- [ ] **Step 4: Write and run tests**

```csharp
// tests/Braavo.UnitTests/UseCases/GetProductHandlerTests.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using FluentAssertions;
using NSubstitute;

namespace Braavo.UnitTests.UseCases;

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepo = Substitute.For<IProductRepository>();
    private readonly IPersonaRepository _personaRepo = Substitute.For<IPersonaRepository>();
    private readonly IUserStoryRepository _storyRepo = Substitute.For<IUserStoryRepository>();
    private readonly IFeatureRepository _featureRepo = Substitute.For<IFeatureRepository>();
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _handler = new GetProductHandler(_productRepo, _personaRepo, _storyRepo, _featureRepo);
    }

    [Fact]
    public async Task Handle_WithValidOwner_ReturnsProduct()
    {
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(userId));
        
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _personaRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns([]);
        _storyRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns([]);
        _featureRepo.GetByProductIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns([]);

        var result = await _handler.Handle(new GetProductQuery(product.Id, userId), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Handle_WithWrongOwner_ReturnsNull()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", UserId.From(ownerId));
        
        _productRepo.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _handler.Handle(new GetProductQuery(product.Id, otherUserId), CancellationToken.None);

        result.Should().BeNull();
    }
}
```

```bash
cd src/Backend && dotnet test tests/Braavo.UnitTests --filter "GetProductHandlerTests" -v n
```
Expected: 2 tests PASS

- [ ] **Step 5: Commit**

```bash
git add -A && git commit -m "feat: add GetProduct and ListProducts queries"
```

---

### Task 9: Product API Endpoints

**Files:**
- Create: `src/Backend/Braavo.Api/Endpoints/Products/CreateProductEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Products/GetProductEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Products/ListProductsEndpoint.cs`

**Interfaces:**
- Consumes: `IMediator`, JWT claims for UserId
- Produces: FastEndpoints for products CRUD

- [ ] **Step 1: Create CreateProductEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Products/CreateProductEndpoint.cs
using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public record CreateProductRequest(string Name, string Description, string[]? Categories);

public class CreateProductEndpoint : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly IMediator _mediator;

    public CreateProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/products");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var command = new CreateProductCommand(req.Name, req.Description, userId, req.Categories);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendCreatedAtAsync<GetProductEndpoint>(
            new { id = result.ProductId },
            result,
            cancellation: ct
        );
    }
}
```

- [ ] **Step 2: Create GetProductEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Products/GetProductEndpoint.cs
using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public class GetProductEndpoint : EndpointWithoutRequest<GetProductResponse>
{
    private readonly IMediator _mediator;

    public GetProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("id");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var result = await _mediator.Send(new GetProductQuery(productId, userId), ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}
```

- [ ] **Step 3: Create ListProductsEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Products/ListProductsEndpoint.cs
using System.Security.Claims;
using Braavo.Core.UseCases.Products;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Products;

public class ListProductsEndpoint : EndpointWithoutRequest<IReadOnlyList<ProductSummaryDto>>
{
    private readonly IMediator _mediator;

    public ListProductsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new ListProductsQuery(userId), ct);
        await SendOkAsync(result, ct);
    }
}
```

- [ ] **Step 4: Verify build succeeds**

```bash
cd src/Backend && dotnet build Braavo.slnx
```
Expected: Build succeeded

- [ ] **Step 5: Commit**

```bash
git add -A && git commit -m "feat: add Product API endpoints"
```

---

### Task 10: Database Migration

**Files:**
- Create: EF Core migration via CLI

**Interfaces:**
- Consumes: EF Core configurations from Task 5
- Produces: Migration files for new tables

- [ ] **Step 1: Create migration**

```bash
cd src/Backend/Braavo.Infrastructure && dotnet ef migrations add AddProductEntities --startup-project ../Braavo.Api
```

- [ ] **Step 2: Review generated migration**

Check the generated migration file in `Migrations/` folder. Verify it creates:
- `products` table with all columns
- `product_versions` table with foreign key
- `personas` table with foreign key to products
- `user_stories` table with foreign keys
- `features` table with indexes

- [ ] **Step 3: Apply migration (development only)**

```bash
cd src/Backend/Braavo.Infrastructure && dotnet ef database update --startup-project ../Braavo.Api
```

- [ ] **Step 4: Commit**

```bash
git add -A && git commit -m "feat: add database migration for Product entities"
```

---

### Task 11: Frontend Products Slice and API

**Files:**
- Create: `src/Frontend/src/store/slices/productsSlice.ts`
- Create: `src/Frontend/src/api/products.ts`

**Interfaces:**
- Consumes: Redux Toolkit patterns, existing auth token handling
- Produces: Products state management with CRUD operations

- [ ] **Step 1: Create products API client**

```typescript
// src/Frontend/src/api/products.ts
import { api } from './client';

export interface ProductSummary {
  id: string;
  name: string;
  description: string;
  status: string;
  completionPercentage: number;
  updatedAt: string;
}

export interface PersonaDto {
  id: string;
  name: string;
  role: string;
  technicalLevel: string;
  goals: string[];
  painPoints: string[];
  quote: string;
}

export interface UserStoryDto {
  id: string;
  personaId: string | null;
  asA: string;
  iWant: string;
  soThat: string;
  priority: string;
  acceptanceCriteria: string[];
}

export interface FeatureDto {
  id: string;
  parentId: string | null;
  name: string;
  description: string;
  phase: string;
  effort: string | null;
  linkedStoryIds: string[];
}

export interface Product extends ProductSummary {
  version: number;
  vision: string;
  problemStatement: string;
  valueProposition: string;
  targetMarket: string[];
  businessGoals: string[];
  createdAt: string;
  personas: PersonaDto[];
  userStories: UserStoryDto[];
  features: FeatureDto[];
}

export interface CreateProductRequest {
  name: string;
  description: string;
  categories?: string[];
}

export const productsApi = {
  list: () => api.get<ProductSummary[]>('/api/products'),
  get: (id: string) => api.get<Product>(`/api/products/${id}`),
  create: (data: CreateProductRequest) => api.post<{ productId: string }>('/api/products', data),
  delete: (id: string) => api.delete(`/api/products/${id}`),
};
```

- [ ] **Step 2: Create products slice**

```typescript
// src/Frontend/src/store/slices/productsSlice.ts
import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { productsApi, Product, ProductSummary } from '../../api/products';

interface ProductsState {
  items: ProductSummary[];
  currentProduct: Product | null;
  loading: boolean;
  error: string | null;
}

const initialState: ProductsState = {
  items: [],
  currentProduct: null,
  loading: false,
  error: null,
};

export const fetchProducts = createAsyncThunk('products/fetchAll', async () => {
  const response = await productsApi.list();
  return response.data;
});

export const fetchProduct = createAsyncThunk('products/fetchOne', async (id: string) => {
  const response = await productsApi.get(id);
  return response.data;
});

export const createProduct = createAsyncThunk(
  'products/create',
  async (data: { name: string; description: string }) => {
    const response = await productsApi.create(data);
    return response.data;
  }
);

const productsSlice = createSlice({
  name: 'products',
  initialState,
  reducers: {
    clearCurrentProduct: (state) => {
      state.currentProduct = null;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchProducts.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProducts.fulfilled, (state, action: PayloadAction<ProductSummary[]>) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchProducts.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch products';
      })
      .addCase(fetchProduct.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchProduct.fulfilled, (state, action: PayloadAction<Product>) => {
        state.loading = false;
        state.currentProduct = action.payload;
      })
      .addCase(fetchProduct.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch product';
      })
      .addCase(createProduct.fulfilled, (state) => {
        state.loading = false;
      });
  },
});

export const { clearCurrentProduct, clearError } = productsSlice.actions;
export default productsSlice.reducer;
```

- [ ] **Step 3: Add to store**

Update `src/Frontend/src/store/index.ts` to include productsSlice:

```typescript
import productsReducer from './slices/productsSlice';

// In configureStore:
reducer: {
  // existing reducers...
  products: productsReducer,
}
```

- [ ] **Step 4: Verify frontend builds**

```bash
cd src/Frontend && npm run build
```
Expected: Build succeeded

- [ ] **Step 5: Commit**

```bash
git add -A && git commit -m "feat: add products Redux slice and API client"
```

---

### Task 12: Frontend Welcome and Products Pages

**Files:**
- Create: `src/Frontend/src/pages/WelcomePage.tsx`
- Create: `src/Frontend/src/pages/ProductListPage.tsx`
- Create: `src/Frontend/src/components/products/ProductCard.tsx`
- Modify: `src/Frontend/src/App.tsx` (add routes)

**Interfaces:**
- Consumes: `productsSlice`, React Router
- Produces: Welcome page with product list

- [ ] **Step 1: Create ProductCard component**

```tsx
// src/Frontend/src/components/products/ProductCard.tsx
import { Link } from 'react-router-dom';
import { ProductSummary } from '../../api/products';

interface ProductCardProps {
  product: ProductSummary;
}

export function ProductCard({ product }: ProductCardProps) {
  const statusColors: Record<string, string> = {
    Draft: 'bg-gray-100 text-gray-800',
    InProgress: 'bg-blue-100 text-blue-800',
    Review: 'bg-yellow-100 text-yellow-800',
    Final: 'bg-green-100 text-green-800',
  };

  return (
    <Link
      to={`/products/${product.id}`}
      className="block p-6 bg-white rounded-lg border border-gray-200 hover:border-blue-500 transition-colors"
    >
      <div className="flex justify-between items-start mb-2">
        <h3 className="text-lg font-semibold text-gray-900">{product.name}</h3>
        <span className={`px-2 py-1 text-xs rounded-full ${statusColors[product.status] ?? statusColors.Draft}`}>
          {product.status}
        </span>
      </div>
      <p className="text-gray-600 text-sm mb-4 line-clamp-2">{product.description}</p>
      <div className="flex justify-between items-center">
        <div className="w-full bg-gray-200 rounded-full h-2">
          <div
            className="bg-blue-600 h-2 rounded-full"
            style={{ width: `${product.completionPercentage}%` }}
          />
        </div>
        <span className="ml-2 text-sm text-gray-500">{product.completionPercentage}%</span>
      </div>
    </Link>
  );
}
```

- [ ] **Step 2: Create ProductListPage**

```tsx
// src/Frontend/src/pages/ProductListPage.tsx
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchProducts } from '../store/slices/productsSlice';
import { ProductCard } from '../components/products/ProductCard';

export function ProductListPage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { items, loading, error } = useAppSelector((state) => state.products);

  useEffect(() => {
    dispatch(fetchProducts());
  }, [dispatch]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-8 text-center text-red-600">
        <p>Error: {error}</p>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto p-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Your Products</h1>
        <button
          onClick={() => navigate('/products/new')}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          New Product
        </button>
      </div>

      {items.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-500 mb-4">No products yet. Create your first PRD!</p>
          <button
            onClick={() => navigate('/products/new')}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Create Product
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {items.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}
    </div>
  );
}
```

- [ ] **Step 3: Create WelcomePage**

```tsx
// src/Frontend/src/pages/WelcomePage.tsx
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '../store/hooks';

export function WelcomePage() {
  const navigate = useNavigate();
  const user = useAppSelector((state) => state.auth.user);

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100">
      <div className="max-w-4xl mx-auto px-8 py-16">
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Welcome to Braavo{user?.email ? `, ${user.email.split('@')[0]}` : ''}
          </h1>
          <p className="text-xl text-gray-600">
            Build comprehensive Product Requirements Documents with AI assistance
          </p>
        </div>

        <div className="grid md:grid-cols-2 gap-8 mb-12">
          <div className="bg-white p-6 rounded-xl shadow-sm">
            <h3 className="text-lg font-semibold mb-2">Guided PRD Builder</h3>
            <p className="text-gray-600 mb-4">
              Step through each section of your PRD with visual controls and AI suggestions.
            </p>
          </div>
          <div className="bg-white p-6 rounded-xl shadow-sm">
            <h3 className="text-lg font-semibold mb-2">Version History</h3>
            <p className="text-gray-600 mb-4">
              Track changes and restore previous versions of your documentation.
            </p>
          </div>
        </div>

        <div className="text-center">
          <button
            onClick={() => navigate('/products')}
            className="px-8 py-4 bg-blue-600 text-white text-lg rounded-lg hover:bg-blue-700 transition-colors"
          >
            View Your Products
          </button>
        </div>
      </div>
    </div>
  );
}
```

- [ ] **Step 4: Update App.tsx routes**

Add to `src/Frontend/src/App.tsx`:

```tsx
import { WelcomePage } from './pages/WelcomePage';
import { ProductListPage } from './pages/ProductListPage';

// Add routes inside Routes:
<Route path="/welcome" element={<ProtectedRoute><WelcomePage /></ProtectedRoute>} />
<Route path="/products" element={<ProtectedRoute><ProductListPage /></ProtectedRoute>} />
```

- [ ] **Step 5: Verify frontend builds and test**

```bash
cd src/Frontend && npm run build && npm run dev
```

Open browser, login, navigate to /welcome and /products.

- [ ] **Step 6: Commit**

```bash
git add -A && git commit -m "feat: add Welcome and ProductList pages with routing"
```

---

## Completion Checklist

- [ ] All 12 tasks completed
- [ ] All tests passing
- [ ] Database migration applied
- [ ] Frontend builds without errors
- [ ] Manual testing of new routes

**Plan complete.** Ready for execution via subagent-driven-development or inline execution