using System.Security.Claims;
using Braavo.Core.UseCases.Timeline;
using FastEndpoints;
using MediatR;

namespace Braavo.Api.Endpoints.Timeline;

public record UpdateTimelineRequest(
    IReadOnlyList<UpdateTimelinePhaseRequest> Phases
);

public record UpdateTimelinePhaseRequest(
    string Name,
    int DurationWeeks,
    DateTime? StartDate,
    int SortOrder,
    IReadOnlyList<UpdateMilestoneRequest> Milestones
);

public record UpdateMilestoneRequest(
    string Name,
    int WeekNumber,
    string[] Deliverables,
    string Status
);

public class UpdateTimelineEndpoint : Endpoint<UpdateTimelineRequest, UpdateTimelineResult>
{
    private readonly IMediator _mediator;

    public UpdateTimelineEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/products/{productId}/timeline");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(UpdateTimelineRequest req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var productId = Route<Guid>("productId");

        var phases = req.Phases.Select(p => new UpdateTimelinePhaseInput(
            p.Name,
            p.DurationWeeks,
            p.StartDate,
            p.SortOrder,
            p.Milestones.Select(m => new UpdateMilestoneInput(
                m.Name,
                m.WeekNumber,
                m.Deliverables,
                m.Status
            )).ToList()
        )).ToList();

        var command = new UpdateTimelineCommand(productId, userId, phases);
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            await SendAsync(result, 400, ct);
            return;
        }

        await SendOkAsync(result, ct);
    }
}
