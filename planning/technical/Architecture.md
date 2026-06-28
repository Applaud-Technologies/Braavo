# Braavo - Technical Architecture

> **Status:** Draft  
> **Last Updated:** 2026-06-27  
> **Owner:** [TBD]  
> **Detailed Design:** [02-Design-Phase.md](../sdlc/02-Design-Phase.md)

## 1. Architecture Overview

Braavo follows a microservices architecture with Clean Architecture principles in each service.

```
┌─────────────────────────────────────────────────────────────────┐
│                          Frontend Layer                         │
│  React.js + TypeScript + Tailwind CSS + Redux Toolkit          │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway Layer                        │
│  Authentication • Rate Limiting • Load Balancing               │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Microservices Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │Core Service │  │ AI Service  │  │Design Svc   │             │
│  │PRD, Templates│  │NLP, CodeGen │  │Wireframes   │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │Export Svc   │  │Integration  │  │Analytics    │             │
│  │PDF, Code    │  │GitHub, Jira │  │Metrics      │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                         Data Layer                              │
│  PostgreSQL (Primary) • Redis (Cache) • S3 (Files)             │
└─────────────────────────────────────────────────────────────────┘
```

## 2. Technology Stack

### Frontend
| Component | Technology |
|-----------|------------|
| Framework | React.js 18+ with TypeScript |
| State | Redux Toolkit + RTK Query |
| Styling | Tailwind CSS + shadcn/ui |
| Build | Vite |
| Testing | Jest + React Testing Library |

### Backend
| Component | Technology |
|-----------|------------|
| Runtime | .NET 8.0 |
| Framework | ASP.NET Core Web API |
| Architecture | Ardalis Clean Architecture |
| Endpoints | FastEndpoints-style vertical slices |
| Patterns | CQRS/MediatR for workflows |
| ORM | Entity Framework Core |
| Validation | FluentValidation |

### Data
| Component | Technology |
|-----------|------------|
| Primary DB | PostgreSQL 15+ |
| Cache | Redis 7+ |
| Search | PostgreSQL FTS (MVP) → OpenSearch (post-MVP, optional) |
| Files | Local filesystem (MVP) → MinIO or S3-compatible (production) |

### Infrastructure
| Component | Technology |
|-----------|------------|
| Hosting | VPS (MVP) → Cloud provider (scale) |
| Containers | Docker + Docker Compose (MVP) → Kubernetes (scale) |
| CI/CD | GitHub Actions |
| Monitoring | Prometheus + Grafana |

## 3. Key Architecture Decisions

### ADR-001: Clean Architecture with FastEndpoints
**Decision:** Use Ardalis Clean Architecture with FastEndpoints-style vertical endpoint classes.

**Rationale:**
- Thin HTTP layer that delegates to application use cases
- Feature locality (request, response, validator together)
- OpenAPI-friendly endpoint metadata
- Clear separation of API and domain layers

### ADR-002: CQRS/MediatR for Workflows
**Decision:** Use CQRS pattern via MediatR for document, artifact, and generation workflows.

**Rationale:**
- Separation of read/write concerns
- Pipeline behaviors for cross-cutting concerns
- Independent scaling of read/write sides
- Easier testing of individual handlers

### ADR-003: LLM Provider Abstraction
**Decision:** Abstract LLM provider behind internal interface, with OpenAI as initial implementation.

**Rationale:**
- Avoid vendor lock-in
- Enable multi-provider strategy
- Simplify testing with mock providers
- Future-proof against provider changes

### ADR-004: Search Strategy
**Decision:** Use PostgreSQL full-text search for MVP. Migrate to OpenSearch post-MVP if advanced search is needed.

**Rationale:**
- PostgreSQL FTS is sufficient for basic document/PRD search
- Reduces operational complexity for MVP (no extra service)
- OpenSearch (Apache 2.0, Elasticsearch fork) is the recommended upgrade path
- OpenSearch adds: vector/semantic search, advanced relevance tuning, log analytics

### ADR-005: File Storage Strategy
**Decision:** Abstract file storage behind `IFileStorage` interface. Use local filesystem for MVP, MinIO or S3-compatible for production.

**Rationale:**
- Abstraction allows swapping storage backends without code changes
- Local filesystem simplifies MVP development (no extra services)
- MinIO provides S3-compatible API for self-hosted production
- Same AWS SDK works with MinIO, S3, and other S3-compatible stores
- Avoids cloud vendor lock-in while preserving option to use cloud storage

### ADR-006: Infrastructure Strategy
**Decision:** Start with VPS + Docker Compose for MVP. Migrate to Kubernetes on cloud providers when scaling requires it.

**Rationale:**
- VPS is cost-effective and sufficient for early users
- Docker Compose simplifies deployment without K8s complexity
- Containerized from day one ensures easy migration path
- Avoid premature cloud/K8s overhead until scale demands it
- GitHub Actions CI/CD works the same regardless of target environment

## 4. Performance Targets

| Operation | Target |
|-----------|--------|
| Document generation | < 2 seconds |
| Wireframe generation | < 8 seconds |
| Code generation | < 15 seconds |
| Page load | < 3 seconds |
| API response | < 1 second |

## 5. Security Architecture

- OAuth 2.0 / OpenID Connect for authentication
- JWT tokens with refresh mechanism
- Role-based access control (RBAC)
- End-to-end encryption for sensitive data
- SOC 2 / GDPR compliance

---

*For detailed designs, see [02-Design-Phase.md](../sdlc/02-Design-Phase.md)*
