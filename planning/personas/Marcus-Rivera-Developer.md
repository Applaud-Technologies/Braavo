# Marcus Rivera — Software Architect/Developer

> **Archetype:** The Context-Starved Builder  
> **Role:** Senior Software Engineer / Tech Lead  
> **Experience:** 6-12 years in engineering  
> **Segment:** Startup to Mid-market

---

## Quick Reference

| Field | Value |
|-------|-------|
| **Key Quote** | *"Just tell me what you actually want built — I'll figure out the how."* |
| **Primary Value** | Requirements that translate to code |
| **Buying Role** | Influencer — validates technical credibility, can block or boost |

---

## Demographics

- Age 28-42, reports to Engineering Manager or CTO
- Leads technical decisions for 1-3 features or services
- Works closely with 1-2 PMs and a small dev team
- Often the one who translates PRD into technical spec

## Goals & Motivations

- Build systems that are clean, scalable, and maintainable
- Understand the "why" so they can make good tradeoffs
- Reduce time spent on boilerplate and scaffolding
- Ship without rework caused by requirement ambiguity
- Grow toward Staff/Principal or Architect role

## Frustrations & Pain Points

- PRDs lack technical context — "what does 'fast' mean?"
- Requirements change after implementation starts
- Spends hours writing API specs that could be generated
- Boilerplate setup for every new service/feature
- Meetings to clarify what should have been in the doc
- Designers hand off mockups with no spec for edge cases

## Buying Triggers

- Starting a greenfield project and wants to move fast
- Just inherited a messy codebase with no documentation
- Tired of writing OpenAPI specs by hand
- PM handed over a vague PRD and expects estimates by EOD
- Team adopted a new framework; needs scaffolding

## Objections & Concerns

- "Generated code is usually garbage I have to rewrite"
- "I don't trust AI to understand our architecture"
- "Will this lock us into weird patterns?"
- "Security — is it sending our code somewhere?"
- "I'd rather just write it myself than fix AI output"

## Channels & Influences

- Hacker News, Reddit (r/programming, r/ExperiencedDevs)
- Dev.to, personal blogs of engineers they respect
- GitHub trending, Twitter/X dev community
- Conference talks (QCon, StrangeLoop, local meetups)
- Strong influence from what their tech lead or architect endorses

## Messaging Angles

| Priority | Message |
|----------|---------|
| 1 | **Context preservation:** "Requirements flow into API specs — no game of telephone" |
| 2 | **Scaffold, don't generate:** "Clean starting points, not AI slop" |
| 3 | **Time on architecture, not boilerplate:** "Focus on the hard problems" |
| 4 | **Works with your stack:** "React, .NET, your design system — not generic code" |

## Day-in-the-Life Scenario

Marcus gets tagged on a PRD for a new notifications feature. Instead of scheduling a meeting to clarify requirements, he opens the Braavo doc and sees user stories with acceptance criteria. He clicks "Generate API Spec" and gets an OpenAPI contract based on the data entities implied by the requirements. He tweaks a few field names, generates a .NET project scaffold with FastEndpoints stubs, and starts implementing the actual logic. The PM later adds a requirement — Marcus regenerates the affected endpoint stub without touching his business logic.

## Feature Prioritization

| Priority | Feature |
|----------|---------|
| Must-have | API contract generation (OpenAPI) |
| Must-have | Code scaffolding (.NET, React) |
| High | Database schema / ERD generation |
| High | Requirements with clear acceptance criteria |
| Medium | Architecture diagrams (system context, sequence) |
| Medium | Integration with GitHub |
| Lower | Wireframes (that's design's job) |

## Competitive Alternatives

| Alternative | Why They'd Use It |
|-------------|-------------------|
| ChatGPT/Claude | Good for snippets, but no project context |
| GitHub Copilot | Great for inline, but doesn't do architecture |
| Swagger/OpenAPI generators | Manual, no requirements link |
| JetBrains AI / Cursor | IDE-focused, not requirements-driven |
| Hand-written | "I'll just do it myself" |

## Segment Variants

**Startup (seed to Series A):**
- Wears architect hat by default — no dedicated architect
- Values speed over perfection; "ship it" culture
- More willing to try new tools if they save time
- Less process; might adopt without formal approval
- Cares about free tier and no onboarding friction

**Mid-market (50-500 employees):**
- May have dedicated architects who set standards
- Needs to fit into existing tech stack and patterns
- Tool adoption requires buy-in from tech lead or EM
- More concerned about maintainability and handoff
- Values integrations with existing CI/CD and GitHub

## Validation Notes

- [ ] Is API spec generation actually valued, or do devs prefer hand-writing?
- [ ] What's the threshold for "good enough" scaffolding quality?
- [ ] Do devs want this from the PRD tool, or separate?
- [ ] Is security concern real or reflexive?

---

*See also: [Maya Chen (PM)](Maya-Chen-PM.md), [Jordan Park (Designer)](Jordan-Park-Designer.md), [Priya Sharma (QA Lead)](Priya-Sharma-QA.md)*
