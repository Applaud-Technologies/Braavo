using Braavo.Core.Entities;
using Braavo.Core.Interfaces;
using Braavo.Core.UseCases.Products;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Timeline;

public record UpdateTimelinePhaseInput(
    string Name,
    int DurationWeeks,
    DateTime? StartDate,
    int SortOrder,
    IReadOnlyList<UpdateMilestoneInput> Milestones
);

public record UpdateMilestoneInput(
    string Name,
    int WeekNumber,
    string[] Deliverables,
    string Status
);

public record UpdateTimelineCommand(
    Guid ProductId,
    Guid UserId,
    IReadOnlyList<UpdateTimelinePhaseInput> Phases
) : IRequest<UpdateTimelineResult>;

public record UpdateTimelineResult(bool Success, string? Error = null);

public class UpdateTimelineHandler : IRequestHandler<UpdateTimelineCommand, UpdateTimelineResult>
{
    private readonly IProductRepository _productRepo;
    private readonly ITimelineRepository _timelineRepo;
    private readonly IMediator _mediator;

    public UpdateTimelineHandler(IProductRepository productRepo, ITimelineRepository timelineRepo, IMediator mediator)
    {
        _productRepo  = productRepo;
        _timelineRepo = timelineRepo;
        _mediator     = mediator;
    }

    public async Task<UpdateTimelineResult> Handle(UpdateTimelineCommand request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new UpdateTimelineResult(false, "Product not found");

        var phases = request.Phases.Select(p =>
        {
            var phase = TimelinePhase.Create(
                request.ProductId,
                p.Name,
                p.DurationWeeks,
                p.StartDate,
                p.SortOrder);

            foreach (var m in p.Milestones)
            {
                var milestone = Milestone.Create(
                    phase.Id,
                    m.Name,
                    m.WeekNumber,
                    m.Deliverables,
                    m.Status);
                phase.Milestones.Add(milestone);
            }

            return phase;
        }).ToList();

        await _timelineRepo.ReplaceTimelineAsync(request.ProductId, phases, ct);

        await _mediator.Send(new RecalculateCompletionCommand(request.ProductId), ct);

        return new UpdateTimelineResult(true);
    }
}
