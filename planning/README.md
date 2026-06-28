# Braavo Planning Documentation

> **Last Updated:** 2026-06-27

## Quick Links

| Document | Status | Description |
|----------|--------|-------------|
| [PRD.md](PRD.md) | Draft | Product Requirements Document - the source of truth |

## Document Structure

```
planning/
├── README.md              ← You are here
├── PRD.md                 ← Core product requirements
│
├── personas/              ← Target user personas
│   ├── Maya-Chen-PM.md
│   ├── Marcus-Rivera-Developer.md
│   ├── Jordan-Park-Designer.md
│   └── Priya-Sharma-QA.md
│
├── business/              ← Business strategy & analysis
│   ├── Business-Model.md  ← Pricing, revenue, market
│   └── Competitive-Analysis.md
│
├── technical/             ← Technical design decisions
│   └── Architecture.md    ← System architecture summary
│
└── sdlc/                  ← Detailed SDLC phase artifacts
    ├── 01-Requirements-Phase.md
    ├── 02-Design-Phase.md
    ├── 03-Implementation-Phase.md
    ├── 04-Testing-Phase.md
    ├── 05-Deployment-Phase.md
    └── 06-Maintenance-Phase.md
```

## Document Purposes

### PRD.md (Start Here)
The canonical source of truth for **what** we're building and **why**. Contains:
- Problem statement and vision
- Target users and their jobs-to-be-done
- Functional requirements (features)
- Success metrics and roadmap
- Out of scope boundaries

### personas/
**Detailed user personas for messaging, positioning, and feature prioritization.** Reference these by name in user stories.

| Persona | Role | Archetype | Buying Role |
|---------|------|-----------|-------------|
| [Maya Chen](personas/Maya-Chen-PM.md) | Product Manager | The Overwhelmed Orchestrator | Champion |
| [Marcus Rivera](personas/Marcus-Rivera-Developer.md) | Developer/Architect | The Context-Starved Builder | Influencer |
| [Jordan Park](personas/Jordan-Park-Designer.md) | UI/UX Designer | The Requirements Translator | User |
| [Priya Sharma](personas/Priya-Sharma-QA.md) | Team Lead/QA | The Quality Gatekeeper | Beneficiary |

**Usage in user stories:**
> "As **Maya (PM)**, I want to generate a PRD from a chat conversation so that I can get from idea to document in under an hour."

### business/
**Business strategy and market positioning.** Separated from PRD to keep product requirements focused.

| File | Contents |
|------|----------|
| Business-Model.md | Pricing tiers, revenue streams, financial targets |
| Competitive-Analysis.md | Market landscape, competitors, differentiation |

### technical/
**Technical architecture and design decisions.** The PRD says "what," these docs say "how."

| File | Contents |
|------|----------|
| Architecture.md | High-level system design, tech stack, ADRs |

### sdlc/
**Detailed artifacts for each SDLC phase.** Generated from and aligned with the PRD.

| File | Contents |
|------|----------|
| 01-Requirements-Phase.md | Use cases, user stories, acceptance criteria |
| 02-Design-Phase.md | System architecture, database design, API specs |
| 03-Implementation-Phase.md | Code scaffolding, patterns, best practices |
| 04-Testing-Phase.md | Test plans, test cases, QA strategy |
| 05-Deployment-Phase.md | Infrastructure, CI/CD, environments |
| 06-Maintenance-Phase.md | Operations, monitoring, change management |

## Status Legend

| Status | Meaning |
|--------|---------|
| Draft | Initial creation, open for major changes |
| In Review | Awaiting stakeholder feedback |
| Approved | Signed off, ready for implementation |
| Implemented | Features shipped, doc is historical reference |

## How to Use This Documentation

1. **Starting a new feature?** Start with [PRD.md](PRD.md) for context
2. **Writing user stories?** Reference [personas/](personas/) by name
3. **Understanding the market?** See [business/](business/)
4. **Making technical decisions?** See [technical/Architecture.md](technical/Architecture.md)
5. **Implementing a phase?** See the relevant [sdlc/](sdlc/) document

## Change Log

| Date | Change | Author |
|------|--------|--------|
| 2026-06-27 | Initial reorganization from single PRD to structured docs | — |
