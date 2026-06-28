using System.Text;
using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Diagrams;

public record GenerateGanttCommand(Guid ProductId, Guid UserId) : IRequest<GenerateGanttResult>;

public record GenerateGanttResult(
    bool Success,
    string? MermaidCode = null,
    string? Error = null
);

public class GenerateGanttHandler : IRequestHandler<GenerateGanttCommand, GenerateGanttResult>
{
    private readonly IProductRepository _productRepo;
    private readonly ITimelineRepository _timelineRepo;

    public GenerateGanttHandler(IProductRepository productRepo, ITimelineRepository timelineRepo)
    {
        _productRepo = productRepo;
        _timelineRepo = timelineRepo;
    }

    public async Task<GenerateGanttResult> Handle(GenerateGanttCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new GenerateGanttResult(false, Error: "Product not found");

        var phases = await _timelineRepo.GetByProductIdAsync(request.ProductId, ct);

        if (phases.Count == 0)
            return new GenerateGanttResult(false, Error: "No timeline phases found");

        var orderedPhases = phases.OrderBy(p => p.SortOrder).ToList();
        var mermaidCode = GenerateMermaidGantt(product.Name, orderedPhases);

        return new GenerateGanttResult(true, MermaidCode: mermaidCode);
    }

    private static string GenerateMermaidGantt(string projectName, List<TimelinePhase> phases)
    {
        var sb = new StringBuilder();
        sb.AppendLine("gantt");
        sb.AppendLine($"    title {projectName} Timeline");
        sb.AppendLine("    dateFormat YYYY-MM-DD");
        sb.AppendLine();

        // Compute project start from first phase that has an explicit date, or default to today
        var projectStart = phases.FirstOrDefault(p => p.StartDate.HasValue)?.StartDate
            ?? DateTime.UtcNow.Date;

        string? lastTaskId = null;
        var cumulativeWeeks = 0;

        for (var phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
        {
            var phase = phases[phaseIndex];
            sb.AppendLine($"    section {phase.Name}");

            var phaseStart = phase.StartDate ?? projectStart.AddDays(cumulativeWeeks * 7);
            var orderedMilestones = phase.Milestones.OrderBy(m => m.WeekNumber).ToList();

            if (orderedMilestones.Count == 0)
            {
                // No milestones: represent the entire phase as a single task
                var taskId = $"p{phaseIndex}t0";
                var startStr = BuildStartString(phaseStart, lastTaskId, phase.StartDate.HasValue);
                sb.AppendLine($"    {EscapeName(phase.Name),-24}: {taskId}, {startStr}, {phase.DurationWeeks}w");
                lastTaskId = taskId;
            }
            else
            {
                for (var mIdx = 0; mIdx < orderedMilestones.Count; mIdx++)
                {
                    var milestone = orderedMilestones[mIdx];
                    var taskId = $"p{phaseIndex}t{mIdx}";

                    string startStr;
                    if (mIdx == 0)
                    {
                        // First task in a phase — anchor to a date or previous phase's last task
                        startStr = BuildStartString(phaseStart, lastTaskId, phase.StartDate.HasValue);
                    }
                    else
                    {
                        startStr = $"after p{phaseIndex}t{mIdx - 1}";
                    }

                    // Duration: gap to next milestone, or remaining weeks in phase for the last one
                    int durationWeeks;
                    if (mIdx < orderedMilestones.Count - 1)
                    {
                        durationWeeks = orderedMilestones[mIdx + 1].WeekNumber - milestone.WeekNumber;
                        if (durationWeeks <= 0) durationWeeks = 1;
                    }
                    else
                    {
                        durationWeeks = Math.Max(1, phase.DurationWeeks - milestone.WeekNumber + 1);
                    }

                    var statusStr = GetStatusPrefix(milestone.Status);
                    sb.AppendLine($"    {EscapeName(milestone.Name),-24}: {statusStr}{taskId}, {startStr}, {durationWeeks}w");
                    lastTaskId = taskId;
                }
            }

            cumulativeWeeks += phase.DurationWeeks;
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Returns either an absolute date string or an "after {lastTaskId}" reference.
    /// An explicit phase StartDate always wins; otherwise we fall back to "after" when a
    /// previous task exists, keeping the diagram flexible when dates are not fully set.
    /// </summary>
    private static string BuildStartString(DateTime phaseStart, string? lastTaskId, bool hasExplicitDate)
    {
        if (!hasExplicitDate && lastTaskId is not null)
            return $"after {lastTaskId}";

        return phaseStart.ToString("yyyy-MM-dd");
    }

    private static string GetStatusPrefix(string status) => status.ToLowerInvariant() switch
    {
        "done" or "completed" => "done, ",
        "active" or "in-progress" or "in_progress" => "active, ",
        _ => ""
    };

    /// <summary>Strips colons from names as they are reserved delimiters in Mermaid Gantt syntax.</summary>
    private static string EscapeName(string name) => name.Replace(":", "");
}
