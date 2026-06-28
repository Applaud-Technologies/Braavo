# Testing Phase Documentation

## 1. Testing Strategy

### 1.1 Testing Pyramid

```
                    ┌─────────────────────────────────────┐
                    │           E2E Tests                 │
                    │     (User Journey Testing)          │
                    │        10% of tests                 │
                    └─────────────────────────────────────┘
                            ┌─────────────────────────────────────┐
                            │         Integration Tests           │
                            │    (API & Service Testing)          │
                            │         20% of tests                │
                            └─────────────────────────────────────┘
                                    ┌─────────────────────────────────────┐
                                    │            Unit Tests               │
                                    │      (Component Testing)            │
                                    │         70% of tests                │
                                    └─────────────────────────────────────┘
```

### 1.2 Testing Types

#### Unit Testing
- **Purpose**: Test individual components/functions in isolation
- **Coverage**: 80% minimum code coverage
- **Tools**: xUnit, FluentAssertions, Moq (backend); Jest, React Testing Library (frontend)
- **Focus**: Business logic, CQRS command/query handlers, MediatR behaviors, API endpoints

#### Integration Testing
- **Purpose**: Test component interactions and API integrations
- **Coverage**: Critical user flows and service integrations
- **Tools**: xUnit, WebApplicationFactory, TestContainers (backend); Jest, Supertest (frontend)
- **Focus**: Database operations, external API calls, service communication

#### End-to-End Testing
- **Purpose**: Test complete user journeys
- **Coverage**: Major user workflows
- **Tools**: Cypress, Playwright
- **Focus**: User interactions, cross-browser compatibility

#### Performance Testing
- **Purpose**: Test system performance under load
- **Coverage**: API endpoints, database queries, frontend rendering
- **Tools**: Artillery, Lighthouse, WebPageTest
- **Focus**: Response times, throughput, resource utilization

#### Security Testing
- **Purpose**: Test for security vulnerabilities
- **Coverage**: Authentication, authorization, input validation
- **Tools**: OWASP ZAP, Snyk, npm audit
- **Focus**: SQL injection, XSS, CSRF, dependency vulnerabilities

#### Mutation Testing
- **Purpose**: Measure test suite quality by introducing code mutations
- **Coverage**: Critical business logic, Core layer, CQRS handlers
- **Tools**: Stryker.NET (backend), Stryker Mutator (frontend)
- **Focus**: Verify tests actually catch bugs, not just execute code
- **Target**: 80%+ mutation score on Core layer

## 2. Test Plan

### 2.1 Test Objectives

#### Primary Objectives
1. **Functional Correctness**: All features work as specified
2. **Performance**: System meets performance requirements
3. **Security**: System is secure against common vulnerabilities
4. **Usability**: Interface is intuitive and accessible
5. **Reliability**: System is stable under normal and peak loads

#### Success Criteria
- All critical tests pass
- Code coverage > 80%
- Mutation score > 80% (Core layer)
- No high-severity security issues
- Performance metrics meet requirements
- User acceptance testing approval

### 2.2 Test Scope

#### In Scope
- All user-facing features
- API endpoints and business logic
- Database operations
- Third-party integrations
- Security mechanisms
- Performance under normal load

#### Out of Scope
- Third-party service internal functionality
- Network infrastructure
- Operating system functionality
- Browser-specific bugs (unless affecting core functionality)

### 2.3 Test Environment

#### Development Environment
- **Purpose**: Developer testing during development
- **Database**: Local PostgreSQL with test data
- **Dependencies**: Mock services for external APIs
- **Configuration**: Development configuration

#### Staging Environment
- **Purpose**: Pre-production testing
- **Database**: Staging PostgreSQL with production-like data
- **Dependencies**: Staging versions of external services
- **Configuration**: Production-like configuration

#### Production Environment
- **Purpose**: Smoke testing after deployment
- **Database**: Production PostgreSQL
- **Dependencies**: Production external services
- **Configuration**: Production configuration

## 3. Test Cases

### 3.1 Authentication Test Cases

#### Test Case: User Registration
```gherkin
Feature: User Registration

Scenario: Successful user registration
  Given I am on the registration page
  When I enter valid user details
    | Field    | Value                |
    | Name     | John Doe             |
    | Email    | john@example.com     |
    | Password | StrongPassword123    |
  And I click the "Register" button
  Then I should see a success message
  And I should receive a confirmation email
  And I should be redirected to the dashboard

Scenario: Registration with existing email
  Given I am on the registration page
  When I enter an email that already exists
  And I click the "Register" button
  Then I should see an error message "Email already exists"
  And I should remain on the registration page

Scenario: Registration with weak password
  Given I am on the registration page
  When I enter a weak password
  And I click the "Register" button
  Then I should see an error message "Password must be at least 8 characters"
  And I should remain on the registration page
```

#### Test Case: User Login
```gherkin
Feature: User Login

Scenario: Successful login
  Given I have a registered account
  And I am on the login page
  When I enter valid credentials
  And I click the "Login" button
  Then I should be redirected to the dashboard
  And I should see my profile information

Scenario: Login with invalid credentials
  Given I am on the login page
  When I enter invalid credentials
  And I click the "Login" button
  Then I should see an error message "Invalid credentials"
  And I should remain on the login page

Scenario: Login with expired session
  Given I have an expired session
  When I try to access a protected page
  Then I should be redirected to the login page
  And I should see a message "Session expired"
```

### 3.2 PRD Generation Test Cases

#### Test Case: Basic PRD Generation
```gherkin
Feature: PRD Generation

Scenario: Generate PRD from simple idea
  Given I am logged in as a product manager
  And I am on the PRD generation page
  When I enter a product idea: "A mobile app for tracking daily water intake"
  And I click "Generate PRD"
  Then I should see a structured PRD with the following sections:
    | Section                | Present |
    | Product Overview       | Yes     |
    | Problem Statement      | Yes     |
    | Solution Overview      | Yes     |
    | User Stories           | Yes     |
    | Functional Requirements| Yes     |
    | Success Metrics        | Yes     |
  And the PRD should be editable
  And I should be able to save the PRD

Scenario: Generate PRD with specific template
  Given I am logged in as a product manager
  And I am on the PRD generation page
  When I enter a product idea
  And I select "SaaS Application" template
  And I click "Generate PRD"
  Then the PRD should include SaaS-specific sections
  And the PRD should include relevant metrics for SaaS products
```

### 3.3 Design Generation Test Cases

#### Test Case: Wireframe Generation
```gherkin
Feature: Wireframe Generation

Scenario: Generate wireframe from user story
  Given I am logged in as a designer
  And I have a PRD with user stories
  When I select a user story: "As a user, I want to create an account"
  And I click "Generate Wireframe"
  Then I should see a wireframe with:
    | Element           | Present |
    | Registration form | Yes     |
    | Input fields      | Yes     |
    | Submit button     | Yes     |
    | Validation messages| Yes    |
  And the wireframe should be editable
  And I should be able to export to Figma

Scenario: Apply design system to wireframe
  Given I have generated a wireframe
  And I have a design system configured
  When I click "Apply Design System"
  Then the wireframe should use the design system colors
  And the wireframe should use the design system typography
  And the wireframe should use the design system spacing
```

### 3.4 Code Generation Test Cases

#### Test Case: React Component Generation
```gherkin
Feature: Code Generation

Scenario: Generate React component from wireframe
  Given I am logged in as a developer
  And I have a completed wireframe
  When I click "Generate Code"
  And I select "React" as the framework
  And I select "TypeScript" as the language
  Then I should receive generated code with:
    | Component          | Present |
    | Component file     | Yes     |
    | Props interface    | Yes     |
    | CSS/styled-components | Yes  |
    | Basic functionality| Yes     |
  And the code should be syntactically valid
  And the code should follow React best practices

Scenario: Generate API endpoints from requirements
  Given I have a PRD with data requirements
  When I click "Generate API"
  And I select "ASP.NET Core FastEndpoints" as the framework
  Then I should receive:
    | File                | Present |
    | Endpoint classes    | Yes     |
    | Request/response DTOs | Yes   |
    | EF Core entities    | Yes     |
    | FluentValidation validators | Yes |
  And the API should follow RESTful conventions
```

### 3.5 Collaboration Test Cases

#### Test Case: Real-time Collaboration
```gherkin
Feature: Real-time Collaboration

Scenario: Multiple users editing same document
  Given User A and User B are both logged in
  And they are viewing the same PRD
  When User A makes an edit to the title
  And User B makes an edit to the description
  Then User A should see User B's changes in real-time
  And User B should see User A's changes in real-time
  And both changes should be saved correctly
  And the version history should record both changes

Scenario: Commenting on documents
  Given I am viewing a shared PRD
  When I select text and add a comment
  Then the comment should be visible to other team members
  And other team members should receive notifications
  And the comment should be linked to the specific text
```

## 4. Automated Test Implementation

### 4.1 Unit Test Examples

#### Frontend Component Testing
```typescript
// src/components/Button.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { Button } from './Button';

describe('Button Component', () => {
  it('renders button with correct text', () => {
    render(<Button>Click me</Button>);
    expect(screen.getByText('Click me')).toBeInTheDocument();
  });

  it('handles click events', () => {
    const handleClick = jest.fn();
    render(<Button onClick={handleClick}>Click me</Button>);

    fireEvent.click(screen.getByText('Click me'));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('shows loading state', () => {
    render(<Button isLoading>Loading</Button>);
    expect(screen.getByText('Loading')).toBeInTheDocument();
    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('applies correct variant classes', () => {
    render(<Button variant="primary">Primary</Button>);
    expect(screen.getByRole('button')).toHaveClass('bg-blue-600');
  });
});
```

#### Backend CQRS Testing with MediatR
```csharp
// Braavo.Tests/Features/Documents/CreateDocumentCommandHandlerTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using FluentValidation;
using Braavo.Core.Features.Documents.Commands;
using Braavo.Core.Features.Documents.Handlers;
using Braavo.Core.Features.Documents.Validators;
using Braavo.Core.Interfaces;
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Braavo.Core.Exceptions;
using Braavo.Shared.DTOs;

namespace Braavo.Tests.Features.Documents
{
    public class CreateDocumentCommandHandlerTests
    {
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IValidator<CreateDocumentCommand> _validator;
        private readonly CreateDocumentCommandHandler _handler;

        public CreateDocumentCommandHandlerTests()
        {
            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _validator = new CreateDocumentCommandValidator();

            _handler = new CreateDocumentCommandHandler(
                _documentRepositoryMock.Object,
                _projectRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _validator
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsDocumentDto()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var command = new CreateDocumentCommand
            {
                Title = "Test Document",
                Content = "Test content",
                Type = "PRD",
                ProjectId = projectId,
                UserId = userId
            };

            var project = new Project("Test Project", userId);
            var document = Document.Create(command.Title, command.Content, command.Type, projectId, userId);
            var documentDto = new DocumentDto { Id = document.Id, Title = command.Title };

            _projectRepositoryMock.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync(project);
            _mapperMock.Setup(x => x.Map<DocumentDto>(It.IsAny<Document>()))
                .Returns(documentDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(command.Title);
            _documentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Document>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ThrowsValidationException()
        {
            // Arrange
            var command = new CreateDocumentCommand
            {
                Title = "", // Invalid - empty title
                Content = "Test content",
                Type = "PRD",
                ProjectId = Guid.NewGuid(),
                UserId = new UserId(Guid.NewGuid())
            };

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_ProjectNotFound_ThrowsProjectNotFoundException()
        {
            // Arrange
            var command = new CreateDocumentCommand
            {
                Title = "Test Document",
                Content = "Test content",
                Type = "PRD",
                ProjectId = Guid.NewGuid(),
                UserId = new UserId(Guid.NewGuid())
            };

            _projectRepositoryMock.Setup(x => x.GetByIdAsync(command.ProjectId))
                .ReturnsAsync((Project)null);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<ProjectNotFoundException>();
        }

        [Fact]
        public async Task Handle_UnauthorizedAccess_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var otherUserId = new UserId(Guid.NewGuid());
            var command = new CreateDocumentCommand
            {
                Title = "Test Document",
                Content = "Test content",
                Type = "PRD",
                ProjectId = Guid.NewGuid(),
                UserId = userId
            };

            var project = new Project("Test Project", otherUserId); // Different user owns project
            _projectRepositoryMock.Setup(x => x.GetByIdAsync(command.ProjectId))
                .ReturnsAsync(project);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}

// Braavo.Tests/Features/Documents/GetDocumentsQueryHandlerTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using AutoMapper;
using Braavo.Core.Features.Documents.Queries;
using Braavo.Core.Features.Documents.Handlers;
using Braavo.Core.Interfaces;
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Braavo.Shared.DTOs;

namespace Braavo.Tests.Features.Documents
{
    public class GetDocumentsQueryHandlerTests
    {
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetDocumentsQueryHandler _handler;

        public GetDocumentsQueryHandlerTests()
        {
            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetDocumentsQueryHandler(_documentRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_ReturnsPaginatedResult()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var query = new GetDocumentsQuery
            {
                UserId = userId,
                Limit = 10,
                Offset = 0
            };

            var documents = new List<Document>
            {
                Document.Create("Doc 1", "Content 1", "PRD", Guid.NewGuid(), userId),
                Document.Create("Doc 2", "Content 2", "TechnicalSpec", Guid.NewGuid(), userId)
            };

            var documentDtos = new List<DocumentDto>
            {
                new DocumentDto { Id = documents[0].Id, Title = "Doc 1" },
                new DocumentDto { Id = documents[1].Id, Title = "Doc 2" }
            };

            _documentRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, 0, 10))
                .ReturnsAsync(documents);
            _documentRepositoryMock.Setup(x => x.GetCountByUserIdAsync(userId))
                .ReturnsAsync(2);
            _mapperMock.Setup(x => x.Map<List<DocumentDto>>(documents))
                .Returns(documentDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.Limit.Should().Be(10);
            result.Offset.Should().Be(0);
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var query = new GetDocumentsQuery
            {
                UserId = userId,
                Limit = 10,
                Offset = 0
            };

            _documentRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, 0, 10))
                .ReturnsAsync(new List<Document>());
            _documentRepositoryMock.Setup(x => x.GetCountByUserIdAsync(userId))
                .ReturnsAsync(0);
            _mapperMock.Setup(x => x.Map<List<DocumentDto>>(It.IsAny<List<Document>>()))
                .Returns(new List<DocumentDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }
    }
}

// Braavo.Tests/Endpoints/DocumentsEndpointTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Braavo.Api.Endpoints.Documents;
using Braavo.Core.Features.Documents.Commands;
using Braavo.Core.Features.Documents.Queries;
using Braavo.Core.ValueObjects;
using Braavo.Shared.DTOs;
using FluentValidation;

namespace Braavo.Tests.Endpoints
{
    public class DocumentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DocumentsController _controller;

        public DocumentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new DocumentsController(_mediatorMock.Object);

            // Setup user context
            var userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", userId.ToString())
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetDocuments_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var expectedResult = new PaginatedResult<DocumentDto>
            {
                Items = new List<DocumentDto>(),
                TotalCount = 0,
                Limit = 20,
                Offset = 0
            };

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetDocumentsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetDocuments(null, null, 20, 0);

            // Assert
            result.Should().BeOfType<ActionResult<ApiResponse<PaginatedResult<DocumentDto>>>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CreateDocument_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new CreateDocumentRequest
            {
                Title = "Test Document",
                Content = "Test content",
                Type = "PRD",
                ProjectId = Guid.NewGuid()
            };

            var expectedResult = new DocumentDto
            {
                Id = Guid.NewGuid(),
                Title = request.Title
            };

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateDocumentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateDocument(request);

            // Assert
            result.Should().BeOfType<ActionResult<ApiResponse<DocumentDto>>>();
            var createdResult = result.Result as ObjectResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CreateDocument_ValidationError_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateDocumentRequest
            {
                Title = "Test Document",
                Content = "Test content",
                Type = "PRD",
                ProjectId = Guid.NewGuid()
            };

            var validationException = new ValidationException("Validation failed");
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateDocumentCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(validationException);

            // Act
            var result = await _controller.CreateDocument(request);

            // Assert
            result.Should().BeOfType<ActionResult<ApiResponse<DocumentDto>>>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
        }
    }
}

// Braavo.Tests/Behaviors/ValidationBehaviorTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Braavo.Core.Behaviors;
using Braavo.Core.Features.Documents.Commands;
using Braavo.Core.ValueObjects;

namespace Braavo.Tests.Behaviors
{
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_ValidRequest_CallsNext()
        {
            // Arrange
            var validators = new List<IValidator<CreateDocumentCommand>>();
            var behavior = new ValidationBehavior<CreateDocumentCommand, DocumentDto>(validators);
            var request = new CreateDocumentCommand
            {
                Title = "Valid Title",
                Content = "Valid Content",
                Type = "PRD",
                ProjectId = Guid.NewGuid(),
                UserId = new UserId(Guid.NewGuid())
            };

            var mockNext = new Mock<RequestHandlerDelegate<DocumentDto>>();
            var expectedResponse = new DocumentDto();
            mockNext.Setup(x => x()).ReturnsAsync(expectedResponse);

            // Act
            var result = await behavior.Handle(request, mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResponse);
            mockNext.Verify(x => x(), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ThrowsValidationException()
        {
            // Arrange
            var validator = new Mock<IValidator<CreateDocumentCommand>>();
            var validationFailure = new ValidationFailure("Title", "Title is required");
            var validationResult = new ValidationResult(new[] { validationFailure });

            validator.Setup(x => x.ValidateAsync(It.IsAny<CreateDocumentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var validators = new List<IValidator<CreateDocumentCommand>> { validator.Object };
            var behavior = new ValidationBehavior<CreateDocumentCommand, DocumentDto>(validators);
            var request = new CreateDocumentCommand();

            var mockNext = new Mock<RequestHandlerDelegate<DocumentDto>>();

            // Act & Assert
            await behavior.Invoking(x => x.Handle(request, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>();

            mockNext.Verify(x => x(), Times.Never);
        }
    }
}
```

### 4.2 Integration Test Examples

#### AI Service Integration Testing
```typescript
// src/services/aiService.test.ts
import { aiService } from './aiService';
import { PRDGenerationInput } from '../types/ai';

describe('AI Service Integration', () => {
  describe('generatePRD', () => {
    it('generates PRD from simple idea', async () => {
      const input: PRDGenerationInput = {
        idea: 'A mobile app for tracking daily water intake',
        targetAudience: 'Health-conscious individuals',
        template: 'Mobile App'
      };

      const result = await aiService.generatePRD(input);

      expect(result).toHaveProperty('title');
      expect(result).toHaveProperty('sections');
      expect(result.sections).toContain('Product Overview');
      expect(result.sections).toContain('User Stories');
      expect(result.sections).toContain('Functional Requirements');
    }, 30000); // Increase timeout for AI API calls

    it('handles API errors gracefully', async () => {
      const input: PRDGenerationInput = {
        idea: '', // Empty idea should cause error
        targetAudience: 'Users',
        template: 'Standard'
      };

      await expect(aiService.generatePRD(input)).rejects.toThrow();
    });
  });
});
```

### 4.3 End-to-End Test Examples

#### Cypress E2E Tests
```typescript
// cypress/e2e/prd-generation.cy.ts
describe('PRD Generation Flow', () => {
  beforeEach(() => {
    cy.login('test@example.com', 'password');
  });

  it('creates PRD from idea to export', () => {
    // Navigate to PRD generation
    cy.visit('/dashboard');
    cy.get('[data-testid="new-prd-button"]').click();

    // Enter product idea
    cy.get('[data-testid="idea-input"]').type('A mobile app for tracking daily water intake');
    cy.get('[data-testid="generate-button"]').click();

    // Wait for PRD generation
    cy.get('[data-testid="loading-indicator"]').should('be.visible');
    cy.get('[data-testid="prd-content"]', { timeout: 30000 }).should('be.visible');

    // Verify PRD content
    cy.get('[data-testid="prd-title"]').should('contain', 'Water Tracking App');
    cy.get('[data-testid="prd-sections"]').should('contain', 'Product Overview');
    cy.get('[data-testid="prd-sections"]').should('contain', 'User Stories');

    // Edit PRD
    cy.get('[data-testid="edit-button"]').click();
    cy.get('[data-testid="title-input"]').clear().type('Hydration Tracker App');
    cy.get('[data-testid="save-button"]').click();

    // Generate wireframe
    cy.get('[data-testid="generate-wireframe-button"]').click();
    cy.get('[data-testid="wireframe-canvas"]', { timeout: 30000 }).should('be.visible');

    // Export PRD
    cy.get('[data-testid="export-button"]').click();
    cy.get('[data-testid="export-pdf"]').click();
    cy.get('[data-testid="download-link"]').should('be.visible');
  });

  it('handles collaboration features', () => {
    // Open shared PRD
    cy.visit('/documents/shared-prd-id');

    // Add comment
    cy.get('[data-testid="prd-content"]').first().dblclick();
    cy.get('[data-testid="comment-input"]').type('This section needs more detail');
    cy.get('[data-testid="add-comment-button"]').click();

    // Verify comment appears
    cy.get('[data-testid="comment-indicator"]').should('be.visible');
    cy.get('[data-testid="comment-text"]').should('contain', 'This section needs more detail');

    // Make edit
    cy.get('[data-testid="edit-button"]').click();
    cy.get('[data-testid="content-editor"]').type('Additional requirements...');
    cy.get('[data-testid="save-button"]').click();

    // Verify version history
    cy.get('[data-testid="version-history"]').click();
    cy.get('[data-testid="version-list"]').should('contain', 'Version 2');
  });
});
```

## 5. Performance Testing

### 5.1 Load Testing Configuration
```javascript
// artillery/load-test.yml
config:
  target: 'http://localhost:3001'
  phases:
    - duration: 60
      arrivalRate: 10
      name: "Warm up"
    - duration: 120
      arrivalRate: 50
      name: "Sustained load"
    - duration: 60
      arrivalRate: 100
      name: "Peak load"
  variables:
    auth_token: "{{ $randomString() }}"

scenarios:
  - name: "API Load Test"
    weight: 100
    flow:
      - post:
          url: "/api/auth/login"
          json:
            email: "test@example.com"
            password: "password"
          capture:
            - json: "$.token"
              as: "auth_token"
      - get:
          url: "/api/documents"
          headers:
            Authorization: "Bearer {{ auth_token }}"
      - post:
          url: "/api/ai/generate-prd"
          headers:
            Authorization: "Bearer {{ auth_token }}"
          json:
            idea: "Test product idea"
            template: "Standard"
```

### 5.2 Performance Metrics
```typescript
// src/utils/performanceMonitor.ts
export class PerformanceMonitor {
  private metrics: Map<string, number[]> = new Map();

  startTimer(operation: string): () => void {
    const start = performance.now();
    return () => {
      const end = performance.now();
      const duration = end - start;
      this.recordMetric(operation, duration);
    };
  }

  recordMetric(operation: string, value: number): void {
    if (!this.metrics.has(operation)) {
      this.metrics.set(operation, []);
    }
    this.metrics.get(operation)!.push(value);
  }

  getStats(operation: string): {
    avg: number;
    min: number;
    max: number;
    p95: number;
    count: number;
  } {
    const values = this.metrics.get(operation) || [];
    if (values.length === 0) {
      return { avg: 0, min: 0, max: 0, p95: 0, count: 0 };
    }

    const sorted = values.sort((a, b) => a - b);
    const avg = values.reduce((sum, val) => sum + val, 0) / values.length;
    const min = sorted[0];
    const max = sorted[sorted.length - 1];
    const p95Index = Math.floor(sorted.length * 0.95);
    const p95 = sorted[p95Index];

    return { avg, min, max, p95, count: values.length };
  }
}
```

## 6. C# Backend Testing Examples

### 6.1 Clean Architecture Unit Testing
```csharp
// Tests/Core/UseCases/CreateDocumentUseCaseTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using Braavo.Core.UseCases.Documents;
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using Braavo.Core.Exceptions;
using Braavo.Shared.Enums;

namespace Braavo.Tests.Core.UseCases
{
    public class CreateDocumentUseCaseTests
    {
        private readonly Mock<IDocumentRepository> _mockDocumentRepository;
        private readonly Mock<IProjectRepository> _mockProjectRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateDocumentUseCase _useCase;

        public CreateDocumentUseCaseTests()
        {
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _mockProjectRepository = new Mock<IProjectRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _useCase = new CreateDocumentUseCase(
                _mockDocumentRepository.Object,
                _mockProjectRepository.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenProjectExists_ShouldCreateDocument()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var project = new Project("Test Project", userId);

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync(project);

            // Act
            var result = await _useCase.ExecuteAsync(
                "Test Document",
                "Test Content",
                DocumentType.PRD,
                projectId,
                userId);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Document");
            result.Content.Should().Be("Test Content");
            result.Type.Should().Be(DocumentType.PRD);
            result.ProjectId.Should().Be(projectId);
            result.CreatedBy.Should().Be(userId);

            _mockDocumentRepository.Verify(x => x.AddAsync(It.IsAny<Document>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenProjectDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync((Project?)null);

            // Act & Assert
            await _useCase.Invoking(x => x.ExecuteAsync(
                "Test Document",
                "Test Content",
                DocumentType.PRD,
                projectId,
                userId))
                .Should()
                .ThrowAsync<ProjectNotFoundException>()
                .WithMessage($"Project with ID {projectId} was not found");

            _mockDocumentRepository.Verify(x => x.AddAsync(It.IsAny<Document>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenUserHasNoAccess_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var otherUserId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var project = new Project("Test Project", otherUserId);

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync(project);

            // Act & Assert
            await _useCase.Invoking(x => x.ExecuteAsync(
                "Test Document",
                "Test Content",
                DocumentType.PRD,
                projectId,
                userId))
                .Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User does not have access to this project");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ExecuteAsync_WhenTitleIsInvalid_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var project = new Project("Test Project", userId);

            _mockProjectRepository.Setup(x => x.GetByIdAsync(projectId))
                .ReturnsAsync(project);

            // Act & Assert
            await _useCase.Invoking(x => x.ExecuteAsync(
                invalidTitle,
                "Test Content",
                DocumentType.PRD,
                projectId,
                userId))
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Title cannot be empty*");
        }
    }
}
```

### 6.2 Integration Testing with TestContainers
```csharp
// Tests/Integration/DocumentsControllerIntegrationTests.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Testcontainers.PostgreSql;
using Braavo.Infrastructure.Data;
using Braavo.Shared.DTOs;
using Braavo.Shared.Enums;
using System.Net;

namespace Braavo.Tests.Integration
{
    public class DocumentsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly PostgreSqlContainer _postgres;

        public DocumentsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _postgres = new PostgreSqlBuilder()
                .WithImage("postgres:15")
                .WithDatabase("braavo_test")
                .WithUsername("postgres")
                .WithPassword("test123")
                .Build();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database context
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BraavoDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add test database context
                    services.AddDbContext<BraavoDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));
                });
            });

            _client = _factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            // Run migrations
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BraavoDbContext>();
            await context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgres.DisposeAsync();
            _client.Dispose();
        }

        [Fact]
        public async Task GetDocuments_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/documents");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateDocument_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new CreateDocumentRequest
            {
                Title = "Integration Test Document",
                Content = "This is a test document created during integration testing",
                Type = DocumentType.PRD,
                ProjectId = await CreateTestProjectAsync()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/documents", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<DocumentDto>>();
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be("Integration Test Document");
            result.Data.Type.Should().Be(DocumentType.PRD);
        }

        [Fact]
        public async Task CreateDocument_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new CreateDocumentRequest
            {
                Title = "", // Invalid empty title
                Content = "Test content",
                Type = DocumentType.PRD,
                ProjectId = Guid.NewGuid()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/documents", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            result.Should().NotBeNull();
            result!.Success.Should().BeFalse();
            result.Error.Should().NotBeEmpty();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            // Create a test user and return JWT token
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
            return result!.Data!.Token;
        }

        private async Task<Guid> CreateTestProjectAsync()
        {
            var projectRequest = new CreateProjectRequest
            {
                Name = "Test Project",
                Description = "Test project for integration testing"
            };

            var response = await _client.PostAsJsonAsync("/api/projects", projectRequest);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProjectDto>>();
            return result!.Data!.Id;
        }
    }
}
```

### 6.3 Repository Testing with TestContainers
```csharp
// Tests/Infrastructure/Repositories/DocumentRepositoryTests.cs
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Testcontainers.PostgreSql;
using Braavo.Infrastructure.Data;
using Braavo.Infrastructure.Repositories;
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Braavo.Shared.Enums;

namespace Braavo.Tests.Infrastructure.Repositories
{
    public class DocumentRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres;
        private BraavoDbContext _context = null!;
        private DocumentRepository _repository = null!;

        public DocumentRepositoryTests()
        {
            _postgres = new PostgreSqlBuilder()
                .WithImage("postgres:15")
                .WithDatabase("braavo_test")
                .WithUsername("postgres")
                .WithPassword("test123")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            var options = new DbContextOptionsBuilder<BraavoDbContext>()
                .UseNpgsql(_postgres.GetConnectionString())
                .Options;

            _context = new BraavoDbContext(options);
            await _context.Database.MigrateAsync();

            _repository = new DocumentRepository(_context);
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
            await _postgres.DisposeAsync();
        }

        [Fact]
        public async Task AddAsync_ShouldAddDocumentToDatabase()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var document = new Document("Test Document", "Test Content", DocumentType.PRD, projectId, userId);

            // Act
            await _repository.AddAsync(document);
            await _context.SaveChangesAsync();

            // Assert
            var savedDocument = await _context.Documents.FirstOrDefaultAsync(d => d.Id == document.Id);
            savedDocument.Should().NotBeNull();
            savedDocument!.Title.Should().Be("Test Document");
            savedDocument.Content.Should().Be("Test Content");
            savedDocument.Type.Should().Be(DocumentType.PRD);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDocumentExists_ShouldReturnDocument()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var document = new Document("Test Document", "Test Content", DocumentType.PRD, projectId, userId);

            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(document.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(document.Id);
            result.Title.Should().Be("Test Document");
        }

        [Fact]
        public async Task GetByIdAsync_WhenDocumentDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Update_ShouldUpdateDocumentInDatabase()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var document = new Document("Original Title", "Original Content", DocumentType.PRD, projectId, userId);

            await _context.Documents.AddAsync(document);
            await _context.SaveChangesAsync();

            // Act
            document.UpdateTitle("Updated Title");
            document.UpdateContent("Updated Content");
            _repository.Update(document);
            await _context.SaveChangesAsync();

            // Assert
            var updatedDocument = await _context.Documents.FirstOrDefaultAsync(d => d.Id == document.Id);
            updatedDocument.Should().NotBeNull();
            updatedDocument!.Title.Should().Be("Updated Title");
            updatedDocument.Content.Should().Be("Updated Content");
            updatedDocument.Version.Should().Be(2); // Version should be incremented
        }
    }
}
```

### 6.4 Mutation Testing with Stryker
```json
// stryker-config.json
{
  "stryker-config": {
    "project": "csharp",
    "solution": "Braavo.sln",
    "test-projects": [
      "Tests/Braavo.Tests.csproj"
    ],
    "reporters": [
      "progress",
      "html",
      "json"
    ],
    "html-reporter": {
      "baseline-enabled": true
    },
    "thresholds": {
      "high": 90,
      "low": 70,
      "break": 60
    },
    "mutation-level": "Complete",
    "concurrency": 4,
    "ignore-mutations": [
      "string"
    ],
    "exclude-mutations": [
      "**/Migrations/**",
      "**/Program.cs",
      "**/Startup.cs"
    ],
    "ignore-methods": [
      "*ToString*",
      "*GetHashCode*",
      "*Equals*"
    ]
  }
}
```

```bash
# Run Stryker mutation testing
dotnet stryker --config-file stryker-config.json

# Run with specific mutation score threshold
dotnet stryker --threshold-high 85 --threshold-low 70 --threshold-break 60

# Generate baseline for comparison
dotnet stryker --reporter html --reporter baseline
```

### 6.5 Performance Testing with BenchmarkDotNet
```csharp
// Tests/Performance/DocumentServiceBenchmarks.cs
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Braavo.Infrastructure.Data;
using Braavo.Infrastructure.Repositories;
using Braavo.Core.Entities;
using Braavo.Core.ValueObjects;
using Braavo.Shared.Enums;

namespace Braavo.Tests.Performance
{
    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 3, targetCount: 10)]
    public class DocumentServiceBenchmarks
    {
        private BraavoDbContext _context = null!;
        private DocumentRepository _repository = null!;
        private List<Document> _testDocuments = null!;

        [GlobalSetup]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BraavoDbContext>()
                .UseInMemoryDatabase(databaseName: "BenchmarkDb")
                .Options;

            _context = new BraavoDbContext(options);
            _repository = new DocumentRepository(_context);

            // Seed test data
            _testDocuments = GenerateTestDocuments(1000);
            _context.Documents.AddRange(_testDocuments);
            _context.SaveChanges();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [Benchmark]
        public async Task GetDocumentById()
        {
            var randomDocument = _testDocuments[Random.Shared.Next(_testDocuments.Count)];
            var result = await _repository.GetByIdAsync(randomDocument.Id);
        }

        [Benchmark]
        public async Task GetDocumentsByProjectId()
        {
            var randomDocument = _testDocuments[Random.Shared.Next(_testDocuments.Count)];
            var result = await _repository.GetByProjectIdAsync(randomDocument.ProjectId);
        }

        [Benchmark]
        public async Task AddDocument()
        {
            var userId = new UserId(Guid.NewGuid());
            var projectId = Guid.NewGuid();
            var document = new Document("Benchmark Document", "Benchmark Content", DocumentType.PRD, projectId, userId);

            await _repository.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        private List<Document> GenerateTestDocuments(int count)
        {
            var documents = new List<Document>();
            var userId = new UserId(Guid.NewGuid());

            for (int i = 0; i < count; i++)
            {
                var projectId = Guid.NewGuid();
                var document = new Document($"Test Document {i}", $"Content for document {i}", DocumentType.PRD, projectId, userId);
                documents.Add(document);
            }

            return documents;
        }
    }
}
```

### 6.6 Test Configuration and CI/CD Integration
```yaml
# .github/workflows/test.yml
name: Test Suite

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: test123
          POSTGRES_DB: braavo_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Run Unit Tests
      run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"

    - name: Run Integration Tests
      run: dotnet test Tests/Integration --no-build --verbosity normal
      env:
        ConnectionStrings__DefaultConnection: "Host=localhost;Database=braavo_test;Username=postgres;Password=test123"

    - name: Run Mutation Tests
      run: |
        dotnet tool install --global dotnet-stryker
        dotnet stryker --config-file stryker-config.json --reporter json

    - name: Upload Coverage Reports
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage.xml
        flags: unittests
        name: codecov-umbrella

    - name: Upload Mutation Test Results
      uses: actions/upload-artifact@v3
      with:
        name: mutation-test-results
        path: StrykerOutput/
```

## 7. Security Testing

### 7.1 Security Test Cases
```typescript
// src/security/security.test.ts
describe('Security Tests', () => {
  describe('Authentication', () => {
    it('prevents SQL injection in login', async () => {
      const maliciousInput = {
        email: "'; DROP TABLE users; --",
        password: "password"
      };

      const response = await request(app)
        .post('/api/auth/login')
        .send(maliciousInput)
        .expect(400);

      expect(response.body.success).toBe(false);
      // Verify users table still exists
      const users = await prisma.user.findMany();
      expect(users).toBeDefined();
    });

    it('prevents XSS in user input', async () => {
      const maliciousInput = {
        name: '<script>alert("XSS")</script>',
        email: 'test@example.com',
        password: 'password'
      };

      const response = await request(app)
        .post('/api/auth/register')
        .send(maliciousInput)
        .expect(400);

      expect(response.body.success).toBe(false);
      expect(response.body.error).toContain('Invalid input');
    });

    it('enforces rate limiting', async () => {
      const promises = [];
      for (let i = 0; i < 20; i++) {
        promises.push(
          request(app)
            .post('/api/auth/login')
            .send({ email: 'test@example.com', password: 'wrong' })
        );
      }

      const responses = await Promise.all(promises);
      const rateLimitedResponses = responses.filter(r => r.status === 429);
      expect(rateLimitedResponses.length).toBeGreaterThan(0);
    });
  });

  describe('Authorization', () => {
    it('prevents unauthorized access to documents', async () => {
      const user1 = await createTestUser('user1@example.com');
      const user2 = await createTestUser('user2@example.com');

      const document = await createTestDocument(user1.id);

      const response = await request(app)
        .get(`/api/documents/${document.id}`)
        .set('Authorization', `Bearer ${generateTestToken(user2.id)}`)
        .expect(404);

      expect(response.body.success).toBe(false);
    });
  });
});
```

## 7. Test Reporting

### 7.1 Test Report Template
```markdown
# Test Report - Braavo

## Test Summary
- **Test Period**: [Date Range]
- **Environment**: [Staging/Production]
- **Total Test Cases**: [Number]
- **Passed**: [Number]
- **Failed**: [Number]
- **Skipped**: [Number]
- **Code Coverage**: [Percentage]

## Test Results by Feature

### Authentication
- **Total Tests**: 25
- **Passed**: 24
- **Failed**: 1
- **Coverage**: 85%
- **Issues**: Password reset email not sent in production

### PRD Generation
- **Total Tests**: 30
- **Passed**: 28
- **Failed**: 2
- **Coverage**: 92%
- **Issues**: AI service timeout on complex requests

### Design Generation
- **Total Tests**: 20
- **Passed**: 19
- **Failed**: 1
- **Coverage**: 88%
- **Issues**: Figma integration failing for large files

## Performance Results
- **API Response Time**: 95th percentile < 2 seconds ✅
- **PRD Generation**: 95th percentile < 30 seconds ✅
- **Database Queries**: 95th percentile < 100ms ✅
- **Page Load Time**: 95th percentile < 3 seconds ✅

## Security Results
- **Vulnerability Scan**: No high-severity issues found ✅
- **Penetration Testing**: No critical vulnerabilities ✅
- **Code Analysis**: 2 medium-severity issues found ⚠️

## Recommendations
1. Fix password reset email issue in production
2. Implement retry mechanism for AI service timeouts
3. Optimize Figma integration for large files
4. Address medium-severity security issues
```

This comprehensive testing documentation provides a solid foundation for ensuring the quality and reliability of the Braavo platform. The testing strategy covers all aspects of the application from unit tests to end-to-end testing, performance testing, and security testing.
