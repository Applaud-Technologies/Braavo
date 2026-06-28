using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using MediatR;

namespace Braavo.Core.UseCases.Timeline;

public record GetTimelineQuery(Guid ProductId, Guid UserId) : IRequest<GetTimelineResult>;

public record GetTimelineResult(
    bool Success,
    IReadOnlyList<TimelinePhaseDto>? Phases = null,
    string? Error = null
);

public record TimelinePhaseDto(
    Guid Id,
    string Name,
    int DurationWeeks,
    DateTime? StartDate,
    int SortOrder,
    IReadOnlyList<MilestoneDto> Milestones
);

public record MilestoneDto(
    Guid Id,
    string Name,
    int WeekNumber,
    string[] Deliverables,
    string Status
);

public class GetTimelineHandler : IRequestHandler<GetTimelineQuery, GetTimelineResult>
{
    private readonly IProductRepository _productRepo;
    private readonly ITimelineRepository _timelineRepo;

    public GetTimelineHandler(IProductRepository productRepo, ITimelineRepository timelineRepo)
    {
        _productRepo = productRepo;
        _timelineRepo = timelineRepo;
    }

    public async Task<GetTimelineResult> Handle(GetTimelineQuery request, CancellationToken ct)
    {
        var product = await _productRepo.GetByIdAsync(request.ProductId, ct);
        if (product is null || product.OwnerId != UserId.From(request.UserId))
            return new GetTimelineResult(false, Error: "Product not found");

        var phases = await _timelineRepo.GetByProductIdAsync(request.ProductId, ct);

        var dtos = phases.Select(p => new TimelinePhaseDto(
            p.Id,
            p.Name,
            p.DurationWeeks,
            p.StartDate,
            p.SortOrder,
            p.Milestones.Select(m => new MilestoneDto(
                m.Id,
                m.Name,
                m.WeekNumber,
                m.Deliverables,
                m.Status
            )).ToList()
        )).ToList();

        return new GetTimelineResult(true, dtos);
    }
}
