using Braavo.Core.Entities;

namespace Braavo.Infrastructure.Data.Seeds;

public static class TemplateSeed
{
    public static List<Template> GetDefaultTemplates() =>
    [
        Template.Create(
            "SaaS Application",
            "Template for B2B SaaS products with subscription model",
            "Software",
            GetSaasTemplate(),
            "Describe your SaaS product's core value proposition and target market",
            isSystem: true
        ),
        Template.Create(
            "Mobile App",
            "Template for iOS/Android mobile applications",
            "Mobile",
            GetMobileTemplate(),
            "Describe your mobile app's main features and target users",
            isSystem: true
        ),
        Template.Create(
            "E-commerce Platform",
            "Template for online retail and marketplace products",
            "E-commerce",
            GetEcommerceTemplate(),
            "Describe your e-commerce product and target market segment",
            isSystem: true
        ),
        Template.Create(
            "Internal Tool",
            "Template for internal business applications and tools",
            "Enterprise",
            GetInternalToolTemplate(),
            "Describe the business process this tool will streamline",
            isSystem: true
        ),
        Template.Create(
            "API/Platform",
            "Template for developer platforms and API products",
            "Developer",
            GetApiTemplate(),
            "Describe your API's capabilities and target developer audience",
            isSystem: true
        )
    ];

    private static string GetSaasTemplate() => """
        # [Product Name] PRD

        ## Overview
        [Brief description of the SaaS product]

        ## Problem Statement
        [What problem does this solve for businesses?]

        ## Target Users
        - Primary: [Job title/role]
        - Secondary: [Other stakeholders]

        ## Features
        1. **Core Feature** - [Description]
        2. **Dashboard** - [Analytics and reporting]
        3. **Integrations** - [Third-party connections]
        4. **Team Management** - [Collaboration features]

        ## Success Metrics
        - Monthly Recurring Revenue (MRR)
        - Customer Acquisition Cost (CAC)
        - Monthly Active Users (MAU)
        - Net Promoter Score (NPS)

        ## User Stories
        - As a [user], I want to [goal] so that [benefit]
        """;

    private static string GetMobileTemplate() => """
        # [App Name] PRD

        ## Overview
        [Brief description of the mobile app]

        ## Problem Statement
        [What problem does this solve for mobile users?]

        ## Target Users
        - Primary: [Demographics and behaviors]
        - Secondary: [Other user segments]

        ## Features
        1. **Onboarding** - [First-time user experience]
        2. **Core Feature** - [Main functionality]
        3. **Notifications** - [Push notification strategy]
        4. **Offline Mode** - [Offline capabilities]

        ## Success Metrics
        - Daily Active Users (DAU)
        - Session Duration
        - Retention Rate (D1, D7, D30)
        - App Store Rating

        ## User Stories
        - As a [user], I want to [goal] so that [benefit]
        """;

    private static string GetEcommerceTemplate() => """
        # [Store Name] PRD

        ## Overview
        [Brief description of the e-commerce platform]

        ## Problem Statement
        [What shopping problem does this solve?]

        ## Target Users
        - Buyers: [Customer segments]
        - Sellers: [Merchant types if applicable]

        ## Features
        1. **Product Catalog** - [Browse and search]
        2. **Shopping Cart** - [Cart management]
        3. **Checkout** - [Payment and shipping]
        4. **Order Tracking** - [Post-purchase experience]

        ## Success Metrics
        - Conversion Rate
        - Average Order Value (AOV)
        - Cart Abandonment Rate
        - Customer Lifetime Value (CLV)

        ## User Stories
        - As a shopper, I want to [goal] so that [benefit]
        """;

    private static string GetInternalToolTemplate() => """
        # [Tool Name] PRD

        ## Overview
        [Brief description of the internal tool]

        ## Problem Statement
        [What business process does this streamline?]

        ## Target Users
        - Primary: [Department/Role]
        - Admin: [System administrators]

        ## Features
        1. **Data Entry** - [Input mechanisms]
        2. **Workflow Automation** - [Automated processes]
        3. **Reporting** - [Analytics and exports]
        4. **Access Control** - [Permissions]

        ## Success Metrics
        - Time Saved per Task
        - Error Rate Reduction
        - User Adoption Rate
        - Process Completion Time

        ## User Stories
        - As an employee, I want to [goal] so that [benefit]
        """;

    private static string GetApiTemplate() => """
        # [API Name] PRD

        ## Overview
        [Brief description of the API/platform]

        ## Problem Statement
        [What developer problem does this solve?]

        ## Target Users
        - Developers: [Skill levels and use cases]
        - Partners: [Integration partners]

        ## Features
        1. **Core API** - [Main endpoints]
        2. **Authentication** - [Auth mechanisms]
        3. **SDKs** - [Language support]
        4. **Documentation** - [Developer portal]

        ## Success Metrics
        - API Calls per Month
        - Developer Adoption
        - Integration Success Rate
        - Documentation Satisfaction

        ## User Stories
        - As a developer, I want to [goal] so that [benefit]
        """;
}
