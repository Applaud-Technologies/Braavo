using Braavo.Core.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Braavo.Core.Services;

/// <summary>
/// Domain service that generates a professional PDF PRD document from structured product data.
/// </summary>
public static class PdfExporter
{
    public static byte[] GeneratePrd(
        Product product,
        IReadOnlyList<Persona> personas,
        IReadOnlyList<UserStory> userStories,
        IReadOnlyList<Feature> features,
        IReadOnlyList<TimelinePhase> timelinePhases)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, product));
                page.Content().Element(c => ComposeContent(c, product, personas, userStories, features, timelinePhases));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, Product product)
    {
        container.Column(col =>
        {
            col.Item().BorderBottom(1).BorderColor(Colors.Grey.Darken2).PaddingBottom(8).Row(row =>
            {
                row.RelativeItem().Column(inner =>
                {
                    inner.Item().Text(product.Name)
                        .FontSize(22).Bold().FontColor(Colors.Blue.Darken3);
                    inner.Item().Text("Product Requirements Document")
                        .FontSize(13).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(120).AlignRight().Column(inner =>
                {
                    inner.Item().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd}")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                    inner.Item().Text("Braavo PRD Builder")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            col.Item().Height(8);
        });
    }

    private static void ComposeContent(
        IContainer container,
        Product product,
        IReadOnlyList<Persona> personas,
        IReadOnlyList<UserStory> userStories,
        IReadOnlyList<Feature> features,
        IReadOnlyList<TimelinePhase> timelinePhases)
    {
        container.Column(col =>
        {
            col.Spacing(16);

            ComposeOverview(col, product);
            ComposePersonas(col, personas);
            ComposeUserStories(col, userStories);
            ComposeFeatures(col, features);
            ComposeTimeline(col, timelinePhases);
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text("Braavo - Guided PRD Builder").FontSize(8).FontColor(Colors.Grey.Medium);
            row.ConstantItem(50).AlignRight().Text(text =>
            {
                text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private static void ComposeOverview(ColumnDescriptor col, Product product)
    {
        col.Item().Element(c => SectionHeading(c, "Overview"));

        col.Item().Column(inner =>
        {
            inner.Spacing(10);

            SubsectionRow(inner, "Vision",
                string.IsNullOrWhiteSpace(product.Vision) ? "Not defined yet." : product.Vision);

            SubsectionRow(inner, "Problem Statement",
                string.IsNullOrWhiteSpace(product.ProblemStatement) ? "Not defined yet." : product.ProblemStatement);

            SubsectionRow(inner, "Value Proposition",
                string.IsNullOrWhiteSpace(product.ValueProposition) ? "Not defined yet." : product.ValueProposition);

            if (product.TargetMarket.Length > 0)
            {
                inner.Item().Column(list =>
                {
                    list.Item().Text("Target Market").Bold().FontSize(11);
                    list.Item().PaddingLeft(10).Column(bullets =>
                    {
                        foreach (var segment in product.TargetMarket)
                            bullets.Item().Text($"• {segment}");
                    });
                });
            }

            if (product.BusinessGoals.Length > 0)
            {
                inner.Item().Column(list =>
                {
                    list.Item().Text("Business Goals").Bold().FontSize(11);
                    list.Item().PaddingLeft(10).Column(bullets =>
                    {
                        foreach (var goal in product.BusinessGoals)
                            bullets.Item().Text($"• {goal}");
                    });
                });
            }
        });
    }

    private static void ComposePersonas(ColumnDescriptor col, IReadOnlyList<Persona> personas)
    {
        col.Item().Element(c => SectionHeading(c, "User Personas"));

        if (personas.Count == 0)
        {
            col.Item().Text("No personas defined yet.").Italic().FontColor(Colors.Grey.Medium);
            return;
        }

        col.Item().Column(inner =>
        {
            inner.Spacing(10);

            foreach (var persona in personas.OrderBy(p => p.SortOrder))
            {
                inner.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(card =>
                {
                    card.Item().Row(row =>
                    {
                        row.RelativeItem().Text(persona.Name).Bold().FontSize(13).FontColor(Colors.Blue.Darken2);
                        row.ConstantItem(120).AlignRight().Text($"{persona.Role}").Italic().FontColor(Colors.Grey.Darken1);
                    });

                    card.Item().Text($"Technical Level: {persona.TechnicalLevel}").FontSize(9).FontColor(Colors.Grey.Darken1);
                    card.Item().Height(4);

                    if (persona.Goals.Length > 0)
                    {
                        card.Item().Text("Goals:").Bold().FontSize(10);
                        card.Item().PaddingLeft(10).Column(list =>
                        {
                            foreach (var goal in persona.Goals)
                                list.Item().Text($"• {goal}").FontSize(9);
                        });
                    }

                    if (persona.PainPoints.Length > 0)
                    {
                        card.Item().Text("Pain Points:").Bold().FontSize(10);
                        card.Item().PaddingLeft(10).Column(list =>
                        {
                            foreach (var pain in persona.PainPoints)
                                list.Item().Text($"• {pain}").FontSize(9);
                        });
                    }

                    if (persona.Motivations.Length > 0)
                    {
                        card.Item().Text("Motivations:").Bold().FontSize(10);
                        card.Item().PaddingLeft(10).Column(list =>
                        {
                            foreach (var motivation in persona.Motivations)
                                list.Item().Text($"• {motivation}").FontSize(9);
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(persona.Quote))
                    {
                        card.Item().Height(4);
                        card.Item().Background(Colors.Blue.Lighten5).Padding(6)
                            .Text($"\"{persona.Quote}\"").Italic().FontSize(10).FontColor(Colors.Blue.Darken3);
                    }
                });
            }
        });
    }

    private static void ComposeUserStories(ColumnDescriptor col, IReadOnlyList<UserStory> userStories)
    {
        col.Item().Element(c => SectionHeading(c, "User Stories"));

        if (userStories.Count == 0)
        {
            col.Item().Text("No user stories defined yet.").Italic().FontColor(Colors.Grey.Medium);
            return;
        }

        var byPriority = userStories
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.SortOrder)
            .GroupBy(s => s.Priority);

        col.Item().Column(inner =>
        {
            inner.Spacing(10);

            foreach (var group in byPriority)
            {
                inner.Item().Text($"{group.Key} Priority").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);

                inner.Item().Column(stories =>
                {
                    stories.Spacing(6);
                    foreach (var story in group)
                    {
                        stories.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(card =>
                        {
                            card.Item().Text(text =>
                            {
                                text.Span("As a ").FontSize(10);
                                text.Span(story.AsA).Bold().FontSize(10);
                                text.Span(", I want ").FontSize(10);
                                text.Span(story.IWant).Bold().FontSize(10);
                                text.Span(" so that ").FontSize(10);
                                text.Span(story.SoThat).FontSize(10);
                            });

                            if (story.AcceptanceCriteria.Length > 0)
                            {
                                card.Item().Height(4);
                                card.Item().Text("Acceptance Criteria:").Bold().FontSize(9);
                                card.Item().PaddingLeft(10).Column(list =>
                                {
                                    foreach (var criterion in story.AcceptanceCriteria)
                                        list.Item().Text($"• {criterion}").FontSize(9);
                                });
                            }
                        });
                    }
                });
            }
        });
    }

    private static void ComposeFeatures(ColumnDescriptor col, IReadOnlyList<Feature> features)
    {
        col.Item().Element(c => SectionHeading(c, "Features"));

        if (features.Count == 0)
        {
            col.Item().Text("No features defined yet.").Italic().FontColor(Colors.Grey.Medium);
            return;
        }

        var byPhase = features
            .OrderBy(f => f.Phase)
            .ThenBy(f => f.SortOrder)
            .GroupBy(f => f.Phase);

        col.Item().Column(inner =>
        {
            inner.Spacing(10);

            foreach (var group in byPhase)
            {
                var phaseLabel = group.Key switch
                {
                    FeaturePhase.Mvp => "MVP",
                    FeaturePhase.Enhanced => "Enhanced",
                    FeaturePhase.Future => "Future",
                    _ => group.Key.ToString()
                };

                inner.Item().Text($"Phase: {phaseLabel}").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);

                var topLevel = group.Where(f => f.ParentId is null).ToList();
                var allChildren = group.Where(f => f.ParentId is not null).ToList();

                inner.Item().Column(featureCol =>
                {
                    featureCol.Spacing(4);
                    foreach (var feature in topLevel)
                        AppendFeaturePdf(featureCol, feature, allChildren, depth: 0);
                });
            }
        });
    }

    private static void AppendFeaturePdf(ColumnDescriptor col, Feature feature, List<Feature> allChildren, int depth)
    {
        var fontSize = depth == 0 ? 11 : 10;
        var paddingLeft = depth * 14;

        col.Item().PaddingLeft(paddingLeft).Column(inner =>
        {
            inner.Item().Row(row =>
            {
                row.RelativeItem().Text(feature.Name).Bold().FontSize(fontSize);
                if (feature.Effort.HasValue)
                    row.ConstantItem(80).AlignRight().Text($"Effort: {feature.Effort}").FontSize(9).FontColor(Colors.Grey.Darken1);
            });

            if (!string.IsNullOrWhiteSpace(feature.Description))
                inner.Item().Text(feature.Description).FontSize(9).FontColor(Colors.Grey.Darken2);
        });

        var children = allChildren.Where(c => c.ParentId == feature.Id).OrderBy(c => c.SortOrder).ToList();
        foreach (var child in children)
            AppendFeaturePdf(col, child, allChildren, depth + 1);
    }

    private static void ComposeTimeline(ColumnDescriptor col, IReadOnlyList<TimelinePhase> timelinePhases)
    {
        col.Item().Element(c => SectionHeading(c, "Timeline"));

        if (timelinePhases.Count == 0)
        {
            col.Item().Text("No timeline defined yet.").Italic().FontColor(Colors.Grey.Medium);
            return;
        }

        col.Item().Column(inner =>
        {
            inner.Spacing(10);

            foreach (var phase in timelinePhases.OrderBy(p => p.SortOrder))
            {
                inner.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(10).Column(card =>
                {
                    card.Item().Text(phase.Name).Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
                    card.Item().Text($"Duration: {phase.DurationWeeks} week{(phase.DurationWeeks == 1 ? "" : "s")}")
                        .FontSize(10).FontColor(Colors.Grey.Darken1);

                    if (phase.StartDate.HasValue)
                        card.Item().Text($"Start Date: {phase.StartDate.Value:yyyy-MM-dd}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);

                    if (phase.Milestones.Count > 0)
                    {
                        card.Item().Height(6);
                        card.Item().Text("Milestones:").Bold().FontSize(10);
                        card.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3);
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(4);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Milestone").Bold().FontSize(9);
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Week").Bold().FontSize(9);
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Status").Bold().FontSize(9);
                                header.Cell().Background(Colors.Blue.Lighten4).Padding(4).Text("Deliverables").Bold().FontSize(9);
                            });

                            foreach (var milestone in phase.Milestones.OrderBy(m => m.WeekNumber))
                            {
                                var deliverables = milestone.Deliverables.Length > 0
                                    ? string.Join(", ", milestone.Deliverables)
                                    : "-";

                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(milestone.Name).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(milestone.WeekNumber.ToString()).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(milestone.Status.ToString()).FontSize(9);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(deliverables).FontSize(9);
                            }
                        });
                    }
                });
            }
        });
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static void SectionHeading(IContainer container, string title)
    {
        container.BorderBottom(2).BorderColor(Colors.Blue.Darken2).PaddingBottom(4)
            .Text(title).Bold().FontSize(16).FontColor(Colors.Blue.Darken3);
    }

    private static void SubsectionRow(ColumnDescriptor col, string label, string value)
    {
        col.Item().Column(inner =>
        {
            inner.Item().Text(label).Bold().FontSize(11);
            inner.Item().PaddingLeft(8).Text(value).FontColor(Colors.Grey.Darken3);
        });
    }
}
