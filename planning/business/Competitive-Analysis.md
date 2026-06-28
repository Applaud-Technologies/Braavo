# Braavo - Competitive Analysis

> **Status:** Draft  
> **Last Updated:** 2026-06-27  
> **Owner:** [TBD]

## 1. Market Positioning

Braavo occupies a unique position at the intersection of three tool categories:

```
                    PM Tools
                   (Notion, Coda)
                        │
                        ▼
    ┌───────────────────────────────────────┐
    │                                       │
    │              BRAAVO                   │
    │       PRD → Design → Code             │
    │                                       │
    └───────────────────────────────────────┘
           ▲                       ▲
           │                       │
    Design Tools              DevTools
   (Figma, Polymet)      (Cursor, Copilot)
```

## 2. Competitive Landscape

### PM & Documentation Tools

| Competitor | Strengths | Weaknesses | Our Advantage |
|------------|-----------|------------|---------------|
| **Notion** | Flexible, popular | No AI generation, no SDLC | End-to-end SDLC pipeline |
| **Coda** | Powerful automation | Steep learning curve | AI-first simplicity |
| **Confluence** | Enterprise adoption | Outdated UX, no AI | Modern AI-powered experience |
| **Linear** | Developer-friendly | Limited PM features | Full PRD generation |

### Design-to-Code Tools

| Competitor | Strengths | Weaknesses | Our Advantage |
|------------|-----------|------------|---------------|
| **Polymet.ai** | Strong design-to-code | No PRD integration, React only | Requirements-driven design, multi-framework |
| **v0 (Vercel)** | Quick prototyping | No full SDLC | Complete product lifecycle |
| **Figma Dev Mode** | Industry standard | Manual handoff | Automated code generation |
| **Builder.io** | Visual editing | Limited code output | Production-ready code |

### AI Coding Tools

| Competitor | Strengths | Weaknesses | Our Advantage |
|------------|-----------|------------|---------------|
| **GitHub Copilot** | Deep IDE integration | Code-only, no context | Business context from PRD |
| **Cursor** | Full IDE experience | No PM/design features | Cross-functional platform |
| **Replit AI** | Instant deployment | Limited enterprise features | Enterprise-ready architecture |

## 3. Feature Comparison Matrix

| Capability | Braavo | Polymet | Copilot | Notion | Figma |
|------------|--------|---------|---------|--------|-------|
| PRD Generation | ✅ | ❌ | ❌ | ❌ | ❌ |
| AI Coaching | ✅ | ❌ | ❌ | ❌ | ❌ |
| Wireframe Gen | ✅ | ✅ | ❌ | ❌ | ❌ |
| Design System | ✅ | ✅ | ❌ | ❌ | ✅ |
| Code Generation | ✅ | ✅ | ✅ | ❌ | ❌ |
| Multi-framework | ✅ | ❌ | ✅ | ❌ | ❌ |
| Team Collaboration | ✅ | Basic | ❌ | ✅ | ✅ |
| Figma Integration | ✅ | ✅ | ❌ | ❌ | N/A |
| UML Diagrams | ✅ | ❌ | ❌ | ❌ | ❌ |
| API Generation | ✅ | ❌ | ✅ | ❌ | ❌ |
| IaC Generation | ✅ | ❌ | ✅ | ❌ | ❌ |

## 4. Competitive Moats

### 1. Requirements-to-Code Pipeline
No competitor offers the full journey from product idea → PRD → design → code → deployment in a single platform.

### 2. Cross-Functional Collaboration
While tools specialize (Figma for designers, Copilot for devs), Braavo serves the entire product team.

### 3. Business Context Preservation
AI code generation understands the "why" from the PRD, not just the "what" from a prompt.

### 4. SDLC Artifact Generation
Automated generation of diagrams, test plans, and deployment configs that competitors don't address.

## 5. Threat Assessment

### High Threat
- **Figma** adding AI code generation
- **GitHub/Microsoft** expanding Copilot to PM features
- **Notion** adding AI SDLC features

### Medium Threat
- **New AI-native entrants** with full-stack approach
- **Polymet** expanding upstream to requirements

### Low Threat
- **Traditional PM tools** (slow to adopt AI)
- **Enterprise vendors** (Atlassian, ServiceNow)

## 6. Defensive Strategy

1. **Move fast on integration** - Lock in Figma/GitHub users before competitors
2. **Build switching costs** - Template libraries, team workflows, project history
3. **Focus on workflows** - Not just features, but end-to-end processes
4. **Enterprise relationships** - Long-term contracts with key accounts

---

*See also: [Business Model](Business-Model.md)*
