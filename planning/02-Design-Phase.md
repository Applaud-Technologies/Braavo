# Design Phase Documentation

## 1. System Architecture

### 1.1 High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                          Frontend Layer                         │
├─────────────────────────────────────────────────────────────────┤
│  React.js + TypeScript + Tailwind CSS + Redux Toolkit         │
│  • Chat Interface     • Design Canvas    • Code Editor        │
│  • Document Editor    • Collaboration    • Analytics Dashboard │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway Layer                        │
├─────────────────────────────────────────────────────────────────┤
│  • Authentication    • Rate Limiting    • Load Balancing       │
│  • API Versioning    • Request Logging  • Error Handling      │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Microservices Layer                        │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │  Core Service   │  │  AI Service     │  │ Design Service  │  │
│  │  • PRD Gen      │  │  • NLP Engine   │  │ • Wireframes    │  │
│  │  • Templates    │  │  • Code Gen     │  │ • Prototypes    │  │
│  │  • Collaboration│  │  • Analysis     │  │ • Figma Sync    │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │ Export Service  │  │ Integration Svc │  │ Analytics Svc   │  │
│  │ • PDF/Word Gen  │  │ • GitHub API    │  │ • Usage Metrics │  │
│  │ • Code Packages │  │ • Jira API      │  │ • Performance   │  │
│  │ • Asset Export  │  │ • Slack API     │  │ • Reporting     │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Data Layer                              │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   PostgreSQL    │  │     Redis       │  │   File Storage  │  │
│  │  • User Data    │  │  • Sessions     │  │  • Documents    │  │
│  │  • Documents    │  │  • Cache        │  │  • Assets       │  │
│  │  • Projects     │  │  • Job Queue    │  │  • Exports      │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### 1.2 Technology Stack

#### Frontend
- **Framework**: React.js 18+ with TypeScript
- **State Management**: Redux Toolkit + RTK Query
- **Styling**: Tailwind CSS with HeadlessUI
- **Build Tool**: Vite
- **Testing**: Jest + React Testing Library

#### Backend
- **Runtime**: .NET 8.0
- **Framework**: ASP.NET Core Web API
- **API Documentation**: OpenAPI 3.0 + Swagger
- **Authentication**: JWT + OAuth 2.0
- **Validation**: FluentValidation
- **ORM**: Entity Framework Core

#### Database
- **Primary**: PostgreSQL 15+
- **Cache**: Redis 7+
- **Search**: Elasticsearch (optional)
- **File Storage**: AWS S3 / Azure Blob Storage

#### Infrastructure
- **Cloud**: AWS / Azure / GCP
- **Containers**: Docker + Kubernetes
- **CI/CD**: GitHub Actions
- **Monitoring**: Prometheus + Grafana

### 1.3 Backend Architecture - CQRS with MediatR

The backend services implement the CQRS (Command Query Responsibility Segregation) pattern using MediatR for clean separation of concerns and improved maintainability.

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Controllers                          │
├─────────────────────────────────────────────────────────────────┤
│  • HTTP Request/Response Handling                               │
│  • Authentication/Authorization                                 │
│  • Input Validation                                            │
│  • Error Handling                                              │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                         MediatR Layer                           │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │              Cross-Cutting Concerns                         │ │
│  │  • ValidationBehavior    • LoggingBehavior                 │ │
│  │  • CachingBehavior      • PerformanceBehavior              │ │
│  └─────────────────────────────────────────────────────────────┘ │
│                              │                                  │
│                              ▼                                  │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                Request Routing                              │ │
│  │  • Command/Query Identification                             │ │
│  │  • Handler Resolution                                       │ │
│  │  • Pipeline Execution                                       │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    CQRS Command/Query Layer                     │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐                    ┌─────────────────┐     │
│  │   Commands      │                    │     Queries     │     │
│  │  (Write Ops)    │                    │   (Read Ops)    │     │
│  ├─────────────────┤                    ├─────────────────┤     │
│  │ CreateDocument  │                    │ GetDocuments    │     │
│  │ UpdateDocument  │                    │ GetDocumentById │     │
│  │ DeleteDocument  │                    │ SearchDocuments │     │
│  │ CreateProject   │                    │ GetProjects     │     │
│  │ UpdateProject   │                    │ GetProjectById  │     │
│  │ ...             │                    │ ...             │     │
│  └─────────────────┘                    └─────────────────┘     │
│           │                                        │             │
│           ▼                                        ▼             │
│  ┌─────────────────┐                    ┌─────────────────┐     │
│  │ Command Handlers│                    │ Query Handlers  │     │
│  │ • Business Logic│                    │ • Data Fetching │     │
│  │ • Validation    │                    │ • Filtering     │     │
│  │ • Authorization │                    │ • Sorting       │     │
│  │ • Side Effects  │                    │ • Pagination    │     │
│  └─────────────────┘                    └─────────────────┘     │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Repository & Unit of Work                     │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   Repositories  │  │  Unit of Work   │  │   Domain Model  │  │
│  │ • Data Access   │  │ • Transactions  │  │ • Entities      │  │
│  │ • Abstraction   │  │ • Consistency   │  │ • Value Objects │  │
│  │ • Testability   │  │ • Change Track  │  │ • Domain Logic  │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

#### CQRS Pattern Benefits
- **Separation of Concerns**: Commands (writes) and Queries (reads) are handled separately
- **Optimized Operations**: Different models for read and write operations
- **Scalability**: Independent scaling of read and write sides
- **Maintainability**: Clear separation of business logic
- **Testability**: Easy to unit test individual handlers

#### MediatR Pattern Benefits
- **Decoupling**: Controllers don't depend on specific business logic implementations
- **Cross-Cutting Concerns**: Behaviors for validation, logging, caching, etc.
- **Single Responsibility**: Each handler has a single purpose
- **Dependency Injection**: Easy to inject dependencies and mock for testing
- **Pipeline Processing**: Request processing through configurable pipeline

#### Implementation Example
```csharp
// Command Definition
public class CreateDocumentCommand : IRequest<DocumentDto>
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
    public Guid ProjectId { get; set; }
    public UserId UserId { get; set; }
}

// Command Handler
public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, DocumentDto>
{
    private readonly IDocumentRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<DocumentDto> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = Document.Create(request.Title, request.Content, request.Type, request.ProjectId, request.UserId);
        await _repository.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<DocumentDto>(document);
    }
}

// Controller Usage
[HttpPost]
public async Task<ActionResult<ApiResponse<DocumentDto>>> CreateDocument([FromBody] CreateDocumentRequest request)
{
    var command = new CreateDocumentCommand
    {
        Title = request.Title,
        Content = request.Content,
        Type = request.Type,
        ProjectId = request.ProjectId,
        UserId = GetCurrentUserId()
    };

    var result = await _mediator.Send(command);
    return StatusCode(201, new ApiResponse<DocumentDto> { Success = true, Data = result });
}
```

### 1.4 Authentication & Security Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    OAuth 2.0 & OpenID Connect                  │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐ │
│  │   Google OAuth  │  │ Facebook OAuth  │  │  OpenID Connect │ │
│  │   Provider      │  │   Provider      │  │   Provider      │ │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘ │
│                               │                                 │
│                               ▼                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │              Identity Provider Abstraction                  │ │
│  │  • OAuth 2.0 Authorization Code Flow                       │ │
│  │  • PKCE (Proof Key for Code Exchange)                      │ │
│  │  • JWT Token Management                                     │ │
│  │  • Refresh Token Handling                                   │ │
│  └─────────────────────────────────────────────────────────────┘ │
│                               │                                 │
│                               ▼                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                 Authorization Server                        │ │
│  │  • Token Validation                                         │ │
│  │  • Claims Extraction                                        │ │
│  │  • Role-Based Access Control                                │ │
│  │  • Permission Management                                     │ │
│  └─────────────────────────────────────────────────────────────┘ │
│                               │                                 │
│                               ▼                                 │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │                 Protected Resources                         │ │
│  │  • API Endpoints                                            │ │
│  │  • User Data Access                                         │ │
│  │  • Document Operations                                       │ │
│  │  • Project Management                                       │ │
│  └─────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### OAuth Configuration
```json
{
  "Authentication": {
    "OpenIdConnect": {
      "Authority": "https://accounts.google.com",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "CallbackPath": "/signin-oidc",
      "SignedOutCallbackPath": "/signout-callback-oidc"
    },
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret",
      "CallbackPath": "/signin-google"
    },
    "Facebook": {
      "AppId": "your-facebook-app-id",
      "AppSecret": "your-facebook-app-secret",
      "CallbackPath": "/signin-facebook"
    },
    "JWT": {
      "Key": "your-jwt-signing-key",
      "Issuer": "chatprd.com",
      "Audience": "chatprd-api",
      "ExpirationInMinutes": 60
    }
  }
}
```

## 2. Database Design

### 2.1 Entity Relationship Diagram

```
┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐
│      Users      │         │  Organizations  │         │      Teams      │
├─────────────────┤         ├─────────────────┤         ├─────────────────┤
│ id (PK)         │    ┌────│ id (PK)         │    ┌────│ id (PK)         │
│ email           │    │    │ name            │    │    │ name            │
│ name            │    │    │ plan_type       │    │    │ organization_id │
│ password_hash   │    │    │ created_at      │    │    │ created_at      │
│ role            │    │    │ settings        │    │    │ settings        │
│ organization_id │────┘    └─────────────────┘    │    └─────────────────┘
│ created_at      │                                │             │
│ settings        │                                │             │
└─────────────────┘                                │             │
         │                                         │             │
         │                                         │             │
         ▼                                         ▼             │
┌─────────────────┐                       ┌─────────────────┐    │
│   TeamMembers   │                       │     Projects    │    │
├─────────────────┤                       ├─────────────────┤    │
│ id (PK)         │                       │ id (PK)         │    │
│ user_id (FK)    │                       │ name            │    │
│ team_id (FK)    │───────────────────────│ team_id (FK)    │────┘
│ role            │                       │ created_by      │
│ joined_at       │                       │ created_at      │
└─────────────────┘                       │ updated_at      │
                                          │ status          │
                                          └─────────────────┘
                                                   │
                                                   ▼
                                          ┌─────────────────┐
                                          │    Documents    │
                                          ├─────────────────┤
                                          │ id (PK)         │
                                          │ project_id (FK) │
                                          │ title           │
                                          │ content         │
                                          │ type            │
                                          │ version         │
                                          │ created_by      │
                                          │ created_at      │
                                          │ updated_at      │
                                          └─────────────────┘
                                                   │
                                                   ▼
                                          ┌─────────────────┐
                                          │    Artifacts    │
                                          ├─────────────────┤
                                          │ id (PK)         │
                                          │ document_id (FK)│
                                          │ type            │
                                          │ content         │
                                          │ metadata        │
                                          │ created_at      │
                                          └─────────────────┘
```

### 2.2 Database Schema

#### Users Table
```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255),
    role VARCHAR(50) DEFAULT 'user',
    organization_id UUID REFERENCES organizations(id),
    avatar_url TEXT,
    preferences JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### Organizations Table
```sql
CREATE TABLE organizations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    plan_type VARCHAR(50) DEFAULT 'free',
    billing_email VARCHAR(255),
    settings JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### Projects Table
```sql
CREATE TABLE projects (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    organization_id UUID REFERENCES organizations(id),
    created_by UUID REFERENCES users(id),
    status VARCHAR(50) DEFAULT 'active',
    settings JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### Documents Table
```sql
CREATE TABLE documents (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    project_id UUID REFERENCES projects(id),
    title VARCHAR(255) NOT NULL,
    content JSONB NOT NULL,
    type VARCHAR(50) NOT NULL, -- 'prd', 'wireframe', 'code', etc.
    version INTEGER DEFAULT 1,
    created_by UUID REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

#### Artifacts Table
```sql
CREATE TABLE artifacts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    document_id UUID REFERENCES documents(id),
    type VARCHAR(50) NOT NULL, -- 'diagram', 'code', 'export', etc.
    content JSONB NOT NULL,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## 3. API Specifications

### 3.1 REST API Endpoints

#### Authentication Endpoints
```yaml
/api/auth:
  POST /login:
    summary: User login
    requestBody:
      required: true
      content:
        application/json:
          schema:
            type: object
            properties:
              email:
                type: string
                format: email
              password:
                type: string
    responses:
      200:
        description: Login successful
        content:
          application/json:
            schema:
              type: object
              properties:
                token:
                  type: string
                user:
                  $ref: '#/components/schemas/User'

  POST /register:
    summary: User registration
    requestBody:
      required: true
      content:
        application/json:
          schema:
            type: object
            properties:
              email:
                type: string
                format: email
              name:
                type: string
              password:
                type: string
    responses:
      201:
        description: User created successfully
```

#### Document Endpoints
```yaml
/api/documents:
  GET /:
    summary: Get user documents
    parameters:
      - name: project_id
        in: query
        schema:
          type: string
          format: uuid
      - name: type
        in: query
        schema:
          type: string
          enum: [prd, wireframe, code]
    responses:
      200:
        description: List of documents
        content:
          application/json:
            schema:
              type: array
              items:
                $ref: '#/components/schemas/Document'

  POST /:
    summary: Create new document
    requestBody:
      required: true
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/CreateDocumentRequest'
    responses:
      201:
        description: Document created successfully

  GET /{id}:
    summary: Get document by ID
    parameters:
      - name: id
        in: path
        required: true
        schema:
          type: string
          format: uuid
    responses:
      200:
        description: Document details
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/Document'

  PUT /{id}:
    summary: Update document
    parameters:
      - name: id
        in: path
        required: true
        schema:
          type: string
          format: uuid
    requestBody:
      required: true
      content:
        application/json:
          schema:
            $ref: '#/components/schemas/UpdateDocumentRequest'
    responses:
      200:
        description: Document updated successfully
```

#### AI Generation Endpoints
```yaml
/api/ai:
  POST /generate-prd:
    summary: Generate PRD from idea
    requestBody:
      required: true
      content:
        application/json:
          schema:
            type: object
            properties:
              idea:
                type: string
                description: Product idea description
              template:
                type: string
                description: Template to use
              preferences:
                type: object
                description: User preferences
    responses:
      200:
        description: Generated PRD
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/GeneratedPRD'

  POST /generate-wireframe:
    summary: Generate wireframe from requirements
    requestBody:
      required: true
      content:
        application/json:
          schema:
            type: object
            properties:
              requirements:
                type: string
                description: User requirements
              design_system:
                type: string
                description: Design system to use
              fidelity:
                type: string
                enum: [low, high]
    responses:
      200:
        description: Generated wireframe
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/GeneratedWireframe'

  POST /generate-code:
    summary: Generate code from design
    requestBody:
      required: true
      content:
        application/json:
          schema:
            type: object
            properties:
              design:
                type: object
                description: Design specification
              framework:
                type: string
                enum: [react, vue, angular]
              language:
                type: string
                enum: [typescript, javascript]
    responses:
      200:
        description: Generated code
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/GeneratedCode'
```

### 3.2 Data Schemas

#### User Schema
```yaml
User:
  type: object
  properties:
    id:
      type: string
      format: uuid
    email:
      type: string
      format: email
    name:
      type: string
    role:
      type: string
      enum: [user, admin, owner]
    organization_id:
      type: string
      format: uuid
    avatar_url:
      type: string
      format: uri
    preferences:
      type: object
    created_at:
      type: string
      format: date-time
    updated_at:
      type: string
      format: date-time
```

#### Document Schema
```yaml
Document:
  type: object
  properties:
    id:
      type: string
      format: uuid
    project_id:
      type: string
      format: uuid
    title:
      type: string
    content:
      type: object
      description: Document content in JSON format
    type:
      type: string
      enum: [prd, wireframe, code, test, deployment]
    version:
      type: integer
    created_by:
      type: string
      format: uuid
    created_at:
      type: string
      format: date-time
    updated_at:
      type: string
      format: date-time
```

## 4. UML Diagrams

### 4.1 Class Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                          Core Classes                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │      User       │    │  Organization   │    │     Project     │  │
│  ├─────────────────┤    ├─────────────────┤    ├─────────────────┤  │
│  │ +id: UUID       │    │ +id: UUID       │    │ +id: UUID       │  │
│  │ +email: String  │    │ +name: String   │    │ +name: String   │  │
│  │ +name: String   │    │ +planType: Enum │    │ +description    │  │
│  │ +role: Enum     │    │ +settings: JSON │    │ +status: Enum   │  │
│  │ +orgId: UUID    │    │ +createdAt: Date│    │ +createdAt: Date│  │
│  ├─────────────────┤    ├─────────────────┤    ├─────────────────┤  │
│  │ +authenticate() │    │ +getUsers()     │    │ +getDocuments() │  │
│  │ +authorize()    │    │ +addUser()      │    │ +createDoc()    │  │
│  │ +updateProfile()│    │ +updatePlan()   │    │ +updateStatus() │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
│           │                       │                       │          │
│           └───────────────────────┼───────────────────────┘          │
│                                   │                                  │
│  ┌─────────────────┐              │              ┌─────────────────┐  │
│  │    Document     │              │              │    Artifact     │  │
│  ├─────────────────┤              │              ├─────────────────┤  │
│  │ +id: UUID       │              │              │ +id: UUID       │  │
│  │ +title: String  │              │              │ +type: Enum     │  │
│  │ +content: JSON  │              │              │ +content: JSON  │  │
│  │ +type: Enum     │              │              │ +metadata: JSON │  │
│  │ +version: Int   │              │              │ +createdAt: Date│  │
│  ├─────────────────┤              │              ├─────────────────┤  │
│  │ +save()         │              │              │ +generate()     │  │
│  │ +validate()     │              │              │ +export()       │  │
│  │ +getHistory()   │              │              │ +getDownloadUrl│  │
│  └─────────────────┘              │              └─────────────────┘  │
│                                   │                                  │
│  ┌─────────────────────────────────┴─────────────────────────────────┐  │
│  │                        AI Services                               │  │
│  ├─────────────────────────────────────────────────────────────────┤  │
│  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │  │
│  │  │   PRDGenerator  │  │ WireframeGen    │  │  CodeGenerator  │  │  │
│  │  ├─────────────────┤  ├─────────────────┤  ├─────────────────┤  │  │
│  │  │ +generatePRD()  │  │ +generateWF()   │  │ +generateCode() │  │  │
│  │  │ +refineContent()│  │ +applyDesignSys()│  │ +validateCode() │  │  │
│  │  │ +getTemplate()  │  │ +createPrototype│  │ +optimizeCode() │  │  │
│  │  └─────────────────┘  └─────────────────┘  └─────────────────┘  │  │
│  └─────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### 4.2 Sequence Diagram - PRD Generation

```
┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐
│  User   │    │Frontend │    │API Gate │    │AI Service│   │Database │
└─────────┘    └─────────┘    └─────────┘    └─────────┘    └─────────┘
     │              │              │              │              │
     │ 1. Input idea│              │              │              │
     │─────────────►│              │              │              │
     │              │ 2. POST /api/ai/generate-prd │              │
     │              │─────────────►│              │              │
     │              │              │ 3. Validate & Route         │
     │              │              │─────────────►│              │
     │              │              │              │ 4. Process idea       │
     │              │              │              │ & Generate PRD        │
     │              │              │              │              │
     │              │              │              │ 5. Save document      │
     │              │              │              │─────────────►│
     │              │              │              │              │
     │              │              │ 6. Return generated PRD     │
     │              │              │◄─────────────│              │
     │              │ 7. PRD Response              │              │
     │              │◄─────────────│              │              │
     │ 8. Display PRD│              │              │              │
     │◄─────────────│              │              │              │
     │              │              │              │              │
     │ 9. Refine PRD│              │              │              │
     │─────────────►│              │              │              │
     │              │ 10. POST /api/ai/refine     │              │
     │              │─────────────►│              │              │
     │              │              │─────────────►│              │
     │              │              │              │ 11. Update document   │
     │              │              │              │─────────────►│
     │              │              │◄─────────────│              │
     │              │◄─────────────│              │              │
     │◄─────────────│              │              │              │
```

### 4.3 Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                     Frontend Components                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │   ChatInterface │    │  DocumentEditor │    │  DesignCanvas   │  │
│  │                 │    │                 │    │                 │  │
│  │ • Input Field   │    │ • Rich Text     │    │ • Wireframe     │  │
│  │ • Chat History  │    │ • Version Ctrl  │    │ • Drag & Drop   │  │
│  │ • AI Responses  │    │ • Collaboration │    │ • Component Lib │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │   CodeEditor    │    │  ProjectManager │    │  SettingsPanel  │  │
│  │                 │    │                 │    │                 │  │
│  │ • Syntax Highlight│  │ • File Explorer │    │ • User Profile  │  │
│  │ • Auto-complete │    │ • Project Tree  │    │ • Team Settings │  │
│  │ • Preview Mode  │    │ • Collaboration │    │ • Billing Info  │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Backend Components                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │  AuthController │    │  DocumentController│ │  AIController   │  │
│  │                 │    │                 │    │                 │  │
│  │ • Login/Logout  │    │ • CRUD Operations│   │ • PRD Generation│  │
│  │ • JWT Management│    │ • Version Control│   │ • Design Gen    │  │
│  │ • Role Check    │    │ • Collaboration │    │ • Code Gen      │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │  ExportService  │    │ IntegrationSvc  │    │  NotificationSvc│  │
│  │                 │    │                 │    │                 │  │
│  │ • PDF Export    │    │ • Figma API     │    │ • Email Alerts  │  │
│  │ • Code Package  │    │ • GitHub API    │    │ • Slack Notify  │  │
│  │ • Asset Export  │    │ • Jira API      │    │ • Push Notify   │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## 5. Design System Architecture

### 5.1 Design Token Structure

```yaml
Design Tokens:
  Colors:
    Primary:
      - primary-50: '#eff6ff'
      - primary-500: '#3b82f6'
      - primary-900: '#1e3a8a'
    Secondary:
      - secondary-50: '#f8fafc'
      - secondary-500: '#64748b'
      - secondary-900: '#0f172a'
    Semantic:
      - success: '#10b981'
      - warning: '#f59e0b'
      - error: '#ef4444'
      - info: '#3b82f6'

  Typography:
    Font Families:
      - sans: ['Inter', 'system-ui', 'sans-serif']
      - mono: ['Fira Code', 'monospace']
    Font Sizes:
      - xs: '0.75rem'
      - sm: '0.875rem'
      - base: '1rem'
      - lg: '1.125rem'
      - xl: '1.25rem'
      - 2xl: '1.5rem'

  Spacing:
    - 0: '0px'
    - 1: '0.25rem'
    - 2: '0.5rem'
    - 4: '1rem'
    - 8: '2rem'
    - 16: '4rem'

  Breakpoints:
    - sm: '640px'
    - md: '768px'
    - lg: '1024px'
    - xl: '1280px'
    - 2xl: '1536px'
```

### 5.2 Component Library Structure

```
Component Library:
├── Basic Components
│   ├── Button
│   ├── Input
│   ├── Card
│   ├── Modal
│   └── Tooltip
├── Form Components
│   ├── FormField
│   ├── Select
│   ├── Checkbox
│   ├── Radio
│   └── FileUpload
├── Layout Components
│   ├── Header
│   ├── Sidebar
│   ├── Footer
│   ├── Grid
│   └── Container
├── Data Display
│   ├── Table
│   ├── List
│   ├── Chart
│   ├── Progress
│   └── Badge
└── Specialized Components
    ├── ChatInterface
    ├── CodeEditor
    ├── DesignCanvas
    ├── DiagramViewer
    └── ExportModal
```

## 6. Integration Architecture

### 6.1 Third-party Integration Map

```
┌─────────────────────────────────────────────────────────────────┐
│                    Integration Layer                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │   Figma API     │    │   GitHub API    │    │    Jira API    │  │
│  │                 │    │                 │    │                 │  │
│  │ • Export Designs│    │ • Create Repos  │    │ • Sync Issues   │  │
│  │ • Import Assets │    │ • Push Code     │    │ • Create Tickets│  │
│  │ • Sync Components│   │ • Pull Requests │    │ • Update Status │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐  │
│  │   OpenAI API    │    │   Slack API     │    │  Payment API    │  │
│  │                 │    │                 │    │                 │  │
│  │ • Text Generation│   │ • Notifications │    │ • Billing       │  │
│  │ • Code Analysis │    │ • Team Updates  │    │ • Subscriptions │  │
│  │ • Image Gen     │    │ • Bot Commands  │    │ • Usage Tracking│  │
│  └─────────────────┘    └─────────────────┘    └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### 6.2 API Integration Patterns

#### Webhook Processing
```typescript
interface WebhookEvent {
  source: 'figma' | 'github' | 'jira';
  event: string;
  data: any;
  timestamp: Date;
}

class WebhookProcessor {
  async processEvent(event: WebhookEvent) {
    switch (event.source) {
      case 'figma':
        return this.processFigmaEvent(event);
      case 'github':
        return this.processGitHubEvent(event);
      case 'jira':
        return this.processJiraEvent(event);
    }
  }
}
```

#### OAuth Integration
```typescript
interface OAuthConfig {
  clientId: string;
  clientSecret: string;
  redirectUri: string;
  scopes: string[];
}

class IntegrationService {
  async connectService(service: string, userId: string) {
    const authUrl = this.buildAuthUrl(service);
    return { authUrl, state: this.generateState(userId) };
  }
  
  async handleCallback(code: string, state: string) {
    const tokens = await this.exchangeCodeForTokens(code);
    return this.storeTokens(tokens, state);
  }
}
```

This design phase documentation provides a comprehensive technical foundation for implementing the Braavo Clone platform. The architecture is designed to be scalable, maintainable, and extensible while supporting all the features outlined in the PRD. 