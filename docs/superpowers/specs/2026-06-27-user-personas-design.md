# Braavo User Personas

> **Version:** 1.0  
> **Status:** Draft  
> **Created:** 2026-06-27  
> **Purpose:** Product messaging, positioning, and feature prioritization

---

## How to Use This Document

These personas are the reference point for product decisions:

- **User Stories:** Each story should map to one or more personas. Ask "who is this for?" and reference the persona by name.
- **Feature Prioritization:** Check the "Feature Prioritization" table in each persona to understand what matters to them.
- **Messaging:** Use the "Messaging Angles" and "Key Quote" when writing copy, landing pages, or onboarding flows.
- **Objection Handling:** Reference "Objections & Concerns" when preparing sales materials or FAQ content.
- **Validation:** Track the "Validation Notes" as you conduct user research — these are assumptions to test.

**Segment Context:** Each persona includes "Segment Variants" for Startup vs. Mid-market contexts. Reference the appropriate variant when targeting specific company sizes.

---

## Target Segments

| Segment | Definition | Characteristics |
|---------|------------|-----------------|
| **Startup** | Seed to Series A | Small teams (2-20), fast-moving, price-sensitive, informal processes |
| **Mid-market** | 50-500 employees | Dedicated product teams, some process, tool purchases need justification |

---

## Persona Summary

| Persona | Role | Archetype | Primary Value |
|---------|------|-----------|---------------|
| Maya Chen | Product Manager | The Overwhelmed Orchestrator | Time back on PRD creation |
| Marcus Rivera | Software Architect/Developer | The Context-Starved Builder | Requirements that translate to code |
| Jordan Park | UI/UX Designer | The Requirements Translator | Context before pixels |
| Priya Sharma | Team Lead/QA | The Quality Gatekeeper | Testable requirements upfront |

---

## Persona 1: Product Manager — "The Overwhelmed Orchestrator"

### Quick Reference

| Field | Value |
|-------|-------|
| **Name** | Maya Chen |
| **Title** | Senior Product Manager |
| **Experience** | 4-8 years in product roles |
| **Key Quote** | *"I spend more time writing about the product than actually discovering what users need."* |

### Demographics

- Age 28-38, typically reports to VP/Director of Product
- Manages 1-3 products or feature areas
- Works with 2-5 engineers and 1-2 designers
- Often the sole PM or one of a small PM team

### Goals & Motivations

- Ship features that move metrics and delight users
- Be seen as strategic, not just a "ticket writer"
- Spend more time on discovery and less on documentation
- Build a track record that leads to Director/VP role
- Create PRDs that engineers actually want to read

### Frustrations & Pain Points

- PRDs take 2-3 days to write; stakeholders still ask "what does this mean?"
- Context gets lost in handoff — designers miss requirements, devs make wrong assumptions
- Template fatigue: every team wants different formats
- Switching between Notion, Figma, Jira, Slack — nothing connects
- AI chatbots help draft text but don't understand product structure

### Buying Triggers

- Just got staffed on a new initiative with a tight deadline
- New job/role and wants to establish credibility fast
- Got burned by a misaligned launch — requirements weren't clear
- Team scaling and needs more consistent documentation
- Heard about AI productivity tools from a peer or podcast

### Objections & Concerns

- "Will the AI output be generic or actually good?"
- "I don't want my team to think I'm not doing the work"
- "What if it doesn't match our company's PRD format?"
- "Security — where does my product data go?"
- "Another tool to learn and convince my team to use"

### Channels & Influences

- Lenny's Newsletter, Product Talk, Reforge
- LinkedIn PM communities, local PM meetups
- Twitter/X product influencers
- Peer recommendations carry heavy weight
- Tries tools that have free tiers before pitching to team

### Messaging Angles

| Priority | Message |
|----------|---------|
| 1 | **Time back:** "Go from idea to PRD in 30 minutes, not 3 days" |
| 2 | **Context preservation:** "Requirements that flow through to design and code" |
| 3 | **Quality improvement:** "AI coaching makes you a better PM, not a lazier one" |
| 4 | **Single source:** "Stop copy-pasting between 5 tools" |

### Day-in-the-Life Scenario

Maya gets pulled into a Monday morning meeting where the CEO shares a new product idea. By Tuesday, leadership wants a PRD for roadmap planning. She opens Braavo, describes the concept conversationally, and the AI asks clarifying questions about users, success metrics, and constraints. Within an hour she has a structured PRD. She refines it through a few more exchanges, generates user stories, and shares a link with her designer. The designer opens the same doc and generates wireframes directly from the user stories — no "can you explain this requirement?" Slack thread.

### Feature Prioritization

| Priority | Feature |
|----------|---------|
| Must-have | Chat-based PRD generation |
| Must-have | Template customization |
| High | AI coaching/improvement suggestions |
| High | Export to PDF/Markdown |
| Medium | Wireframe generation from requirements |
| Medium | Team comments and review workflows |
| Lower | Code scaffolding (hands off to devs) |

### Competitive Alternatives

| Alternative | Why They'd Use It |
|-------------|-------------------|
| Notion AI | Familiar, already in workflow, but no product-specific structure |
| ChatGPT/Claude | Flexible, but requires heavy prompting and copy-paste |
| Gamma/Tome | Pretty decks, but not PRD-focused |
| Productboard | Good for roadmaps, not document generation |

### Segment Variants

**Startup (seed to Series A):**
- Often wears multiple hats — PM + founder + support
- Less formal process, faster decisions
- Price-sensitive — needs free tier or low entry point
- Values speed over polish; "good enough" PRD is fine
- Might be the only stakeholder who needs to approve

**Mid-market (50-500 employees):**
- More structured process, PRDs go through review cycles
- Has dedicated designers and eng leads to hand off to
- Template consistency matters — teams need alignment
- Needs to justify tool purchases to manager
- Collaboration features become more important

### Validation Notes

- [ ] Do PMs actually spend 2-3 days on PRDs, or is this inflated?
- [ ] Is "AI coaching" a real desire or just nice-to-have?
- [ ] How important is Figma integration vs. built-in wireframes?
- [ ] What's the real objection — AI quality or looking lazy?

---

## Persona 2: Software Architect/Developer — "The Context-Starved Builder"

### Quick Reference

| Field | Value |
|-------|-------|
| **Name** | Marcus Rivera |
| **Title** | Senior Software Engineer / Tech Lead |
| **Experience** | 6-12 years in engineering |
| **Key Quote** | *"Just tell me what you actually want built — I'll figure out the how."* |

### Demographics

- Age 28-42, reports to Engineering Manager or CTO
- Leads technical decisions for 1-3 features or services
- Works closely with 1-2 PMs and a small dev team
- Often the one who translates PRD into technical spec

### Goals & Motivations

- Build systems that are clean, scalable, and maintainable
- Understand the "why" so they can make good tradeoffs
- Reduce time spent on boilerplate and scaffolding
- Ship without rework caused by requirement ambiguity
- Grow toward Staff/Principal or Architect role

### Frustrations & Pain Points

- PRDs lack technical context — "what does 'fast' mean?"
- Requirements change after implementation starts
- Spends hours writing API specs that could be generated
- Boilerplate setup for every new service/feature
- Meetings to clarify what should have been in the doc
- Designers hand off mockups with no spec for edge cases

### Buying Triggers

- Starting a greenfield project and wants to move fast
- Just inherited a messy codebase with no documentation
- Tired of writing OpenAPI specs by hand
- PM handed over a vague PRD and expects estimates by EOD
- Team adopted a new framework; needs scaffolding

### Objections & Concerns

- "Generated code is usually garbage I have to rewrite"
- "I don't trust AI to understand our architecture"
- "Will this lock us into weird patterns?"
- "Security — is it sending our code somewhere?"
- "I'd rather just write it myself than fix AI output"

### Channels & Influences

- Hacker News, Reddit (r/programming, r/ExperiencedDevs)
- Dev.to, personal blogs of engineers they respect
- GitHub trending, Twitter/X dev community
- Conference talks (QCon, StrangeLoop, local meetups)
- Strong influence from what their tech lead or architect endorses

### Messaging Angles

| Priority | Message |
|----------|---------|
| 1 | **Context preservation:** "Requirements flow into API specs — no game of telephone" |
| 2 | **Scaffold, don't generate:** "Clean starting points, not AI slop" |
| 3 | **Time on architecture, not boilerplate:** "Focus on the hard problems" |
| 4 | **Works with your stack:** "React, .NET, your design system — not generic code" |

### Day-in-the-Life Scenario

Marcus gets tagged on a PRD for a new notifications feature. Instead of scheduling a meeting to clarify requirements, he opens the Braavo doc and sees user stories with acceptance criteria. He clicks "Generate API Spec" and gets an OpenAPI contract based on the data entities implied by the requirements. He tweaks a few field names, generates a .NET project scaffold with FastEndpoints stubs, and starts implementing the actual logic. The PM later adds a requirement — Marcus regenerates the affected endpoint stub without touching his business logic.

### Feature Prioritization

| Priority | Feature |
|----------|---------|
| Must-have | API contract generation (OpenAPI) |
| Must-have | Code scaffolding (.NET, React) |
| High | Database schema / ERD generation |
| High | Requirements with clear acceptance criteria |
| Medium | Architecture diagrams (system context, sequence) |
| Medium | Integration with GitHub |
| Lower | Wireframes (that's design's job) |

### Competitive Alternatives

| Alternative | Why They'd Use It |
|-------------|-------------------|
| ChatGPT/Claude | Good for snippets, but no project context |
| GitHub Copilot | Great for inline, but doesn't do architecture |
| Swagger/OpenAPI generators | Manual, no requirements link |
| JetBrains AI / Cursor | IDE-focused, not requirements-driven |
| Hand-written | "I'll just do it myself" |

### Segment Variants

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

### Validation Notes

- [ ] Is API spec generation actually valued, or do devs prefer hand-writing?
- [ ] What's the threshold for "good enough" scaffolding quality?
- [ ] Do devs want this from the PRD tool, or separate?
- [ ] Is security concern real or reflexive?

---

## Persona 3: UI/UX Designer — "The Requirements Translator"

### Quick Reference

| Field | Value |
|-------|-------|
| **Name** | Jordan Park |
| **Title** | Product Designer / UI/UX Designer |
| **Experience** | 3-7 years in design |
| **Key Quote** | *"I can't design what I don't understand — give me the 'why,' not just a feature list."* |

### Demographics

- Age 26-36, reports to Design Lead or Head of Product
- Owns design for 1-3 product areas or feature teams
- Partners with 1-2 PMs and collaborates with engineers on feasibility
- Figma is home base; lives in design tools daily

### Goals & Motivations

- Create designs that solve real user problems
- Understand requirements deeply before pushing pixels
- Reduce back-and-forth with PM over "what did you mean?"
- Build a portfolio of shipped work that shows impact
- Move toward Lead Designer or Head of Design

### Frustrations & Pain Points

- PRDs are walls of text with buried UI implications
- Requirements change after designs are "final"
- Has to reverse-engineer user flows from feature lists
- Edge cases surface during dev, not during design
- Wireframing from scratch for every project
- Engineers build something that doesn't match the design

### Buying Triggers

- New project kicked off with a vague PRD and tight timeline
- Just joined a team with no design system or templates
- Got blamed for a launch miss that was really a requirements gap
- Looking for tools that connect requirements to design artifacts
- Heard about AI design tools and wants to experiment

### Objections & Concerns

- "AI-generated designs look generic and template-y"
- "Will this replace my job or make me look replaceable?"
- "I already have Figma — why do I need another tool?"
- "Does it understand our design system, or just Bootstrap?"
- "Designers should design, not machines"

### Channels & Influences

- Figma Community, Dribbble, Behance
- Design podcasts (Design Details, Honest Designers)
- Twitter/X design community, LinkedIn design leaders
- Smashing Magazine, Nielsen Norman Group articles
- Peer recommendations from other designers

### Messaging Angles

| Priority | Message |
|----------|---------|
| 1 | **Context, not just pixels:** "See the requirements behind every screen" |
| 2 | **Wireframes as starting points:** "Skip the blank canvas — iterate from structure" |
| 3 | **Edge cases surfaced early:** "Know what to design before you design it" |
| 4 | **Figma integration:** "Push wireframes to Figma, keep your workflow" |

### Day-in-the-Life Scenario

Jordan gets added to a PRD for a new onboarding flow. Instead of reading 10 pages and asking the PM "so what screens do we need?", she opens Braavo and sees user stories mapped to a flow diagram. She clicks "Generate Wireframes" and gets low-fidelity screens for each step — not pretty, but structurally sound. She exports to Figma and starts applying the design system, knowing she's not missing any states. When the PM adds a "skip onboarding" option, Braavo flags the affected screens so Jordan knows what to update.

### Feature Prioritization

| Priority | Feature |
|----------|---------|
| Must-have | Wireframe generation from user stories |
| Must-have | Figma export/integration |
| High | User flow diagrams from requirements |
| High | Multi-fidelity output (low-fi to high-fi) |
| Medium | Design system integration (Tailwind, MUI) |
| Medium | Comments and review workflows |
| Lower | Code generation (that's dev's job) |

### Competitive Alternatives

| Alternative | Why They'd Use It |
|-------------|-------------------|
| Figma AI | Native to workflow, but limited generation |
| Uizard | AI wireframes, but no requirements connection |
| Relume | Good for marketing sites, less for product |
| Whimsical | Flowcharts and wireframes, no AI |
| Sketch + manual | "I'll just do it myself faster" |

### Segment Variants

**Startup (seed to Series A):**
- Often the only designer — does UX, UI, and some brand
- Speed matters; "good enough" wireframes are fine
- May not have a formal design system yet
- More autonomy, less review process
- Values free tier; budget is tight

**Mid-market (50-500 employees):**
- Part of a design team with established systems
- Needs to match existing component libraries
- Designs go through review with Design Lead
- Cares about handoff quality to dev
- Collaboration features matter more

### Validation Notes

- [ ] Do designers want wireframes generated, or is that "their job"?
- [ ] How important is Figma bidirectional sync vs. one-way export?
- [ ] Is "AI will replace me" fear real or overblown?
- [ ] What fidelity level is actually useful — low-fi or high-fi?

---

## Persona 4: Team Lead/QA — "The Quality Gatekeeper"

### Quick Reference

| Field | Value |
|-------|-------|
| **Name** | Priya Sharma |
| **Title** | QA Lead / Engineering Team Lead |
| **Experience** | 5-10 years in QA or engineering leadership |
| **Key Quote** | *"If I had clearer requirements upfront, I wouldn't be finding these bugs in production."* |

### Demographics

- Age 30-45, reports to Engineering Manager or Director
- Oversees quality for 1-3 product teams
- Reviews PRDs for testability, coordinates releases
- May manage 2-4 QA engineers or wear both lead and IC hats

### Goals & Motivations

- Catch defects before they hit production
- Ship releases confidently without last-minute chaos
- Reduce ambiguity that causes "is this a bug or a feature?" debates
- Build test coverage that scales with the product
- Move toward QA Director or transition to Engineering Management

### Frustrations & Pain Points

- PRDs don't include acceptance criteria — has to guess expected behavior
- Test cases written from scratch for every feature
- Edge cases discovered during testing, not requirements
- "Works as designed" debates when requirements were unclear
- Manual test planning is tedious and time-consuming
- No traceability from requirement to test to bug

### Buying Triggers

- Major bug slipped to production due to unclear requirements
- New feature launch with compressed timeline
- Team scaling and needs consistent test documentation
- Audit/compliance requires traceability
- Heard about AI test generation and wants to evaluate

### Objections & Concerns

- "Generated test cases are shallow — they miss the real edge cases"
- "I still need to review everything anyway"
- "Does it integrate with our test management tool?"
- "Another tool to learn when I'm already stretched thin"
- "Will devs and PMs actually use this, or just QA?"

### Channels & Influences

- Ministry of Testing, QA community Slack/Discord
- LinkedIn QA/testing groups
- Testing conferences (Agile Testing Days, TestBash)
- Peer QA leads at other companies
- What the engineering org standardizes on

### Messaging Angles

| Priority | Message |
|----------|---------|
| 1 | **Testable requirements:** "Acceptance criteria built in, not bolted on" |
| 2 | **Test cases from user stories:** "Generate test plans in minutes" |
| 3 | **Traceability:** "Requirement → test → defect, all connected" |
| 4 | **Edge cases surfaced:** "AI asks the 'what if' questions PMs forget" |

### Day-in-the-Life Scenario

Priya joins a PRD review meeting. Instead of asking "where are the acceptance criteria?", she opens Braavo and sees each user story with testable conditions already defined. She clicks "Generate Test Plan" and gets a structured set of test cases — happy path, edge cases, error scenarios. She adds a few domain-specific cases the AI missed, exports to her test management tool, and assigns to her team. When a bug is filed later, she traces it back to the requirement that lacked clarity — and flags it for the PM to tighten.

### Feature Prioritization

| Priority | Feature |
|----------|---------|
| Must-have | Acceptance criteria on user stories |
| Must-have | Test case generation from requirements |
| High | Edge case and error scenario suggestions |
| High | Export to test management tools |
| Medium | Review workflows with approval gates |
| Medium | Requirements traceability |
| Lower | Wireframes and code generation (other roles) |

### Competitive Alternatives

| Alternative | Why They'd Use It |
|-------------|-------------------|
| TestRail / Zephyr | Test management, but no generation |
| ChatGPT/Claude | Can draft test cases, no structure or traceability |
| Cucumber/Gherkin | BDD format, but manual authoring |
| PractiTest | Good traceability, no AI generation |
| Spreadsheets | "We've always done it this way" |

### Segment Variants

**Startup (seed to Series A):**
- QA is often a dev responsibility — no dedicated QA role
- Testing is informal; "we'll catch it in staging"
- Values anything that adds structure without slowing down
- Less concerned with traceability and audit trails
- May be the Team Lead who also owns quality

**Mid-market (50-500 employees):**
- Dedicated QA team or at least a QA lead
- More formal release processes and sign-offs
- Compliance/audit requirements may exist
- Integrations with existing test tools matter
- Team coordination features become important

### Validation Notes

- [ ] Is test case generation genuinely wanted, or is review burden the concern?
- [ ] How important is integration with TestRail/Zephyr vs. export?
- [ ] Do QA leads have influence to adopt tools, or do PMs/Eng drive?
- [ ] Is traceability a real need or a nice-to-have?

---

## Cross-Persona Insights

### Buying Motion by Persona

| Persona | Role in Purchase |
|---------|------------------|
| Maya (PM) | **Champion** — discovers tool, drives adoption, justifies purchase |
| Marcus (Dev) | **Influencer** — validates technical credibility, can block or boost |
| Jordan (Designer) | **User** — adopts if it fits workflow, vocal if it doesn't |
| Priya (QA) | **Beneficiary** — gains most from connected requirements, least likely to drive purchase |

### Feature Overlap Matrix

| Feature | Maya | Marcus | Jordan | Priya |
|---------|------|--------|--------|-------|
| Chat-based PRD generation | Must-have | - | - | - |
| Template customization | Must-have | - | - | - |
| API contract generation | - | Must-have | - | - |
| Code scaffolding | Lower | Must-have | - | - |
| Wireframe generation | Medium | Lower | Must-have | - |
| Figma integration | - | - | Must-have | - |
| Acceptance criteria | High | High | - | Must-have |
| Test case generation | - | - | - | Must-have |
| Team collaboration | Medium | Medium | Medium | Medium |
| Export (PDF/Markdown) | High | Medium | Medium | High |

### MVP Priority (based on buying motion)

1. **PRD generation** — Maya is the champion; this is table stakes
2. **Acceptance criteria** — benefits Marcus and Priya; improves handoff quality
3. **Template customization** — Maya needs this to match company formats
4. **Export** — all personas need to get content out of the tool
5. **Wireframes** — differentiator that brings Jordan into the workflow
6. **API/Code generation** — brings Marcus in; requires PRD quality to be high first

---

## Appendix: Validation Research Plan

### Priority 1: Product Manager (Maya)

**Questions to answer:**
- How long does PRD creation actually take?
- What's the real blocker — time, quality, or consistency?
- Is AI-generated content a trust issue?

**Methods:**
- 5-8 user interviews with PMs at startups and mid-market
- Survey on PRD creation time and pain points
- Show prototype, measure reaction to AI output quality

### Priority 2: Developer (Marcus)

**Questions to answer:**
- Do devs want API specs from the PRD tool?
- What scaffolding quality is "good enough"?
- Is security a real concern or reflexive objection?

**Methods:**
- 3-5 interviews with tech leads / senior engineers
- Code review of generated output by engineering advisors
- Competitive analysis of Copilot, Cursor, etc.

### Priority 3: Designer (Jordan)

**Questions to answer:**
- Is wireframe generation welcome or threatening?
- What fidelity level is useful?
- How critical is Figma integration?

**Methods:**
- 3-5 interviews with product designers
- Figma plugin prototype test
- Review Uizard, Relume user sentiment

### Priority 4: QA Lead (Priya)

**Questions to answer:**
- Is test generation valued if review is still required?
- How important is tool integration vs. export?
- Who drives QA tool adoption — QA or Engineering?

**Methods:**
- 3-5 interviews with QA leads
- Survey on test documentation practices
- Competitive analysis of TestRail, PractiTest
