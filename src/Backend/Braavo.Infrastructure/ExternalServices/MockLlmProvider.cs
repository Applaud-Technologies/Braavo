using Braavo.Core.Interfaces;
using Braavo.Core.Models;

namespace Braavo.Infrastructure.ExternalServices;

public class MockLlmProvider : ILlmProvider
{
    public Task<LlmResponse> GenerateAsync(LlmRequest request, CancellationToken ct = default)
    {
        var content = request.SystemPrompt switch
        {
            var s when s?.Contains("user persona") == true => GenerateMockPersona(request.Prompt),
            var s when s?.Contains("user stories") == true => GenerateMockStories(),
            var s when s?.Contains("features") == true => GenerateMockFeatures(),
            var s when s?.Contains("refine") == true || s?.Contains("improve") == true => GenerateMockRefinement(request.Prompt),
            _ => GenerateMockPrd(request.Prompt)
        };

        var response = new LlmResponse(
            Content: content,
            PromptTokens: request.Prompt.Length / 4,
            CompletionTokens: 500,
            Success: true
        );
        return Task.FromResult(response);
    }

    private static string GenerateMockPersona(string description)
    {
        return """
        {
          "name": "Alex Thompson",
          "role": "Product Manager",
          "technicalLevel": "Medium",
          "goals": ["Streamline project workflows", "Improve team collaboration", "Track project metrics effectively"],
          "painPoints": ["Too many disconnected tools", "Difficulty getting status updates", "Manual reporting processes"],
          "quote": "I need a single place to see everything happening across my projects."
        }
        """;
    }

    private static string GenerateMockStories()
    {
        return """
        [
          {
            "asA": "Product Manager",
            "iWant": "to view all project statuses on a single dashboard",
            "soThat": "I can quickly identify blockers and priorities",
            "priority": "Must",
            "acceptanceCriteria": ["Dashboard loads in under 2 seconds", "Shows status for all assigned projects", "Updates in real-time"]
          },
          {
            "asA": "Product Manager",
            "iWant": "to generate reports with one click",
            "soThat": "I can share progress with stakeholders without manual work",
            "priority": "Should",
            "acceptanceCriteria": ["Export to PDF and CSV", "Include charts and metrics", "Customizable date ranges"]
          }
        ]
        """;
    }

    private static string GenerateMockFeatures()
    {
        return """
        [
          {
            "name": "Project Dashboard",
            "description": "Centralized view of all project statuses, metrics, and blockers",
            "phase": "Mvp",
            "effort": "Medium",
            "linkedStoryIds": []
          },
          {
            "name": "One-Click Reports",
            "description": "Generate and export project reports with customizable templates",
            "phase": "Enhanced",
            "effort": "Large",
            "linkedStoryIds": []
          }
        ]
        """;
    }

    private static string GenerateMockRefinement(string content)
    {
        return $"Improved: {content.Trim()}. This version is more specific, actionable, and measurable.";
    }

    public Task<IAsyncEnumerable<string>> StreamAsync(LlmRequest request, CancellationToken ct = default)
    {
        return Task.FromResult(StreamMockResponse(request.Prompt));
    }

    private static async IAsyncEnumerable<string> StreamMockResponse(string prompt)
    {
        var content = GenerateMockPrd(prompt);
        var words = content.Split(' ');
        foreach (var word in words)
        {
            yield return word + " ";
            await Task.Delay(20);
        }
    }

    private static string GenerateMockPrd(string prompt)
    {
        var ideaSummary = prompt.Length > 80 ? prompt[..80] + "..." : prompt;
        return $"""
        # Smart Product Solution

        ## 1. Objective and Purpose

        **Product Vision:** {ideaSummary}

        This product addresses a critical need in the market by providing users with an intuitive, efficient solution. The core problem it solves is the friction and complexity users currently face when trying to accomplish their goals.

        **Why Now:** Market demand and technological readiness make this the right time to build this solution. Expected business value includes increased user productivity, reduced operational costs, and new revenue opportunities.

        ## 2. User Personas

        ### Sarah - The Power User
        - **Role:** Team Lead at a mid-size company
        - **Goals:** Streamline daily workflows, improve team collaboration
        - **Pain Points:** Too many disconnected tools, manual repetitive tasks
        - **Technical Comfort:** High - comfortable with modern software

        ### Mike - The Casual User
        - **Role:** Individual contributor
        - **Goals:** Get work done quickly without learning complex tools
        - **Pain Points:** Steep learning curves, feature overload
        - **Technical Comfort:** Medium - prefers simple, intuitive interfaces

        ### Alex - The Administrator
        - **Role:** IT Administrator
        - **Goals:** Manage users, ensure security, generate reports
        - **Pain Points:** Lack of centralized control, compliance concerns
        - **Technical Comfort:** High - needs robust admin capabilities

        ## 3. User Stories

        - As a **power user**, I want to automate repetitive tasks so that I can focus on higher-value work.
        - As a **casual user**, I want a simple onboarding experience so that I can start being productive immediately.
        - As a **team lead**, I want to see my team's progress at a glance so that I can identify blockers early.
        - As an **administrator**, I want to manage user permissions so that I can ensure data security.
        - As a **user**, I want to integrate with my existing tools so that I don't have to change my workflow.
        - As a **user**, I want to access the product on mobile so that I can work from anywhere.

        ## 4. Features and Requirements

        ### Core (MVP)
        - **User Authentication** - Secure login with SSO support
        - **Dashboard** - Personalized overview of key metrics and tasks
        - **Core Workflow** - Main functionality addressing the primary use case
        - **Notifications** - Real-time alerts for important events

        ### Enhanced (Phase 2)
        - **Team Collaboration** - Shared workspaces, comments, mentions
        - **Integrations** - Connect with Slack, GitHub, Jira
        - **Advanced Reporting** - Custom reports and data exports
        - **Mobile App** - Native iOS and Android applications

        ### Future (Phase 3)
        - **AI-Powered Insights** - Predictive analytics and recommendations
        - **Automation Builder** - Custom workflow automation
        - **API Access** - Public API for custom integrations

        ## 5. Technical Requirements

        - **Platforms:** Web (responsive), iOS, Android
        - **Integrations:** OAuth 2.0, REST APIs for Slack/GitHub/Jira
        - **Performance:** Page load < 2 seconds, API response < 200ms, 99.9% uptime
        - **Security:** SOC 2 compliance, end-to-end encryption, MFA support
        - **Constraints:** Must support latest 2 versions of major browsers

        ## 6. Design and UX Considerations

        - **Key Screens:** Dashboard, Workflow View, Settings, Admin Panel
        - **Navigation:** Top nav for main sections, sidebar for context-specific actions
        - **Accessibility:** WCAG 2.1 AA compliance
        - **Visual Style:** Clean, modern, consistent with brand guidelines

        ## 7. Metrics and Success Criteria

        | Metric | Target | Timeframe |
        |--------|--------|-----------|
        | User Adoption | 50% of target users | 3 months post-launch |
        | Daily Active Users | 1,000 DAU | 6 months |
        | Task Completion Rate | > 85% | Ongoing |
        | User Satisfaction (NPS) | > 40 | Quarterly |
        | Feature Usage | 3+ features/user | Monthly |

        ## 8. Timeline and Milestones

        - **Phase 1 (MVP):** Core features - 8 weeks
          - Week 1-2: Architecture and setup
          - Week 3-5: Core workflow implementation
          - Week 6-7: Testing and refinement
          - Week 8: Beta launch

        - **Phase 2 (Enhancement):** Team features + integrations - 6 weeks after MVP

        - **Phase 3 (Scale):** Mobile apps + AI features - Q2 following year
        """;
    }
}
