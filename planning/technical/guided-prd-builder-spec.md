# Guided PRD Builder - Design Specification

## Overview

Transform Braavo from a chat-based PRD generator into an interactive, guided PRD builder that walks users through each section of a professional Product Requirements Document with AI assistance at every step.

## Design Principles

1. **Progressive Disclosure** - Show only what's needed at each step
2. **AI as Assistant, Not Author** - AI suggests and helps refine; user maintains control
3. **Visual First** - Display content as cards, diagrams, and visual elements, not walls of text
4. **Non-Linear Navigation** - Users can jump between sections, not locked to a wizard
5. **Always Exportable** - Full PRD document available at any stage

---

## User Flows

### Flow 1: Welcome & Product Creation

```
┌─────────────────────────────────────────────────────────────────┐
│                         Welcome Screen                          │
│                                                                 │
│   🚀 Welcome to Braavo                                          │
│   Your AI-powered product development assistant                 │
│                                                                 │
│   ┌─────────────────────┐  ┌─────────────────────┐             │
│   │  + New Product      │  │  📁 My Products     │             │
│   │  Start building     │  │  Continue working   │             │
│   └─────────────────────┘  └─────────────────────┘             │
│                                                                 │
│   Recent Products:                                              │
│   ┌──────────────────────────────────────────────┐             │
│   │ TaskFlow - 65% complete - Last edited 2h ago │             │
│   │ DataSync - 30% complete - Last edited 1d ago │             │
│   └──────────────────────────────────────────────┘             │
└─────────────────────────────────────────────────────────────────┘
```

### Flow 2: New Product Setup

```
┌─────────────────────────────────────────────────────────────────┐
│  Step 1 of 2: Tell us about your product                        │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  Product Name *                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ TaskFlow                                                 │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  Describe your product idea                                     │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ A task management app for remote teams with Kanban      │   │
│  │ boards, real-time collaboration, and integrations       │   │
│  │ with Slack and GitHub...                                │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  Product Category                                               │
│  [SaaS ▼] [Productivity ▼] [+ Add tag]                         │
│                                                                 │
│                                    [Back] [Continue →]          │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│  Step 2 of 2: Choose your starting point                        │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  ┌────────────────────┐  ┌────────────────────┐                │
│  │  🪄 AI Jumpstart   │  │  📝 Start Blank    │                │
│  │                    │  │                    │                │
│  │  Let AI generate   │  │  Build each        │                │
│  │  a draft PRD based │  │  section from      │                │
│  │  on your idea      │  │  scratch           │                │
│  │                    │  │                    │                │
│  │  [Select]          │  │  [Select]          │                │
│  └────────────────────┘  └────────────────────┘                │
│                                                                 │
│  ┌────────────────────┐                                        │
│  │  📋 Use Template   │                                        │
│  │                    │                                        │
│  │  Start from a      │   Templates:                           │
│  │  pre-built         │   • SaaS Application                   │
│  │  template          │   • Mobile App                         │
│  │                    │   • E-commerce Platform                │
│  │  [Select]          │   • API/Platform                       │
│  └────────────────────┘                                        │
│                                    [Back] [Create Product →]    │
└─────────────────────────────────────────────────────────────────┘
```

### Flow 3: PRD Builder Main Interface

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  TaskFlow PRD                                              [Preview] [Export]│
├────────────────┬─────────────────────────────────────────────────────────────┤
│                │                                                             │
│  PRD Sections  │   📋 Overview                                    [Edit]     │
│  ────────────  │   ─────────────────────────────────────────────────────    │
│                │                                                             │
│  ✓ Overview    │   Vision                                                    │
│  ✓ Personas    │   ┌────────────────────────────────────────────────────┐   │
│  → User Stories│   │ TaskFlow empowers remote teams to collaborate      │   │
│    Features    │   │ seamlessly through intuitive task management...    │   │
│    Technical   │   └────────────────────────────────────────────────────┘   │
│    Design      │                                                             │
│    Metrics     │   Problem Statement                                         │
│    Timeline    │   ┌────────────────────────────────────────────────────┐   │
│                │   │ Remote teams struggle with disconnected tools,     │   │
│  ────────────  │   │ leading to missed deadlines and poor visibility... │   │
│                │   └────────────────────────────────────────────────────┘   │
│  📊 Diagrams   │                                                             │
│  📦 Export     │   Target Market                                             │
│                │   ┌──────────┐ ┌──────────┐ ┌──────────┐                   │
│                │   │ Remote   │ │ Mid-size │ │ Tech     │                   │
│                │   │ Teams    │ │ Companies│ │ Startups │                   │
│                │   └──────────┘ └──────────┘ └──────────┘                   │
│                │                                                             │
│  ────────────  │   ┌─────────────────────────────────────────────────────┐  │
│  Completion    │   │ 💡 AI Assistant                                     │  │
│  ████████░░    │   │                                                     │  │
│  65%           │   │ Your overview looks good! Consider adding:          │  │
│                │   │ • Specific market size data                         │  │
│                │   │ • Key differentiators from competitors              │  │
│                │   │                                                     │  │
│                │   │ [Refine with AI] [Add market data]                  │  │
│                │   └─────────────────────────────────────────────────────┘  │
│                │                                                             │
│                │                              [← Prev Section] [Next →]      │
└────────────────┴─────────────────────────────────────────────────────────────┘
```

---

## Section Designs

### Section 1: Overview

**Purpose:** Establish the product vision, problem, and market context.

**Data Structure:**
```typescript
interface Overview {
  vision: string;           // 1-2 paragraph vision statement
  problemStatement: string; // Core problem being solved
  targetMarket: string[];   // Market segments
  valueProposition: string; // Unique value
  businessGoals: string[];  // High-level business objectives
}
```

**UI Components:**
- Rich text editors for vision and problem
- Tag input for target market segments
- Bullet list builder for business goals
- AI suggestion panel

**AI Assistance:**
- "Refine my vision statement"
- "Suggest target market segments"
- "What problems does this solve?"

---

### Section 2: Personas

**Purpose:** Define who the users are with detailed profiles.

**Data Structure:**
```typescript
interface Persona {
  id: string;
  name: string;           // "Sarah the Team Lead"
  role: string;           // "Team Lead at mid-size company"
  avatar?: string;        // Generated or selected avatar
  demographics: {
    age?: string;
    location?: string;
    technicalLevel: 'low' | 'medium' | 'high';
  };
  goals: string[];        // What they want to achieve
  painPoints: string[];   // Current frustrations
  motivations: string[];  // What drives them
  behaviors: string[];    // How they currently work
  quote?: string;         // Representative quote
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  👥 User Personas                                    [+ Add]    │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  ┌───────────────────────┐  ┌───────────────────────┐          │
│  │  👩‍💼 Sarah             │  │  👨‍💻 Mike              │          │
│  │  Team Lead            │  │  Developer            │          │
│  │  ──────────────────   │  │  ──────────────────   │          │
│  │  Goals:               │  │  Goals:               │          │
│  │  • Track team tasks   │  │  • Focus on coding    │          │
│  │  • Identify blockers  │  │  • Quick task lookup  │          │
│  │                       │  │                       │          │
│  │  Pain Points:         │  │  Pain Points:         │          │
│  │  • No visibility      │  │  • Context switching  │          │
│  │  • Manual status      │  │  • Too many meetings  │          │
│  │                       │  │                       │          │
│  │  Tech: ████████░░     │  │  Tech: ██████████     │          │
│  │                       │  │                       │          │
│  │  [Edit] [Delete]      │  │  [Edit] [Delete]      │          │
│  └───────────────────────┘  └───────────────────────┘          │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ 💡 Describe a user and I'll create a persona            │   │
│  │ ┌─────────────────────────────────────────────────────┐ │   │
│  │ │ A busy project manager who needs to track multiple  │ │   │
│  │ │ projects across different teams...                  │ │   │
│  │ └─────────────────────────────────────────────────────┘ │   │
│  │                                      [Generate Persona] │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

**Persona Card Edit Modal:**
```
┌─────────────────────────────────────────────────────────────────┐
│  Edit Persona                                            [×]    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────┐  Name: [Sarah                    ]                     │
│  │ 👩‍💼 │  Role: [Team Lead at mid-size co ]                     │
│  └─────┘                                                        │
│  [Change Avatar]                                                │
│                                                                 │
│  Technical Comfort                                              │
│  ○ Low   ● Medium   ○ High                                      │
│                                                                 │
│  Goals                                           [+ Add]        │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ • Track team progress across projects              [×]  │   │
│  │ • Identify blockers before they become problems    [×]  │   │
│  │ • Generate reports for stakeholders                [×]  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  Pain Points                                     [+ Add]        │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ • No single source of truth for task status        [×]  │   │
│  │ • Manually chasing team for updates                [×]  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  Representative Quote                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ "I spend more time tracking tasks than actually        │   │
│  │  helping my team succeed."                             │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  [🪄 AI: Suggest goals] [🪄 AI: Suggest pain points]           │
│                                                                 │
│                                    [Cancel] [Save Persona]      │
└─────────────────────────────────────────────────────────────────┘
```

---

### Section 3: User Stories

**Purpose:** Define what users want to accomplish in the "As a... I want... So that..." format.

**Data Structure:**
```typescript
interface UserStory {
  id: string;
  personaId: string;      // Links to a persona
  asA: string;            // User type
  iWant: string;          // Goal/action
  soThat: string;         // Benefit/value
  acceptanceCriteria: string[];
  priority: 'must' | 'should' | 'could' | 'wont';  // MoSCoW
  linkedFeatures: string[];  // Feature IDs this story drives
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  📖 User Stories                                    [+ Add]     │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  Filter by Persona: [All ▼]    Priority: [All ▼]               │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  MUST HAVE                                              │   │
│  │  ─────────                                              │   │
│  │  ┌───────────────────────────────────────────────────┐  │   │
│  │  │ 👩‍💼 Sarah                                          │  │   │
│  │  │ As a team lead, I want to see all my team's       │  │   │
│  │  │ tasks in one dashboard so that I can quickly      │  │   │
│  │  │ identify blockers.                                │  │   │
│  │  │                                                   │  │   │
│  │  │ Acceptance Criteria:                              │  │   │
│  │  │ ✓ Dashboard shows tasks grouped by team member    │  │   │
│  │  │ ✓ Blocked tasks are visually highlighted          │  │   │
│  │  │ ✓ Can filter by date range                        │  │   │
│  │  │                                                   │  │   │
│  │  │ Links to: Dashboard, Task Filtering         [Edit]│  │   │
│  │  └───────────────────────────────────────────────────┘  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  SHOULD HAVE                                            │   │
│  │  ─────────                                              │   │
│  │  ┌───────────────────────────────────────────────────┐  │   │
│  │  │ 👨‍💻 Mike                                           │  │   │
│  │  │ As a developer, I want to update task status      │  │   │
│  │  │ from my IDE so that I don't have to switch        │  │   │
│  │  │ contexts.                                         │  │   │
│  │  └───────────────────────────────────────────────────┘  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ 💡 Generate stories for persona:                        │   │
│  │ [Sarah ▼]                         [Generate Stories]    │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

### Section 4: Features

**Purpose:** Define product capabilities linked to user stories, prioritized by phase.

**Data Structure:**
```typescript
interface Feature {
  id: string;
  name: string;
  description: string;
  phase: 'mvp' | 'enhanced' | 'future';
  priority: number;       // Order within phase
  linkedStories: string[];
  subFeatures?: Feature[];
  estimatedEffort?: 'small' | 'medium' | 'large';
  dependencies?: string[];  // Other feature IDs
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  🎯 Features & Requirements                          [+ Add]    │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  ┌─────────────────┬─────────────────┬─────────────────┐       │
│  │   MVP (Phase 1) │ Enhanced (Ph 2) │  Future (Ph 3)  │       │
│  ├─────────────────┼─────────────────┼─────────────────┤       │
│  │                 │                 │                 │       │
│  │ ┌─────────────┐ │ ┌─────────────┐ │ ┌─────────────┐ │       │
│  │ │ Dashboard   │ │ │ Slack       │ │ │ AI Insights │ │       │
│  │ │ ─────────── │ │ │ Integration │ │ │ ─────────── │ │       │
│  │ │ Team task   │ │ │ ─────────── │ │ │ Predictive  │ │       │
│  │ │ overview    │ │ │ Sync tasks  │ │ │ analytics   │ │       │
│  │ │             │ │ │ with Slack  │ │ │             │ │       │
│  │ │ Stories: 3  │ │ │             │ │ │ Stories: 1  │ │       │
│  │ │ Effort: M   │ │ │ Stories: 2  │ │ │ Effort: L   │ │       │
│  │ └─────────────┘ │ │ Effort: M   │ │ └─────────────┘ │       │
│  │                 │ └─────────────┘ │                 │       │
│  │ ┌─────────────┐ │                 │ ┌─────────────┐ │       │
│  │ │ Kanban      │ │ ┌─────────────┐ │ │ Mobile App  │ │       │
│  │ │ Board       │ │ │ GitHub      │ │ └─────────────┘ │       │
│  │ │ ─────────── │ │ │ Integration │ │                 │       │
│  │ │ Drag-drop   │ │ └─────────────┘ │                 │       │
│  │ │ task mgmt   │ │                 │                 │       │
│  │ └─────────────┘ │                 │                 │       │
│  │                 │                 │                 │       │
│  │ ┌─────────────┐ │                 │                 │       │
│  │ │ Auth & SSO  │ │                 │                 │       │
│  │ └─────────────┘ │                 │                 │       │
│  │                 │                 │                 │       │
│  │   [+ Add]       │   [+ Add]       │   [+ Add]       │       │
│  └─────────────────┴─────────────────┴─────────────────┘       │
│                                                                 │
│  Drag features between phases to reprioritize                   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ 💡 AI Suggestions based on your user stories:           │   │
│  │                                                         │   │
│  │ Missing features that could address your stories:       │   │
│  │ • Real-time notifications (supports 2 stories)          │   │
│  │ • Search & filtering (supports 3 stories)               │   │
│  │                                                         │   │
│  │ [Add Real-time notifications] [Add Search & filtering]  │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

### Section 5: Technical Requirements

**Purpose:** Define platforms, integrations, performance, security, and constraints.

**Data Structure:**
```typescript
interface TechnicalRequirements {
  platforms: Platform[];
  integrations: Integration[];
  performance: PerformanceReq[];
  security: SecurityReq[];
  constraints: string[];
  infrastructure: string[];
}

interface Platform {
  type: 'web' | 'ios' | 'android' | 'desktop' | 'api';
  details: string;
  priority: 'required' | 'optional';
}

interface Integration {
  name: string;
  type: 'oauth' | 'api' | 'webhook' | 'sdk';
  description: string;
  linkedFeatures: string[];
}

interface PerformanceReq {
  metric: string;       // "Page load time"
  target: string;       // "< 2 seconds"
  measurement: string;  // "P95 latency"
}

interface SecurityReq {
  category: 'auth' | 'data' | 'compliance' | 'infrastructure';
  requirement: string;
  priority: 'must' | 'should';
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  ⚙️ Technical Requirements                                      │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  Platforms                                           [+ Add]    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ ✓ Web (Responsive)     Required                         │  │
│  │ ✓ iOS App              Optional - Phase 2               │  │
│  │ ✓ Android App          Optional - Phase 2               │  │
│  │ ○ Desktop App          Not planned                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  Integrations                                        [+ Add]    │
│  ┌────────────────┐ ┌────────────────┐ ┌────────────────┐      │
│  │ 🔗 Slack       │ │ 🔗 GitHub      │ │ 🔗 Jira        │      │
│  │ OAuth + API    │ │ Webhooks       │ │ REST API       │      │
│  │ Phase 2        │ │ Phase 2        │ │ Phase 3        │      │
│  └────────────────┘ └────────────────┘ └────────────────┘      │
│                                                                 │
│  Performance Targets                                 [+ Add]    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Metric              │ Target        │ Measurement        │  │
│  ├─────────────────────┼───────────────┼────────────────────┤  │
│  │ Page load           │ < 2 seconds   │ P95 latency        │  │
│  │ API response        │ < 200ms       │ P99 latency        │  │
│  │ Uptime              │ 99.9%         │ Monthly            │  │
│  │ Concurrent users    │ 10,000        │ Peak load          │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  Security & Compliance                               [+ Add]    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ ✓ SSO / SAML support                            Must     │  │
│  │ ✓ End-to-end encryption                         Must     │  │
│  │ ✓ SOC 2 Type II compliance                      Should   │  │
│  │ ✓ GDPR compliance                               Must     │  │
│  │ ✓ MFA support                                   Must     │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ 💡 Based on your features, consider adding:             │   │
│  │ • WebSocket support for real-time collaboration         │   │
│  │ • Rate limiting for API endpoints                       │   │
│  │                           [Add to requirements]         │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

### Section 6: Design & UX

**Purpose:** Define user experience guidelines, key screens, and accessibility requirements.

**Data Structure:**
```typescript
interface DesignRequirements {
  keyScreens: Screen[];
  navigationPattern: string;
  designPrinciples: string[];
  accessibility: AccessibilityReq[];
  brandGuidelines?: {
    colors?: string[];
    typography?: string;
    tone?: string;
  };
}

interface Screen {
  name: string;
  description: string;
  wireframe?: string;  // Generated wireframe ID
  linkedFeatures: string[];
}

interface AccessibilityReq {
  standard: 'WCAG 2.1 AA' | 'WCAG 2.1 AAA' | 'Section 508';
  requirements: string[];
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  🎨 Design & UX                                                 │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  Key Screens                                         [+ Add]    │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────┐            │
│  │ ┌──────────┐ │ │ ┌──────────┐ │ │ ┌──────────┐ │            │
│  │ │          │ │ │ │          │ │ │ │          │ │            │
│  │ │ Dashboard│ │ │ │  Kanban  │ │ │ │ Settings │ │            │
│  │ │          │ │ │ │  Board   │ │ │ │          │ │            │
│  │ └──────────┘ │ │ └──────────┘ │ │ └──────────┘ │            │
│  │  Dashboard   │ │  Task Board  │ │  Settings    │            │
│  │  [Wireframe] │ │  [Wireframe] │ │  [Wireframe] │            │
│  └──────────────┘ └──────────────┘ └──────────────┘            │
│                                                                 │
│  Navigation Pattern                                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ ● Top navigation with sidebar                            │  │
│  │ ○ Bottom tabs (mobile-first)                             │  │
│  │ ○ Hamburger menu                                         │  │
│  │ ○ Command palette (keyboard-first)                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  Design Principles                                   [+ Add]    │
│  • Clarity over cleverness                                      │
│  • Progressive disclosure                                       │
│  • Consistent feedback for actions                              │
│  • Mobile-responsive by default                                 │
│                                                                 │
│  Accessibility                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ ✓ WCAG 2.1 AA compliance                                │  │
│  │ ✓ Keyboard navigation support                           │  │
│  │ ✓ Screen reader compatibility                           │  │
│  │ ✓ Color contrast ratios (4.5:1 minimum)                 │  │
│  │ ✓ Focus indicators                                       │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  [🪄 Generate Wireframe for: Dashboard ▼]                      │
└─────────────────────────────────────────────────────────────────┘
```

---

### Section 7: Metrics & Success Criteria

**Purpose:** Define measurable KPIs with targets and timeframes.

**Data Structure:**
```typescript
interface Metric {
  id: string;
  category: 'adoption' | 'engagement' | 'retention' | 'business' | 'technical';
  name: string;
  description: string;
  target: string;
  timeframe: string;
  measurement: string;  // How it will be measured
  linkedFeatures?: string[];
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  📊 Metrics & Success Criteria                       [+ Add]    │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  ADOPTION                                               │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │ 📈 User Registration Rate                       │   │   │
│  │  │    Target: 1,000 users     Timeframe: 3 months  │   │   │
│  │  │    Measured by: Unique sign-ups                 │   │   │
│  │  │    Progress: ████████░░░░░░ 65%                 │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │ 📈 Team Adoption                                │   │   │
│  │  │    Target: 50 teams        Timeframe: 6 months  │   │   │
│  │  │    Measured by: Teams with 3+ active members    │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  ENGAGEMENT                                             │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │ 📈 Daily Active Users (DAU)                     │   │   │
│  │  │    Target: 500 DAU         Timeframe: 6 months  │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │ 📈 Task Completion Rate                         │   │   │
│  │  │    Target: 85%             Timeframe: Ongoing   │   │   │
│  │  │    Measured by: Tasks marked complete / created │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  BUSINESS                                               │   │
│  │  ┌─────────────────────────────────────────────────┐   │   │
│  │  │ 📈 Monthly Recurring Revenue (MRR)              │   │   │
│  │  │    Target: $10,000         Timeframe: 12 months │   │   │
│  │  └─────────────────────────────────────────────────┘   │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ 💡 Suggested metrics based on your features:            │   │
│  │ • Feature adoption rate per feature                     │   │
│  │ • Time to first task creation                           │   │
│  │ • Integration usage (Slack/GitHub)                      │   │
│  │                               [Add suggested metrics]   │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

### Section 8: Timeline & Milestones

**Purpose:** Define development phases with deliverables and dates.

**Data Structure:**
```typescript
interface Timeline {
  phases: Phase[];
}

interface Phase {
  id: string;
  name: string;           // "MVP", "Phase 2", etc.
  startDate?: Date;
  endDate?: Date;
  duration?: string;      // "8 weeks" if dates not set
  milestones: Milestone[];
  deliverables: string[];
  dependencies?: string[]; // Other phase IDs
}

interface Milestone {
  id: string;
  name: string;
  date?: Date;
  week?: number;          // Week number within phase
  deliverables: string[];
  status: 'planned' | 'in-progress' | 'completed';
}
```

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  📅 Timeline & Milestones                                       │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  View: [Gantt ▼]  [Timeline]  [List]                           │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │           W1   W2   W3   W4   W5   W6   W7   W8        │   │
│  │  MVP      ████████████████████████████████████         │   │
│  │  ├─ Setup ████                                         │   │
│  │  ├─ Core       ████████████████                        │   │
│  │  ├─ Testing                    ████████                │   │
│  │  └─ Launch                             ██              │   │
│  │                                                        │   │
│  │           W9   W10  W11  W12  W13  W14                 │   │
│  │  Phase 2       ████████████████████████                │   │
│  │  ├─ Slack          ████████                            │   │
│  │  ├─ GitHub              ████████                       │   │
│  │  └─ Mobile                   ████████████              │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  Phase Details                                                  │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │  📦 MVP (Phase 1)                           8 weeks     │   │
│  │  ──────────────────────────────────────────────────────│   │
│  │                                                         │   │
│  │  Milestones:                                            │   │
│  │  ○ Week 2: Architecture complete                        │   │
│  │  ○ Week 5: Core features implemented                    │   │
│  │  ○ Week 7: Testing complete                             │   │
│  │  ○ Week 8: Beta launch                                  │   │
│  │                                                         │   │
│  │  Deliverables:                                          │   │
│  │  • Dashboard with team task view                        │   │
│  │  • Kanban board with drag-drop                          │   │
│  │  • User authentication with SSO                         │   │
│  │  • Real-time notifications                              │   │
│  │                                                         │   │
│  │  Features included: Dashboard, Kanban, Auth, Notifs     │   │
│  │                                            [Edit Phase] │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  [+ Add Phase]                                                  │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ 💡 AI: Your timeline looks ambitious. Based on the     │   │
│  │ feature scope, consider extending MVP to 10 weeks or   │   │
│  │ moving Mobile App to Phase 3.                          │   │
│  └─────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

---

### Diagrams Section

**Purpose:** Auto-generate and display visual diagrams from PRD content.

**Diagram Types:**
1. **User Journey Map** - Flow from persona through key scenarios
2. **Feature Hierarchy** - Features organized by phase/priority
3. **System Architecture** - Technical components and integrations
4. **Entity Relationship** - Data model visualization
5. **Timeline View** - Gantt-style project timeline

**UI Design:**
```
┌─────────────────────────────────────────────────────────────────┐
│  📊 Diagrams                                                    │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  Auto-generated from your PRD content                           │
│                                                                 │
│  ┌────────────────┐ ┌────────────────┐ ┌────────────────┐      │
│  │ 👤 User        │ │ 🏗️ Feature     │ │ 🔧 System      │      │
│  │ Journey        │ │ Hierarchy      │ │ Architecture   │      │
│  │                │ │                │ │                │      │
│  │ [Generate]     │ │ [Generate]     │ │ [Generate]     │      │
│  └────────────────┘ └────────────────┘ └────────────────┘      │
│                                                                 │
│  ┌────────────────┐ ┌────────────────┐                         │
│  │ 📐 Data        │ │ 📅 Timeline    │                         │
│  │ Model          │ │ Gantt          │                         │
│  │                │ │                │                         │
│  │ [Generate]     │ │ [Generate]     │                         │
│  └────────────────┘ └────────────────┘                         │
│                                                                 │
│  ─────────────────────────────────────────────────────────────  │
│                                                                 │
│  User Journey: Sarah completing a task review                   │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                                                         │   │
│  │  ┌─────┐    ┌─────────┐    ┌─────────┐    ┌─────────┐  │   │
│  │  │Login│───▶│Dashboard│───▶│Task List│───▶│ Review  │  │   │
│  │  └─────┘    └─────────┘    └─────────┘    └─────────┘  │   │
│  │                  │              │              │        │   │
│  │                  ▼              ▼              ▼        │   │
│  │             See team      Filter by      Mark tasks    │   │
│  │             overview      status         complete      │   │
│  │                                                         │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  [Download PNG] [Download SVG] [Copy Mermaid]                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Data Model

### Core Entities

```typescript
// Product - The main container
interface Product {
  id: string;
  userId: string;
  name: string;
  description: string;
  category: string[];
  status: 'draft' | 'in-progress' | 'review' | 'final';
  completionPercentage: number;
  createdAt: Date;
  updatedAt: Date;
  
  // PRD Sections
  overview: Overview;
  personas: Persona[];
  userStories: UserStory[];
  features: Feature[];
  technicalRequirements: TechnicalRequirements;
  designRequirements: DesignRequirements;
  metrics: Metric[];
  timeline: Timeline;
  
  // Generated content
  diagrams: Diagram[];
  exportHistory: Export[];
}

interface Diagram {
  id: string;
  type: 'user-journey' | 'feature-hierarchy' | 'architecture' | 'erd' | 'timeline';
  mermaidCode: string;
  generatedAt: Date;
  linkedEntities: string[];  // IDs of personas, features, etc. used
}

interface Export {
  id: string;
  format: 'pdf' | 'markdown' | 'docx' | 'zip';
  generatedAt: Date;
  url: string;
}
```

### Database Schema (PostgreSQL)

```sql
-- Products table
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category TEXT[],
    status VARCHAR(20) DEFAULT 'draft',
    completion_percentage INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- PRD Sections stored as JSONB for flexibility
CREATE TABLE product_sections (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    section_type VARCHAR(50) NOT NULL, -- 'overview', 'personas', etc.
    content JSONB NOT NULL,
    version INTEGER DEFAULT 1,
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(product_id, section_type)
);

-- Individual items for relational queries
CREATE TABLE personas (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    role VARCHAR(255),
    data JSONB NOT NULL, -- goals, pain_points, etc.
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE user_stories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    persona_id UUID REFERENCES personas(id) ON DELETE SET NULL,
    as_a VARCHAR(255) NOT NULL,
    i_want TEXT NOT NULL,
    so_that TEXT NOT NULL,
    acceptance_criteria TEXT[],
    priority VARCHAR(20) DEFAULT 'should',
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE features (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    parent_id UUID REFERENCES features(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    phase VARCHAR(20) DEFAULT 'mvp',
    effort VARCHAR(20),
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Many-to-many: features to user stories
CREATE TABLE feature_stories (
    feature_id UUID REFERENCES features(id) ON DELETE CASCADE,
    story_id UUID REFERENCES user_stories(id) ON DELETE CASCADE,
    PRIMARY KEY (feature_id, story_id)
);

CREATE TABLE diagrams (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    type VARCHAR(50) NOT NULL,
    mermaid_code TEXT NOT NULL,
    generated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Indexes
CREATE INDEX idx_products_user ON products(user_id);
CREATE INDEX idx_personas_product ON personas(product_id);
CREATE INDEX idx_stories_product ON user_stories(product_id);
CREATE INDEX idx_features_product ON features(product_id);
```

---

## API Endpoints

### Products
```
POST   /api/products                    Create new product
GET    /api/products                    List user's products
GET    /api/products/{id}               Get product with all sections
PUT    /api/products/{id}               Update product metadata
DELETE /api/products/{id}               Delete product

POST   /api/products/{id}/duplicate     Duplicate a product
POST   /api/products/{id}/export        Export to PDF/Markdown/DOCX
```

### Sections
```
GET    /api/products/{id}/overview      Get overview section
PUT    /api/products/{id}/overview      Update overview section

GET    /api/products/{id}/personas      List personas
POST   /api/products/{id}/personas      Add persona
PUT    /api/products/{id}/personas/{pid} Update persona
DELETE /api/products/{id}/personas/{pid} Delete persona

GET    /api/products/{id}/stories       List user stories
POST   /api/products/{id}/stories       Add story
PUT    /api/products/{id}/stories/{sid} Update story
DELETE /api/products/{id}/stories/{sid} Delete story

GET    /api/products/{id}/features      List features
POST   /api/products/{id}/features      Add feature
PUT    /api/products/{id}/features/{fid} Update feature
DELETE /api/products/{id}/features/{fid} Delete feature
PUT    /api/products/{id}/features/reorder  Reorder features

GET    /api/products/{id}/technical     Get technical requirements
PUT    /api/products/{id}/technical     Update technical requirements

GET    /api/products/{id}/design        Get design requirements
PUT    /api/products/{id}/design        Update design requirements

GET    /api/products/{id}/metrics       List metrics
POST   /api/products/{id}/metrics       Add metric
PUT    /api/products/{id}/metrics/{mid} Update metric
DELETE /api/products/{id}/metrics/{mid} Delete metric

GET    /api/products/{id}/timeline      Get timeline
PUT    /api/products/{id}/timeline      Update timeline
```

### AI Assistance
```
POST   /api/ai/generate-overview        Generate overview from description
POST   /api/ai/generate-persona         Generate persona from description
POST   /api/ai/suggest-stories          Suggest stories for persona
POST   /api/ai/suggest-features         Suggest features for stories
POST   /api/ai/suggest-metrics          Suggest metrics for features
POST   /api/ai/refine-section           Refine any section content
POST   /api/ai/generate-diagram         Generate diagram from PRD content
```

### Diagrams
```
GET    /api/products/{id}/diagrams           List diagrams
POST   /api/products/{id}/diagrams/generate  Generate specific diagram type
DELETE /api/products/{id}/diagrams/{did}     Delete diagram
```

---

## Frontend Architecture

### Route Structure
```
/                           Welcome/Dashboard
/products                   Product list
/products/new               Create new product
/products/{id}              PRD Builder (main interface)
/products/{id}/overview     Overview section
/products/{id}/personas     Personas section
/products/{id}/stories      User stories section
/products/{id}/features     Features section
/products/{id}/technical    Technical requirements
/products/{id}/design       Design & UX
/products/{id}/metrics      Metrics section
/products/{id}/timeline     Timeline section
/products/{id}/diagrams     Diagrams view
/products/{id}/preview      Full PRD preview
/products/{id}/export       Export options
```

### State Management (Redux)
```typescript
interface RootState {
  auth: AuthState;
  products: {
    list: Product[];
    current: Product | null;
    loading: boolean;
    saving: boolean;
  };
  ui: {
    sidebarCollapsed: boolean;
    activeSection: string;
    aiPanelOpen: boolean;
  };
  ai: {
    generating: boolean;
    suggestions: Suggestion[];
  };
}
```

### Key Components
```
components/
├── layout/
│   ├── AppLayout.tsx           Main app layout
│   ├── Sidebar.tsx             PRD section navigation
│   └── AIAssistantPanel.tsx    AI suggestions panel
├── products/
│   ├── ProductCard.tsx         Product list item
│   ├── ProductWizard.tsx       New product creation
│   └── CompletionProgress.tsx  Progress indicator
├── sections/
│   ├── OverviewEditor.tsx      Overview section
│   ├── PersonaCard.tsx         Individual persona
│   ├── PersonaEditor.tsx       Persona edit modal
│   ├── StoryCard.tsx           User story display
│   ├── StoryEditor.tsx         Story edit modal
│   ├── FeatureBoard.tsx        Kanban-style features
│   ├── FeatureCard.tsx         Individual feature
│   ├── TechnicalChecklist.tsx  Tech requirements
│   ├── MetricCard.tsx          KPI display
│   └── TimelineGantt.tsx       Timeline visualization
├── diagrams/
│   ├── DiagramViewer.tsx       Mermaid rendering
│   └── DiagramGenerator.tsx    Diagram type selector
└── common/
    ├── RichTextEditor.tsx      Text editing
    ├── TagInput.tsx            Tag management
    ├── PrioritySelector.tsx    MoSCoW priority
    └── AIButton.tsx            AI action trigger
```

---

## Implementation Phases

### Phase 1: Foundation (2 weeks)
- [ ] New data model (Product, Personas, Stories, Features)
- [ ] Database migrations
- [ ] Core API endpoints (CRUD for all sections)
- [ ] Basic frontend routing and layout
- [ ] Welcome page and product list

### Phase 2: Section Editors (3 weeks)
- [ ] Overview editor with rich text
- [ ] Persona cards and editor modal
- [ ] User story cards with persona linking
- [ ] Feature board with drag-drop phases
- [ ] Technical requirements checklist
- [ ] Metrics editor with targets

### Phase 3: AI Integration (2 weeks)
- [ ] AI panel component
- [ ] Generate persona from description
- [ ] Suggest stories for persona
- [ ] Suggest features for stories
- [ ] Refine section content
- [ ] Generate diagrams

### Phase 4: Timeline & Diagrams (2 weeks)
- [ ] Timeline/Gantt view
- [ ] Auto-generate user journey diagram
- [ ] Auto-generate feature hierarchy
- [ ] Auto-generate architecture diagram
- [ ] Diagram export (PNG, SVG, Mermaid)

### Phase 5: Polish & Export (1 week)
- [ ] Full PRD preview mode
- [ ] Export to PDF
- [ ] Export to Markdown
- [ ] Export to Word/DOCX
- [ ] Completion percentage calculation
- [ ] Section validation warnings

---

## Success Metrics

| Metric | Target |
|--------|--------|
| Time to create first PRD | < 30 minutes |
| PRD completion rate | > 70% |
| User satisfaction (NPS) | > 40 |
| AI feature usage | > 60% of users |
| Return user rate | > 50% weekly |

---

## Open Questions

1. **Collaboration** - Should multiple users be able to edit the same PRD?
2. **Version History** - Should we track changes and allow rollback?
3. **Templates** - Should users be able to save their PRD as a template?
4. **Comments** - Should stakeholders be able to comment on sections?
5. **Approval Workflow** - Should there be a review/approval process?
