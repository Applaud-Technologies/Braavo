# Guided PRD Builder Phase 2: Section Editors

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build CRUD API endpoints and interactive section editor pages for Personas, User Stories, and Features.

**Architecture:** FastEndpoints + MediatR for backend APIs following existing Product endpoint patterns. React pages with modal editors for CRUD operations. Redux Toolkit slices for state management. Tailwind CSS for styling.

**Tech Stack:** .NET 8.0, FastEndpoints, MediatR, EF Core, PostgreSQL, React 18, TypeScript 5, Redux Toolkit, Tailwind CSS

## Global Constraints

- Follow existing entity patterns: private setters, static `Create()` factory methods
- All endpoints require JWT authentication with ownership validation
- Backend port: 5153, Frontend port: 5173
- Use existing `productsApi` pattern for API clients
- Modal editors for create/edit operations (not separate pages)
- MoSCoW priority values for stories: Must, Should, Could, Wont
- Feature phases: Mvp, Enhanced, Future
- Technical levels: Low, Medium, High

---

### Task 1: Persona CRUD API Endpoints

**Files:**
- Create: `src/Backend/Braavo.Api/Endpoints/Personas/ListPersonasEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Personas/GetPersonaEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Personas/CreatePersonaEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Personas/UpdatePersonaEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Personas/DeletePersonaEndpoint.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Personas/ListPersonasQuery.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Personas/GetPersonaQuery.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Personas/CreatePersonaCommand.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Personas/UpdatePersonaCommand.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Personas/DeletePersonaCommand.cs`
- Test: `src/Backend/tests/Braavo.UnitTests/UseCases/Personas/`

**Interfaces:**
- Consumes: `Persona` entity from `Braavo.Core.Entities`, `BraavoDbContext`
- Produces: 
  - `GET /api/products/{productId}/personas` → `PersonaSummary[]`
  - `GET /api/products/{productId}/personas/{id}` → `PersonaDto`
  - `POST /api/products/{productId}/personas` → `CreatePersonaResponse`
  - `PUT /api/products/{productId}/personas/{id}` → `UpdatePersonaResponse`
  - `DELETE /api/products/{productId}/personas/{id}` → 204 No Content

- [ ] **Step 1: Create ListPersonasQuery and handler**

```csharp
// src/Backend/Braavo.Core/UseCases/Personas/ListPersonasQuery.cs
using Braavo.Core.Entities;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record PersonaSummary(
    Guid Id,
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    int SortOrder
);

public record ListPersonasQuery(Guid ProductId, Guid UserId) : IRequest<ListPersonasResult>;

public record ListPersonasResult(bool Success, PersonaSummary[] Personas, string? Error = null);

public class ListPersonasQueryHandler : IRequestHandler<ListPersonasQuery, ListPersonasResult>
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IProductRepository _productRepository;

    public ListPersonasQueryHandler(IPersonaRepository personaRepository, IProductRepository productRepository)
    {
        _personaRepository = personaRepository;
        _productRepository = productRepository;
    }

    public async Task<ListPersonasResult> Handle(ListPersonasQuery request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new ListPersonasResult(false, [], "Product not found or access denied");

        var personas = await _personaRepository.ListByProductIdAsync(request.ProductId, ct);
        var summaries = personas
            .OrderBy(p => p.SortOrder)
            .Select(p => new PersonaSummary(p.Id, p.Name, p.Role, p.TechnicalLevel, p.Goals, p.PainPoints, p.SortOrder))
            .ToArray();

        return new ListPersonasResult(true, summaries);
    }
}
```

- [ ] **Step 2: Create IPersonaRepository interface**

```csharp
// src/Backend/Braavo.Core/Interfaces/IPersonaRepository.cs
using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IPersonaRepository
{
    Task<Persona?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Persona>> ListByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task AddAsync(Persona persona, CancellationToken ct = default);
    Task UpdateAsync(Persona persona, CancellationToken ct = default);
    Task DeleteAsync(Persona persona, CancellationToken ct = default);
}
```

- [ ] **Step 3: Implement PersonaRepository in Infrastructure**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Repositories/PersonaRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Data.Repositories;

public class PersonaRepository : IPersonaRepository
{
    private readonly BraavoDbContext _context;

    public PersonaRepository(BraavoDbContext context)
    {
        _context = context;
    }

    public async Task<Persona?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Personas.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<List<Persona>> ListByProductIdAsync(Guid productId, CancellationToken ct = default)
        => await _context.Personas.Where(p => p.ProductId == productId).ToListAsync(ct);

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

    public async Task DeleteAsync(Persona persona, CancellationToken ct = default)
    {
        _context.Personas.Remove(persona);
        await _context.SaveChangesAsync(ct);
    }
}
```

- [ ] **Step 4: Register PersonaRepository in DI**

Add to `src/Backend/Braavo.Infrastructure/DependencyInjection.cs`:
```csharp
services.AddScoped<IPersonaRepository, PersonaRepository>();
```

- [ ] **Step 5: Create CreatePersonaCommand and handler**

```csharp
// src/Backend/Braavo.Core/UseCases/Personas/CreatePersonaCommand.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record CreatePersonaCommand(
    Guid ProductId,
    Guid UserId,
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
) : IRequest<CreatePersonaResult>;

public record CreatePersonaResult(bool Success, Guid? PersonaId = null, string? Error = null);

public class CreatePersonaCommandHandler : IRequestHandler<CreatePersonaCommand, CreatePersonaResult>
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IProductRepository _productRepository;

    public CreatePersonaCommandHandler(IPersonaRepository personaRepository, IProductRepository productRepository)
    {
        _personaRepository = personaRepository;
        _productRepository = productRepository;
    }

    public async Task<CreatePersonaResult> Handle(CreatePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new CreatePersonaResult(false, Error: "Product not found or access denied");

        var persona = Persona.Create(request.ProductId, request.Name, request.Role);
        persona.Update(request.Name, request.Role, request.TechnicalLevel, request.Goals, request.PainPoints, request.Quote);

        await _personaRepository.AddAsync(persona, ct);

        return new CreatePersonaResult(true, persona.Id);
    }
}
```

- [ ] **Step 6: Create UpdatePersonaCommand and handler**

```csharp
// src/Backend/Braavo.Core/UseCases/Personas/UpdatePersonaCommand.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record UpdatePersonaCommand(
    Guid PersonaId,
    Guid ProductId,
    Guid UserId,
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
) : IRequest<UpdatePersonaResult>;

public record UpdatePersonaResult(bool Success, string? Error = null);

public class UpdatePersonaCommandHandler : IRequestHandler<UpdatePersonaCommand, UpdatePersonaResult>
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IProductRepository _productRepository;

    public UpdatePersonaCommandHandler(IPersonaRepository personaRepository, IProductRepository productRepository)
    {
        _personaRepository = personaRepository;
        _productRepository = productRepository;
    }

    public async Task<UpdatePersonaResult> Handle(UpdatePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new UpdatePersonaResult(false, "Product not found or access denied");

        var persona = await _personaRepository.GetByIdAsync(request.PersonaId, ct);
        if (persona == null || persona.ProductId != request.ProductId)
            return new UpdatePersonaResult(false, "Persona not found");

        persona.Update(request.Name, request.Role, request.TechnicalLevel, request.Goals, request.PainPoints, request.Quote);
        await _personaRepository.UpdateAsync(persona, ct);

        return new UpdatePersonaResult(true);
    }
}
```

- [ ] **Step 7: Create DeletePersonaCommand and handler**

```csharp
// src/Backend/Braavo.Core/UseCases/Personas/DeletePersonaCommand.cs
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.Personas;

public record DeletePersonaCommand(Guid PersonaId, Guid ProductId, Guid UserId) : IRequest<DeletePersonaResult>;

public record DeletePersonaResult(bool Success, string? Error = null);

public class DeletePersonaCommandHandler : IRequestHandler<DeletePersonaCommand, DeletePersonaResult>
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IProductRepository _productRepository;

    public DeletePersonaCommandHandler(IPersonaRepository personaRepository, IProductRepository productRepository)
    {
        _personaRepository = personaRepository;
        _productRepository = productRepository;
    }

    public async Task<DeletePersonaResult> Handle(DeletePersonaCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new DeletePersonaResult(false, "Product not found or access denied");

        var persona = await _personaRepository.GetByIdAsync(request.PersonaId, ct);
        if (persona == null || persona.ProductId != request.ProductId)
            return new DeletePersonaResult(false, "Persona not found");

        await _personaRepository.DeleteAsync(persona, ct);

        return new DeletePersonaResult(true);
    }
}
```

- [ ] **Step 8: Create ListPersonasEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Personas/ListPersonasEndpoint.cs
using System.Security.Claims;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public class ListPersonasEndpoint : EndpointWithoutRequest<PersonaSummary[]>
{
    private readonly IMediator _mediator;

    public ListPersonasEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Get("/api/products/{productId}/personas");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("productId");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new ListPersonasQuery(productId, userId), ct);

        if (!result.Success)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(result.Personas, ct);
    }
}
```

- [ ] **Step 9: Create CreatePersonaEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Personas/CreatePersonaEndpoint.cs
using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public record CreatePersonaRequest(
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
);

public class CreatePersonaEndpoint : Endpoint<CreatePersonaRequest, CreatePersonaResult>
{
    private readonly IMediator _mediator;

    public CreatePersonaEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/products/{productId}/personas");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CreatePersonaRequest req, CancellationToken ct)
    {
        var productId = Route<Guid>("productId");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreatePersonaCommand(
            productId, userId, req.Name, req.Role, req.TechnicalLevel,
            req.Goals, req.PainPoints, req.Quote
        );

        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendCreatedAtAsync<ListPersonasEndpoint>(
            new { productId },
            result,
            cancellation: ct
        );
    }
}
```

- [ ] **Step 10: Create UpdatePersonaEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Personas/UpdatePersonaEndpoint.cs
using System.Security.Claims;
using Braavo.Core.Entities;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public record UpdatePersonaRequest(
    string Name,
    string Role,
    TechnicalLevel TechnicalLevel,
    string[] Goals,
    string[] PainPoints,
    string Quote
);

public class UpdatePersonaEndpoint : Endpoint<UpdatePersonaRequest, UpdatePersonaResult>
{
    private readonly IMediator _mediator;

    public UpdatePersonaEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Put("/api/products/{productId}/personas/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(UpdatePersonaRequest req, CancellationToken ct)
    {
        var productId = Route<Guid>("productId");
        var personaId = Route<Guid>("id");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new UpdatePersonaCommand(
            personaId, productId, userId, req.Name, req.Role, req.TechnicalLevel,
            req.Goals, req.PainPoints, req.Quote
        );

        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}
```

- [ ] **Step 11: Create DeletePersonaEndpoint**

```csharp
// src/Backend/Braavo.Api/Endpoints/Personas/DeletePersonaEndpoint.cs
using System.Security.Claims;
using Braavo.Core.UseCases.Personas;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Personas;

public class DeletePersonaEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public DeletePersonaEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Delete("/api/products/{productId}/personas/{id}");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var productId = Route<Guid>("productId");
        var personaId = Route<Guid>("id");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _mediator.Send(new DeletePersonaCommand(personaId, productId, userId), ct);

        if (!result.Success)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
```

- [ ] **Step 12: Write unit tests for persona commands**

```csharp
// src/Backend/tests/Braavo.UnitTests/UseCases/Personas/CreatePersonaCommandTests.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Personas;
using Moq;
using Xunit;

namespace Braavo.UnitTests.UseCases.Personas;

public class CreatePersonaCommandTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesPersona()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", userId);
        typeof(Product).GetProperty("Id")!.SetValue(product, productId);

        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var personaRepo = new Mock<IPersonaRepository>();
        personaRepo.Setup(r => r.AddAsync(It.IsAny<Persona>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreatePersonaCommandHandler(personaRepo.Object, productRepo.Object);
        var command = new CreatePersonaCommand(productId, userId, "Sarah", "Team Lead", 
            TechnicalLevel.Medium, ["Track tasks"], ["No visibility"], "Quote");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.PersonaId);
        personaRepo.Verify(r => r.AddAsync(It.IsAny<Persona>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongOwner_ReturnsError()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var product = Product.Create("Test", "Desc", otherUserId);
        typeof(Product).GetProperty("Id")!.SetValue(product, productId);

        var productRepo = new Mock<IProductRepository>();
        productRepo.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var personaRepo = new Mock<IPersonaRepository>();
        var handler = new CreatePersonaCommandHandler(personaRepo.Object, productRepo.Object);
        var command = new CreatePersonaCommand(productId, userId, "Sarah", "Team Lead",
            TechnicalLevel.Medium, [], [], "");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("access denied", result.Error);
    }
}
```

- [ ] **Step 13: Run tests**

Run: `cd src/Backend && dotnet test`
Expected: All tests pass

- [ ] **Step 14: Commit**

```bash
git add src/Backend/Braavo.Api/Endpoints/Personas/ src/Backend/Braavo.Core/UseCases/Personas/ src/Backend/Braavo.Core/Interfaces/IPersonaRepository.cs src/Backend/Braavo.Infrastructure/Data/Repositories/PersonaRepository.cs src/Backend/tests/Braavo.UnitTests/UseCases/Personas/
git commit -m "feat: add Persona CRUD API endpoints"
```

---

### Task 2: User Story CRUD API Endpoints

**Files:**
- Create: `src/Backend/Braavo.Api/Endpoints/UserStories/ListUserStoriesEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/UserStories/CreateUserStoryEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/UserStories/UpdateUserStoryEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/UserStories/DeleteUserStoryEndpoint.cs`
- Create: `src/Backend/Braavo.Core/UseCases/UserStories/ListUserStoriesQuery.cs`
- Create: `src/Backend/Braavo.Core/UseCases/UserStories/CreateUserStoryCommand.cs`
- Create: `src/Backend/Braavo.Core/UseCases/UserStories/UpdateUserStoryCommand.cs`
- Create: `src/Backend/Braavo.Core/UseCases/UserStories/DeleteUserStoryCommand.cs`
- Create: `src/Backend/Braavo.Core/Interfaces/IUserStoryRepository.cs`
- Create: `src/Backend/Braavo.Infrastructure/Data/Repositories/UserStoryRepository.cs`
- Test: `src/Backend/tests/Braavo.UnitTests/UseCases/UserStories/`

**Interfaces:**
- Consumes: `UserStory` entity, `IProductRepository`, `IPersonaRepository`
- Produces:
  - `GET /api/products/{productId}/stories` → `UserStorySummary[]`
  - `POST /api/products/{productId}/stories` → `CreateUserStoryResult`
  - `PUT /api/products/{productId}/stories/{id}` → `UpdateUserStoryResult`
  - `DELETE /api/products/{productId}/stories/{id}` → 204 No Content

- [ ] **Step 1: Create IUserStoryRepository interface**

```csharp
// src/Backend/Braavo.Core/Interfaces/IUserStoryRepository.cs
using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IUserStoryRepository
{
    Task<UserStory?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<UserStory>> ListByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task AddAsync(UserStory story, CancellationToken ct = default);
    Task UpdateAsync(UserStory story, CancellationToken ct = default);
    Task DeleteAsync(UserStory story, CancellationToken ct = default);
}
```

- [ ] **Step 2: Implement UserStoryRepository**

```csharp
// src/Backend/Braavo.Infrastructure/Data/Repositories/UserStoryRepository.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Braavo.Infrastructure.Data.Repositories;

public class UserStoryRepository : IUserStoryRepository
{
    private readonly BraavoDbContext _context;

    public UserStoryRepository(BraavoDbContext context) => _context = context;

    public async Task<UserStory?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.UserStories.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<List<UserStory>> ListByProductIdAsync(Guid productId, CancellationToken ct = default)
        => await _context.UserStories.Where(s => s.ProductId == productId).ToListAsync(ct);

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

    public async Task DeleteAsync(UserStory story, CancellationToken ct = default)
    {
        _context.UserStories.Remove(story);
        await _context.SaveChangesAsync(ct);
    }
}
```

- [ ] **Step 3: Register UserStoryRepository in DI**

Add to `src/Backend/Braavo.Infrastructure/DependencyInjection.cs`:
```csharp
services.AddScoped<IUserStoryRepository, UserStoryRepository>();
```

- [ ] **Step 4: Create ListUserStoriesQuery**

```csharp
// src/Backend/Braavo.Core/UseCases/UserStories/ListUserStoriesQuery.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record UserStorySummary(
    Guid Id,
    Guid? PersonaId,
    string AsA,
    string IWant,
    string SoThat,
    StoryPriority Priority,
    string[] AcceptanceCriteria,
    int SortOrder
);

public record ListUserStoriesQuery(Guid ProductId, Guid UserId) : IRequest<ListUserStoriesResult>;

public record ListUserStoriesResult(bool Success, UserStorySummary[] Stories, string? Error = null);

public class ListUserStoriesQueryHandler : IRequestHandler<ListUserStoriesQuery, ListUserStoriesResult>
{
    private readonly IUserStoryRepository _storyRepository;
    private readonly IProductRepository _productRepository;

    public ListUserStoriesQueryHandler(IUserStoryRepository storyRepository, IProductRepository productRepository)
    {
        _storyRepository = storyRepository;
        _productRepository = productRepository;
    }

    public async Task<ListUserStoriesResult> Handle(ListUserStoriesQuery request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new ListUserStoriesResult(false, [], "Product not found or access denied");

        var stories = await _storyRepository.ListByProductIdAsync(request.ProductId, ct);
        var summaries = stories
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.SortOrder)
            .Select(s => new UserStorySummary(s.Id, s.PersonaId, s.AsA, s.IWant, s.SoThat, s.Priority, s.AcceptanceCriteria, s.SortOrder))
            .ToArray();

        return new ListUserStoriesResult(true, summaries);
    }
}
```

- [ ] **Step 5: Create CreateUserStoryCommand**

```csharp
// src/Backend/Braavo.Core/UseCases/UserStories/CreateUserStoryCommand.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record CreateUserStoryCommand(
    Guid ProductId,
    Guid UserId,
    Guid? PersonaId,
    string AsA,
    string IWant,
    string SoThat,
    StoryPriority Priority,
    string[] AcceptanceCriteria
) : IRequest<CreateUserStoryResult>;

public record CreateUserStoryResult(bool Success, Guid? StoryId = null, string? Error = null);

public class CreateUserStoryCommandHandler : IRequestHandler<CreateUserStoryCommand, CreateUserStoryResult>
{
    private readonly IUserStoryRepository _storyRepository;
    private readonly IProductRepository _productRepository;

    public CreateUserStoryCommandHandler(IUserStoryRepository storyRepository, IProductRepository productRepository)
    {
        _storyRepository = storyRepository;
        _productRepository = productRepository;
    }

    public async Task<CreateUserStoryResult> Handle(CreateUserStoryCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new CreateUserStoryResult(false, Error: "Product not found or access denied");

        var story = UserStory.Create(request.ProductId, request.AsA, request.IWant, request.SoThat);
        story.Update(request.AsA, request.IWant, request.SoThat, request.Priority);
        story.SetAcceptanceCriteria(request.AcceptanceCriteria);
        if (request.PersonaId.HasValue)
            story.LinkToPersona(request.PersonaId);

        await _storyRepository.AddAsync(story, ct);

        return new CreateUserStoryResult(true, story.Id);
    }
}
```

- [ ] **Step 6: Create UpdateUserStoryCommand**

```csharp
// src/Backend/Braavo.Core/UseCases/UserStories/UpdateUserStoryCommand.cs
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record UpdateUserStoryCommand(
    Guid StoryId,
    Guid ProductId,
    Guid UserId,
    Guid? PersonaId,
    string AsA,
    string IWant,
    string SoThat,
    StoryPriority Priority,
    string[] AcceptanceCriteria
) : IRequest<UpdateUserStoryResult>;

public record UpdateUserStoryResult(bool Success, string? Error = null);

public class UpdateUserStoryCommandHandler : IRequestHandler<UpdateUserStoryCommand, UpdateUserStoryResult>
{
    private readonly IUserStoryRepository _storyRepository;
    private readonly IProductRepository _productRepository;

    public UpdateUserStoryCommandHandler(IUserStoryRepository storyRepository, IProductRepository productRepository)
    {
        _storyRepository = storyRepository;
        _productRepository = productRepository;
    }

    public async Task<UpdateUserStoryResult> Handle(UpdateUserStoryCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new UpdateUserStoryResult(false, "Product not found or access denied");

        var story = await _storyRepository.GetByIdAsync(request.StoryId, ct);
        if (story == null || story.ProductId != request.ProductId)
            return new UpdateUserStoryResult(false, "Story not found");

        story.Update(request.AsA, request.IWant, request.SoThat, request.Priority);
        story.SetAcceptanceCriteria(request.AcceptanceCriteria);
        story.LinkToPersona(request.PersonaId);

        await _storyRepository.UpdateAsync(story, ct);

        return new UpdateUserStoryResult(true);
    }
}
```

- [ ] **Step 7: Create DeleteUserStoryCommand**

```csharp
// src/Backend/Braavo.Core/UseCases/UserStories/DeleteUserStoryCommand.cs
using Braavo.Core.Interfaces;
using MediatR;

namespace Braavo.Core.UseCases.UserStories;

public record DeleteUserStoryCommand(Guid StoryId, Guid ProductId, Guid UserId) : IRequest<DeleteUserStoryResult>;

public record DeleteUserStoryResult(bool Success, string? Error = null);

public class DeleteUserStoryCommandHandler : IRequestHandler<DeleteUserStoryCommand, DeleteUserStoryResult>
{
    private readonly IUserStoryRepository _storyRepository;
    private readonly IProductRepository _productRepository;

    public DeleteUserStoryCommandHandler(IUserStoryRepository storyRepository, IProductRepository productRepository)
    {
        _storyRepository = storyRepository;
        _productRepository = productRepository;
    }

    public async Task<DeleteUserStoryResult> Handle(DeleteUserStoryCommand request, CancellationToken ct)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, ct);
        if (product == null || product.OwnerId != request.UserId)
            return new DeleteUserStoryResult(false, "Product not found or access denied");

        var story = await _storyRepository.GetByIdAsync(request.StoryId, ct);
        if (story == null || story.ProductId != request.ProductId)
            return new DeleteUserStoryResult(false, "Story not found");

        await _storyRepository.DeleteAsync(story, ct);

        return new DeleteUserStoryResult(true);
    }
}
```

- [ ] **Step 8: Create UserStory API endpoints**

Create endpoints following the same pattern as Persona endpoints:
- `ListUserStoriesEndpoint.cs` - GET /api/products/{productId}/stories
- `CreateUserStoryEndpoint.cs` - POST /api/products/{productId}/stories
- `UpdateUserStoryEndpoint.cs` - PUT /api/products/{productId}/stories/{id}
- `DeleteUserStoryEndpoint.cs` - DELETE /api/products/{productId}/stories/{id}

- [ ] **Step 9: Run tests and commit**

```bash
cd src/Backend && dotnet test
git add src/Backend/Braavo.Api/Endpoints/UserStories/ src/Backend/Braavo.Core/UseCases/UserStories/ src/Backend/Braavo.Core/Interfaces/IUserStoryRepository.cs src/Backend/Braavo.Infrastructure/Data/Repositories/UserStoryRepository.cs
git commit -m "feat: add UserStory CRUD API endpoints"
```

---

### Task 3: Feature CRUD API Endpoints

**Files:**
- Create: `src/Backend/Braavo.Api/Endpoints/Features/ListFeaturesEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Features/CreateFeatureEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Features/UpdateFeatureEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Features/DeleteFeatureEndpoint.cs`
- Create: `src/Backend/Braavo.Api/Endpoints/Features/MoveFeatureEndpoint.cs`
- Create: `src/Backend/Braavo.Core/UseCases/Features/` (all commands/queries)
- Create: `src/Backend/Braavo.Core/Interfaces/IFeatureRepository.cs`
- Create: `src/Backend/Braavo.Infrastructure/Data/Repositories/FeatureRepository.cs`

**Interfaces:**
- Consumes: `Feature` entity, `IProductRepository`
- Produces:
  - `GET /api/products/{productId}/features` → `FeatureSummary[]`
  - `POST /api/products/{productId}/features` → `CreateFeatureResult`
  - `PUT /api/products/{productId}/features/{id}` → `UpdateFeatureResult`
  - `PUT /api/products/{productId}/features/{id}/move` → `MoveFeatureResult` (change phase)
  - `DELETE /api/products/{productId}/features/{id}` → 204 No Content

- [ ] **Step 1: Create IFeatureRepository and implementation**

Follow same pattern as PersonaRepository and UserStoryRepository.

- [ ] **Step 2: Create Feature use cases**

- `ListFeaturesQuery` - list features grouped by phase
- `CreateFeatureCommand` - create new feature
- `UpdateFeatureCommand` - update feature details
- `MoveFeatureCommand` - move feature to different phase
- `DeleteFeatureCommand` - delete feature

- [ ] **Step 3: Create Feature endpoints**

- `ListFeaturesEndpoint.cs` - GET /api/products/{productId}/features
- `CreateFeatureEndpoint.cs` - POST /api/products/{productId}/features
- `UpdateFeatureEndpoint.cs` - PUT /api/products/{productId}/features/{id}
- `MoveFeatureEndpoint.cs` - PUT /api/products/{productId}/features/{id}/move
- `DeleteFeatureEndpoint.cs` - DELETE /api/products/{productId}/features/{id}

- [ ] **Step 4: Run tests and commit**

```bash
cd src/Backend && dotnet test
git add src/Backend/Braavo.Api/Endpoints/Features/ src/Backend/Braavo.Core/UseCases/Features/ src/Backend/Braavo.Core/Interfaces/IFeatureRepository.cs src/Backend/Braavo.Infrastructure/Data/Repositories/FeatureRepository.cs
git commit -m "feat: add Feature CRUD API endpoints with phase movement"
```

---

### Task 4: Frontend API Clients

**Files:**
- Create: `src/Frontend/src/api/personas.ts`
- Create: `src/Frontend/src/api/stories.ts`
- Create: `src/Frontend/src/api/features.ts`

**Interfaces:**
- Consumes: `apiClient` from `src/Frontend/src/api/client.ts`
- Produces: `personasApi`, `storiesApi`, `featuresApi` objects with CRUD methods

- [ ] **Step 1: Create personas API client**

```typescript
// src/Frontend/src/api/personas.ts
import apiClient from './client';

export interface Persona {
  id: string;
  name: string;
  role: string;
  technicalLevel: 'Low' | 'Medium' | 'High';
  goals: string[];
  painPoints: string[];
  quote: string;
  sortOrder: number;
}

export interface CreatePersonaRequest {
  name: string;
  role: string;
  technicalLevel: 'Low' | 'Medium' | 'High';
  goals: string[];
  painPoints: string[];
  quote: string;
}

export const personasApi = {
  list: (productId: string) =>
    apiClient.get<Persona[]>(`/products/${productId}/personas`),

  create: (productId: string, data: CreatePersonaRequest) =>
    apiClient.post<{ success: boolean; personaId: string }>(`/products/${productId}/personas`, data),

  update: (productId: string, personaId: string, data: CreatePersonaRequest) =>
    apiClient.put<{ success: boolean }>(`/products/${productId}/personas/${personaId}`, data),

  delete: (productId: string, personaId: string) =>
    apiClient.delete(`/products/${productId}/personas/${personaId}`),
};
```

- [ ] **Step 2: Create stories API client**

```typescript
// src/Frontend/src/api/stories.ts
import apiClient from './client';

export type StoryPriority = 'Must' | 'Should' | 'Could' | 'Wont';

export interface UserStory {
  id: string;
  personaId: string | null;
  asA: string;
  iWant: string;
  soThat: string;
  priority: StoryPriority;
  acceptanceCriteria: string[];
  sortOrder: number;
}

export interface CreateStoryRequest {
  personaId?: string;
  asA: string;
  iWant: string;
  soThat: string;
  priority: StoryPriority;
  acceptanceCriteria: string[];
}

export const storiesApi = {
  list: (productId: string) =>
    apiClient.get<UserStory[]>(`/products/${productId}/stories`),

  create: (productId: string, data: CreateStoryRequest) =>
    apiClient.post<{ success: boolean; storyId: string }>(`/products/${productId}/stories`, data),

  update: (productId: string, storyId: string, data: CreateStoryRequest) =>
    apiClient.put<{ success: boolean }>(`/products/${productId}/stories/${storyId}`, data),

  delete: (productId: string, storyId: string) =>
    apiClient.delete(`/products/${productId}/stories/${storyId}`),
};
```

- [ ] **Step 3: Create features API client**

```typescript
// src/Frontend/src/api/features.ts
import apiClient from './client';

export type FeaturePhase = 'Mvp' | 'Enhanced' | 'Future';
export type EffortSize = 'Small' | 'Medium' | 'Large';

export interface Feature {
  id: string;
  name: string;
  description: string;
  phase: FeaturePhase;
  effort: EffortSize | null;
  linkedStoryIds: string[];
  sortOrder: number;
}

export interface CreateFeatureRequest {
  name: string;
  description: string;
  phase?: FeaturePhase;
  effort?: EffortSize;
}

export interface MoveFeatureRequest {
  phase: FeaturePhase;
  sortOrder?: number;
}

export const featuresApi = {
  list: (productId: string) =>
    apiClient.get<Feature[]>(`/products/${productId}/features`),

  create: (productId: string, data: CreateFeatureRequest) =>
    apiClient.post<{ success: boolean; featureId: string }>(`/products/${productId}/features`, data),

  update: (productId: string, featureId: string, data: CreateFeatureRequest) =>
    apiClient.put<{ success: boolean }>(`/products/${productId}/features/${featureId}`, data),

  move: (productId: string, featureId: string, data: MoveFeatureRequest) =>
    apiClient.put<{ success: boolean }>(`/products/${productId}/features/${featureId}/move`, data),

  delete: (productId: string, featureId: string) =>
    apiClient.delete(`/products/${productId}/features/${featureId}`),
};
```

- [ ] **Step 4: Commit**

```bash
git add src/Frontend/src/api/personas.ts src/Frontend/src/api/stories.ts src/Frontend/src/api/features.ts
git commit -m "feat: add frontend API clients for personas, stories, features"
```

---

### Task 5: Redux Slices for Section Data

**Files:**
- Create: `src/Frontend/src/store/slices/personasSlice.ts`
- Create: `src/Frontend/src/store/slices/storiesSlice.ts`
- Create: `src/Frontend/src/store/slices/featuresSlice.ts`
- Modify: `src/Frontend/src/store/store.ts`

**Interfaces:**
- Consumes: API clients from Task 4
- Produces: Redux state and async thunks for CRUD operations

- [ ] **Step 1: Create personasSlice**

```typescript
// src/Frontend/src/store/slices/personasSlice.ts
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { personasApi, type Persona, type CreatePersonaRequest } from '../../api/personas';

interface PersonasState {
  items: Persona[];
  loading: boolean;
  error: string | null;
}

const initialState: PersonasState = {
  items: [],
  loading: false,
  error: null,
};

export const fetchPersonas = createAsyncThunk(
  'personas/fetchAll',
  async (productId: string) => {
    const response = await personasApi.list(productId);
    return response.data;
  }
);

export const createPersona = createAsyncThunk(
  'personas/create',
  async ({ productId, data }: { productId: string; data: CreatePersonaRequest }) => {
    const response = await personasApi.create(productId, data);
    return response.data;
  }
);

export const updatePersona = createAsyncThunk(
  'personas/update',
  async ({ productId, personaId, data }: { productId: string; personaId: string; data: CreatePersonaRequest }) => {
    await personasApi.update(productId, personaId, data);
    return { personaId, data };
  }
);

export const deletePersona = createAsyncThunk(
  'personas/delete',
  async ({ productId, personaId }: { productId: string; personaId: string }) => {
    await personasApi.delete(productId, personaId);
    return personaId;
  }
);

const personasSlice = createSlice({
  name: 'personas',
  initialState,
  reducers: {
    clearPersonas: (state) => {
      state.items = [];
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchPersonas.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPersonas.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchPersonas.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message ?? 'Failed to fetch personas';
      })
      .addCase(deletePersona.fulfilled, (state, action) => {
        state.items = state.items.filter((p) => p.id !== action.payload);
      });
  },
});

export const { clearPersonas } = personasSlice.actions;
export default personasSlice.reducer;
```

- [ ] **Step 2: Create storiesSlice and featuresSlice**

Follow same pattern as personasSlice.

- [ ] **Step 3: Register slices in store**

```typescript
// src/Frontend/src/store/store.ts - add imports and reducers
import personasReducer from './slices/personasSlice';
import storiesReducer from './slices/storiesSlice';
import featuresReducer from './slices/featuresSlice';

// Add to store:
personas: personasReducer,
stories: storiesReducer,
features: featuresReducer,
```

- [ ] **Step 4: Commit**

```bash
git add src/Frontend/src/store/slices/personasSlice.ts src/Frontend/src/store/slices/storiesSlice.ts src/Frontend/src/store/slices/featuresSlice.ts src/Frontend/src/store/store.ts
git commit -m "feat: add Redux slices for personas, stories, features"
```

---

### Task 6: Persona Section Page with Editor Modal

**Files:**
- Create: `src/Frontend/src/pages/PersonasPage.tsx`
- Create: `src/Frontend/src/components/prd/PersonaCard.tsx`
- Create: `src/Frontend/src/components/prd/PersonaEditor.tsx`
- Modify: `src/Frontend/src/App.tsx`

**Interfaces:**
- Consumes: `personasSlice`, `fetchProduct` from `productsSlice`
- Produces: `/products/:id/personas` route with persona cards and edit modal

- [ ] **Step 1: Create PersonaCard component**

```tsx
// src/Frontend/src/components/prd/PersonaCard.tsx
import type { Persona } from '../../api/personas';

interface PersonaCardProps {
  persona: Persona;
  onEdit: () => void;
  onDelete: () => void;
}

export function PersonaCard({ persona, onEdit, onDelete }: PersonaCardProps) {
  const techLevelWidth = { Low: '33%', Medium: '66%', High: '100%' };

  return (
    <div className="bg-white rounded-lg border border-gray-200 p-4 hover:shadow-md transition-shadow">
      <div className="flex items-start justify-between mb-3">
        <div>
          <h3 className="font-semibold text-gray-900">{persona.name}</h3>
          <p className="text-sm text-gray-500">{persona.role}</p>
        </div>
        <div className="flex gap-2">
          <button onClick={onEdit} className="text-blue-600 hover:text-blue-800 text-sm">
            Edit
          </button>
          <button onClick={onDelete} className="text-red-600 hover:text-red-800 text-sm">
            Delete
          </button>
        </div>
      </div>

      {persona.goals.length > 0 && (
        <div className="mb-3">
          <p className="text-xs font-medium text-gray-500 mb-1">Goals</p>
          <ul className="text-sm text-gray-700 space-y-1">
            {persona.goals.slice(0, 3).map((goal, i) => (
              <li key={i} className="flex items-start">
                <span className="mr-2">•</span>
                <span>{goal}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {persona.painPoints.length > 0 && (
        <div className="mb-3">
          <p className="text-xs font-medium text-gray-500 mb-1">Pain Points</p>
          <ul className="text-sm text-gray-700 space-y-1">
            {persona.painPoints.slice(0, 3).map((pain, i) => (
              <li key={i} className="flex items-start">
                <span className="mr-2">•</span>
                <span>{pain}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      <div>
        <p className="text-xs font-medium text-gray-500 mb-1">Technical Level</p>
        <div className="w-full bg-gray-200 rounded-full h-2">
          <div
            className="bg-blue-600 h-2 rounded-full"
            style={{ width: techLevelWidth[persona.technicalLevel] }}
          />
        </div>
      </div>
    </div>
  );
}
```

- [ ] **Step 2: Create PersonaEditor modal component**

```tsx
// src/Frontend/src/components/prd/PersonaEditor.tsx
import { useState, useEffect } from 'react';
import type { Persona, CreatePersonaRequest } from '../../api/personas';

interface PersonaEditorProps {
  persona?: Persona;
  onSave: (data: CreatePersonaRequest) => Promise<void>;
  onCancel: () => void;
}

export function PersonaEditor({ persona, onSave, onCancel }: PersonaEditorProps) {
  const [name, setName] = useState(persona?.name ?? '');
  const [role, setRole] = useState(persona?.role ?? '');
  const [technicalLevel, setTechnicalLevel] = useState<'Low' | 'Medium' | 'High'>(
    persona?.technicalLevel ?? 'Medium'
  );
  const [goals, setGoals] = useState<string[]>(persona?.goals ?? []);
  const [painPoints, setPainPoints] = useState<string[]>(persona?.painPoints ?? []);
  const [quote, setQuote] = useState(persona?.quote ?? '');
  const [newGoal, setNewGoal] = useState('');
  const [newPainPoint, setNewPainPoint] = useState('');
  const [saving, setSaving] = useState(false);

  const handleAddGoal = () => {
    if (newGoal.trim()) {
      setGoals([...goals, newGoal.trim()]);
      setNewGoal('');
    }
  };

  const handleAddPainPoint = () => {
    if (newPainPoint.trim()) {
      setPainPoints([...painPoints, newPainPoint.trim()]);
      setNewPainPoint('');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      await onSave({ name, role, technicalLevel, goals, painPoints, quote });
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg w-full max-w-2xl max-h-[90vh] overflow-y-auto p-6">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-semibold">
            {persona ? 'Edit Persona' : 'Create Persona'}
          </h2>
          <button onClick={onCancel} className="text-gray-500 hover:text-gray-700">
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Name</label>
              <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                placeholder="Sarah the Team Lead"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Role</label>
              <input
                type="text"
                value={role}
                onChange={(e) => setRole(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg"
                placeholder="Team Lead at mid-size company"
                required
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Technical Comfort</label>
            <div className="flex gap-4">
              {(['Low', 'Medium', 'High'] as const).map((level) => (
                <label key={level} className="flex items-center">
                  <input
                    type="radio"
                    name="technicalLevel"
                    value={level}
                    checked={technicalLevel === level}
                    onChange={() => setTechnicalLevel(level)}
                    className="mr-2"
                  />
                  {level}
                </label>
              ))}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Goals</label>
            <ul className="space-y-2 mb-2">
              {goals.map((goal, i) => (
                <li key={i} className="flex items-center justify-between bg-gray-50 px-3 py-2 rounded">
                  <span>{goal}</span>
                  <button
                    type="button"
                    onClick={() => setGoals(goals.filter((_, j) => j !== i))}
                    className="text-red-500 hover:text-red-700"
                  >
                    ×
                  </button>
                </li>
              ))}
            </ul>
            <div className="flex gap-2">
              <input
                type="text"
                value={newGoal}
                onChange={(e) => setNewGoal(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddGoal())}
                className="flex-1 px-3 py-2 border border-gray-300 rounded-lg"
                placeholder="Add a goal..."
              />
              <button
                type="button"
                onClick={handleAddGoal}
                className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200"
              >
                Add
              </button>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Pain Points</label>
            <ul className="space-y-2 mb-2">
              {painPoints.map((pain, i) => (
                <li key={i} className="flex items-center justify-between bg-gray-50 px-3 py-2 rounded">
                  <span>{pain}</span>
                  <button
                    type="button"
                    onClick={() => setPainPoints(painPoints.filter((_, j) => j !== i))}
                    className="text-red-500 hover:text-red-700"
                  >
                    ×
                  </button>
                </li>
              ))}
            </ul>
            <div className="flex gap-2">
              <input
                type="text"
                value={newPainPoint}
                onChange={(e) => setNewPainPoint(e.target.value)}
                onKeyPress={(e) => e.key === 'Enter' && (e.preventDefault(), handleAddPainPoint())}
                className="flex-1 px-3 py-2 border border-gray-300 rounded-lg"
                placeholder="Add a pain point..."
              />
              <button
                type="button"
                onClick={handleAddPainPoint}
                className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200"
              >
                Add
              </button>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Representative Quote
            </label>
            <textarea
              value={quote}
              onChange={(e) => setQuote(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg"
              rows={2}
              placeholder="I spend more time tracking tasks than actually helping my team succeed."
            />
          </div>

          <div className="flex justify-end gap-3 pt-4 border-t">
            <button
              type="button"
              onClick={onCancel}
              className="px-4 py-2 text-gray-700 border border-gray-300 rounded-lg hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving || !name.trim() || !role.trim()}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              {saving ? 'Saving...' : persona ? 'Save Persona' : 'Create Persona'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
```

- [ ] **Step 3: Create PersonasPage**

```tsx
// src/Frontend/src/pages/PersonasPage.tsx
import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchPersonas, createPersona, updatePersona, deletePersona } from '../store/slices/personasSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import { PersonaCard } from '../components/prd/PersonaCard';
import { PersonaEditor } from '../components/prd/PersonaEditor';
import type { Persona, CreatePersonaRequest } from '../api/personas';
import type { RootState } from '../store/store';

export function PersonasPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { currentProduct } = useAppSelector((state: RootState) => state.products);
  const { items: personas, loading } = useAppSelector((state: RootState) => state.personas);
  const [editorOpen, setEditorOpen] = useState(false);
  const [editingPersona, setEditingPersona] = useState<Persona | undefined>();

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchPersonas(id));
    }
  }, [dispatch, id]);

  const handleCreate = async (data: CreatePersonaRequest) => {
    if (!id) return;
    await dispatch(createPersona({ productId: id, data }));
    await dispatch(fetchPersonas(id));
    setEditorOpen(false);
  };

  const handleUpdate = async (data: CreatePersonaRequest) => {
    if (!id || !editingPersona) return;
    await dispatch(updatePersona({ productId: id, personaId: editingPersona.id, data }));
    await dispatch(fetchPersonas(id));
    setEditingPersona(undefined);
    setEditorOpen(false);
  };

  const handleDelete = async (personaId: string) => {
    if (!id || !confirm('Delete this persona?')) return;
    await dispatch(deletePersona({ productId: id, personaId }));
  };

  return (
    <div className="max-w-6xl mx-auto p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <button
            onClick={() => navigate(`/products/${id}`)}
            className="text-blue-600 hover:underline mb-2"
          >
            ← Back to {currentProduct?.name ?? 'Product'}
          </button>
          <h1 className="text-2xl font-bold text-gray-900">User Personas</h1>
        </div>
        <button
          onClick={() => { setEditingPersona(undefined); setEditorOpen(true); }}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
        >
          + Add Persona
        </button>
      </div>

      {loading ? (
        <div className="flex justify-center py-12">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : personas.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          <p className="mb-4">No personas yet. Create your first persona to get started.</p>
          <button
            onClick={() => setEditorOpen(true)}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Create Persona
          </button>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {personas.map((persona) => (
            <PersonaCard
              key={persona.id}
              persona={persona}
              onEdit={() => { setEditingPersona(persona); setEditorOpen(true); }}
              onDelete={() => handleDelete(persona.id)}
            />
          ))}
        </div>
      )}

      {editorOpen && (
        <PersonaEditor
          persona={editingPersona}
          onSave={editingPersona ? handleUpdate : handleCreate}
          onCancel={() => { setEditorOpen(false); setEditingPersona(undefined); }}
        />
      )}
    </div>
  );
}
```

- [ ] **Step 4: Add route to App.tsx**

Add import and route:
```tsx
import { PersonasPage } from './pages/PersonasPage';

// Add route inside ProtectedRoute:
<Route
  path="/products/:id/personas"
  element={
    <ProtectedRoute>
      <PersonasPage />
    </ProtectedRoute>
  }
/>
```

- [ ] **Step 5: Update ProductDetailPage navigation**

Add link to personas section in ProductDetailPage.tsx.

- [ ] **Step 6: Build and test**

Run: `cd src/Frontend && npm run build`
Test in browser: navigate to a product, click Personas section

- [ ] **Step 7: Commit**

```bash
git add src/Frontend/src/pages/PersonasPage.tsx src/Frontend/src/components/prd/ src/Frontend/src/App.tsx
git commit -m "feat: add Persona section page with card and editor modal"
```

---

### Task 7: User Stories Section Page

**Files:**
- Create: `src/Frontend/src/pages/StoriesPage.tsx`
- Create: `src/Frontend/src/components/prd/StoryCard.tsx`
- Create: `src/Frontend/src/components/prd/StoryEditor.tsx`
- Modify: `src/Frontend/src/App.tsx`

**Interfaces:**
- Consumes: `storiesSlice`, `personasSlice` (for persona linking dropdown)
- Produces: `/products/:id/stories` route with stories grouped by priority

Follow same pattern as Task 6 (PersonasPage), but:
- Group stories by MoSCoW priority (Must, Should, Could, Wont)
- Show linked persona name in story card
- Include persona dropdown in editor
- Show acceptance criteria with checkboxes in card

- [ ] **Step 1-6: Implement StoriesPage, StoryCard, StoryEditor**

Follow same structure as PersonasPage. Key differences:
- Group by priority sections
- Filter by persona dropdown
- Acceptance criteria list editor

- [ ] **Step 7: Commit**

```bash
git add src/Frontend/src/pages/StoriesPage.tsx src/Frontend/src/components/prd/StoryCard.tsx src/Frontend/src/components/prd/StoryEditor.tsx src/Frontend/src/App.tsx
git commit -m "feat: add User Stories section page with persona linking"
```

---

### Task 8: Features Board Page with Drag-Drop

**Files:**
- Create: `src/Frontend/src/pages/FeaturesPage.tsx`
- Create: `src/Frontend/src/components/prd/FeatureCard.tsx`
- Create: `src/Frontend/src/components/prd/FeatureEditor.tsx`
- Create: `src/Frontend/src/components/prd/FeatureBoard.tsx`
- Modify: `src/Frontend/src/App.tsx`

**Interfaces:**
- Consumes: `featuresSlice`
- Produces: `/products/:id/features` route with 3-column Kanban board (MVP, Enhanced, Future)

- [ ] **Step 1: Create FeatureCard**

```tsx
// src/Frontend/src/components/prd/FeatureCard.tsx
import type { Feature } from '../../api/features';

interface FeatureCardProps {
  feature: Feature;
  onEdit: () => void;
  onDelete: () => void;
  draggable?: boolean;
  onDragStart?: (e: React.DragEvent) => void;
}

export function FeatureCard({ feature, onEdit, onDelete, draggable, onDragStart }: FeatureCardProps) {
  const effortColors = {
    Small: 'bg-green-100 text-green-800',
    Medium: 'bg-yellow-100 text-yellow-800',
    Large: 'bg-red-100 text-red-800',
  };

  return (
    <div
      draggable={draggable}
      onDragStart={onDragStart}
      className="bg-white rounded-lg border border-gray-200 p-3 hover:shadow-md transition-shadow cursor-grab active:cursor-grabbing"
    >
      <div className="flex justify-between items-start mb-2">
        <h4 className="font-medium text-gray-900">{feature.name}</h4>
        <div className="flex gap-1">
          <button onClick={onEdit} className="text-blue-600 hover:text-blue-800 text-xs">Edit</button>
          <button onClick={onDelete} className="text-red-600 hover:text-red-800 text-xs">×</button>
        </div>
      </div>
      <p className="text-sm text-gray-600 mb-2 line-clamp-2">{feature.description}</p>
      <div className="flex items-center justify-between text-xs">
        <span className="text-gray-500">Stories: {feature.linkedStoryIds.length}</span>
        {feature.effort && (
          <span className={`px-2 py-0.5 rounded ${effortColors[feature.effort]}`}>
            {feature.effort}
          </span>
        )}
      </div>
    </div>
  );
}
```

- [ ] **Step 2: Create FeatureBoard with drag-drop**

```tsx
// src/Frontend/src/components/prd/FeatureBoard.tsx
import { useState } from 'react';
import type { Feature, FeaturePhase } from '../../api/features';
import { FeatureCard } from './FeatureCard';

interface FeatureBoardProps {
  features: Feature[];
  onMoveFeature: (featureId: string, phase: FeaturePhase) => void;
  onEditFeature: (feature: Feature) => void;
  onDeleteFeature: (featureId: string) => void;
  onAddFeature: (phase: FeaturePhase) => void;
}

const PHASES: { key: FeaturePhase; label: string }[] = [
  { key: 'Mvp', label: 'MVP (Phase 1)' },
  { key: 'Enhanced', label: 'Enhanced (Phase 2)' },
  { key: 'Future', label: 'Future (Phase 3)' },
];

export function FeatureBoard({
  features,
  onMoveFeature,
  onEditFeature,
  onDeleteFeature,
  onAddFeature,
}: FeatureBoardProps) {
  const [draggedId, setDraggedId] = useState<string | null>(null);
  const [dropTarget, setDropTarget] = useState<FeaturePhase | null>(null);

  const handleDragStart = (e: React.DragEvent, featureId: string) => {
    setDraggedId(featureId);
    e.dataTransfer.effectAllowed = 'move';
  };

  const handleDragOver = (e: React.DragEvent, phase: FeaturePhase) => {
    e.preventDefault();
    setDropTarget(phase);
  };

  const handleDrop = (e: React.DragEvent, phase: FeaturePhase) => {
    e.preventDefault();
    if (draggedId) {
      onMoveFeature(draggedId, phase);
    }
    setDraggedId(null);
    setDropTarget(null);
  };

  const handleDragEnd = () => {
    setDraggedId(null);
    setDropTarget(null);
  };

  return (
    <div className="grid grid-cols-3 gap-6">
      {PHASES.map(({ key, label }) => (
        <div
          key={key}
          onDragOver={(e) => handleDragOver(e, key)}
          onDrop={(e) => handleDrop(e, key)}
          onDragLeave={() => setDropTarget(null)}
          className={`bg-gray-50 rounded-lg p-4 min-h-[400px] ${
            dropTarget === key ? 'ring-2 ring-blue-400 bg-blue-50' : ''
          }`}
        >
          <div className="flex justify-between items-center mb-4">
            <h3 className="font-semibold text-gray-900">{label}</h3>
            <span className="text-sm text-gray-500">
              {features.filter((f) => f.phase === key).length}
            </span>
          </div>
          <div className="space-y-3">
            {features
              .filter((f) => f.phase === key)
              .sort((a, b) => a.sortOrder - b.sortOrder)
              .map((feature) => (
                <FeatureCard
                  key={feature.id}
                  feature={feature}
                  draggable
                  onDragStart={(e) => handleDragStart(e, feature.id)}
                  onEdit={() => onEditFeature(feature)}
                  onDelete={() => onDeleteFeature(feature.id)}
                />
              ))}
          </div>
          <button
            onClick={() => onAddFeature(key)}
            className="w-full mt-4 py-2 text-sm text-gray-500 border border-dashed border-gray-300 rounded-lg hover:bg-white hover:text-gray-700"
          >
            + Add Feature
          </button>
        </div>
      ))}
    </div>
  );
}
```

- [ ] **Step 3: Create FeatureEditor modal**

Similar to PersonaEditor but with phase selector and effort dropdown.

- [ ] **Step 4: Create FeaturesPage**

```tsx
// src/Frontend/src/pages/FeaturesPage.tsx
import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchFeatures, createFeature, updateFeature, deleteFeature, moveFeature } from '../store/slices/featuresSlice';
import { fetchProduct } from '../store/slices/productsSlice';
import { FeatureBoard } from '../components/prd/FeatureBoard';
import { FeatureEditor } from '../components/prd/FeatureEditor';
import type { Feature, FeaturePhase, CreateFeatureRequest } from '../api/features';
import type { RootState } from '../store/store';

export function FeaturesPage() {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { currentProduct } = useAppSelector((state: RootState) => state.products);
  const { items: features, loading } = useAppSelector((state: RootState) => state.features);
  const [editorOpen, setEditorOpen] = useState(false);
  const [editingFeature, setEditingFeature] = useState<Feature | undefined>();
  const [defaultPhase, setDefaultPhase] = useState<FeaturePhase>('Mvp');

  useEffect(() => {
    if (id) {
      dispatch(fetchProduct(id));
      dispatch(fetchFeatures(id));
    }
  }, [dispatch, id]);

  const handleCreate = async (data: CreateFeatureRequest) => {
    if (!id) return;
    await dispatch(createFeature({ productId: id, data: { ...data, phase: defaultPhase } }));
    await dispatch(fetchFeatures(id));
    setEditorOpen(false);
  };

  const handleUpdate = async (data: CreateFeatureRequest) => {
    if (!id || !editingFeature) return;
    await dispatch(updateFeature({ productId: id, featureId: editingFeature.id, data }));
    await dispatch(fetchFeatures(id));
    setEditingFeature(undefined);
    setEditorOpen(false);
  };

  const handleMove = async (featureId: string, phase: FeaturePhase) => {
    if (!id) return;
    await dispatch(moveFeature({ productId: id, featureId, phase }));
  };

  const handleDelete = async (featureId: string) => {
    if (!id || !confirm('Delete this feature?')) return;
    await dispatch(deleteFeature({ productId: id, featureId }));
  };

  const handleAddFeature = (phase: FeaturePhase) => {
    setDefaultPhase(phase);
    setEditingFeature(undefined);
    setEditorOpen(true);
  };

  return (
    <div className="max-w-7xl mx-auto p-8">
      <div className="flex items-center justify-between mb-6">
        <div>
          <button
            onClick={() => navigate(`/products/${id}`)}
            className="text-blue-600 hover:underline mb-2"
          >
            ← Back to {currentProduct?.name ?? 'Product'}
          </button>
          <h1 className="text-2xl font-bold text-gray-900">Features & Requirements</h1>
          <p className="text-gray-500 mt-1">Drag features between phases to reprioritize</p>
        </div>
      </div>

      {loading ? (
        <div className="flex justify-center py-12">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      ) : (
        <FeatureBoard
          features={features}
          onMoveFeature={handleMove}
          onEditFeature={(f) => { setEditingFeature(f); setEditorOpen(true); }}
          onDeleteFeature={handleDelete}
          onAddFeature={handleAddFeature}
        />
      )}

      {editorOpen && (
        <FeatureEditor
          feature={editingFeature}
          defaultPhase={defaultPhase}
          onSave={editingFeature ? handleUpdate : handleCreate}
          onCancel={() => { setEditorOpen(false); setEditingFeature(undefined); }}
        />
      )}
    </div>
  );
}
```

- [ ] **Step 5: Add route and navigation**

- [ ] **Step 6: Build and test drag-drop**

- [ ] **Step 7: Commit**

```bash
git add src/Frontend/src/pages/FeaturesPage.tsx src/Frontend/src/components/prd/FeatureCard.tsx src/Frontend/src/components/prd/FeatureBoard.tsx src/Frontend/src/components/prd/FeatureEditor.tsx src/Frontend/src/App.tsx
git commit -m "feat: add Features board with drag-drop between phases"
```

---

### Task 9: Update ProductDetailPage with Section Navigation

**Files:**
- Modify: `src/Frontend/src/pages/ProductDetailPage.tsx`

**Interfaces:**
- Consumes: `currentProduct` from `productsSlice`
- Produces: Clickable section cards that navigate to section pages

- [ ] **Step 1: Update ProductDetailPage to show clickable sections**

Add navigation links from the PRD Sections summary to the individual section pages:
- Click "Personas" → `/products/:id/personas`
- Click "User Stories" → `/products/:id/stories`
- Click "Features" → `/products/:id/features`

- [ ] **Step 2: Test navigation flow**

- [ ] **Step 3: Commit**

```bash
git add src/Frontend/src/pages/ProductDetailPage.tsx
git commit -m "feat: add section navigation from ProductDetailPage"
```

---

### Task 10: Integration Testing and Polish

**Files:**
- Various fixes as needed

- [ ] **Step 1: Start backend and frontend**

```bash
cd src/Backend && dotnet run &
cd src/Frontend && npm run dev &
```

- [ ] **Step 2: Test full flow**

1. Login
2. Create a product
3. Navigate to Personas → create/edit/delete personas
4. Navigate to User Stories → create stories linked to personas
5. Navigate to Features → create features, drag between phases
6. Verify all CRUD operations work

- [ ] **Step 3: Fix any issues found**

- [ ] **Step 4: Run full test suite**

```bash
cd src/Backend && dotnet test
cd src/Frontend && npm test
```

- [ ] **Step 5: Final commit**

```bash
git add -A
git commit -m "feat: Phase 2 complete - Section Editors for Personas, Stories, Features"
```

---

## Summary

Phase 2 delivers:
1. **Persona CRUD API** - Full REST endpoints with ownership validation
2. **User Story CRUD API** - With persona linking and MoSCoW priority
3. **Feature CRUD API** - With phase movement support
4. **Frontend API clients** - Type-safe API wrappers
5. **Redux state management** - Slices for all section data
6. **Persona section page** - Cards with edit modal
7. **User Stories section page** - Grouped by priority, persona filter
8. **Features board** - 3-column Kanban with drag-drop
9. **Product navigation** - Section links from detail page
10. **Integration testing** - Full flow validation
