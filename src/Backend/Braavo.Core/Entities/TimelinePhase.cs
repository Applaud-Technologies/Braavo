namespace Braavo.Core.Entities;

public class TimelinePhase
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int DurationWeeks { get; private set; }
    public DateTime? StartDate { get; private set; }
    public int SortOrder { get; private set; }
    public List<Milestone> Milestones { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private TimelinePhase() { }

    public static TimelinePhase Create(
        Guid productId,
        string name,
        int durationWeeks,
        DateTime? startDate = null,
        int sortOrder = 0)
    {
        return new TimelinePhase
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            DurationWeeks = durationWeeks,
            StartDate = startDate,
            SortOrder = sortOrder,
            Milestones = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, int durationWeeks, DateTime? startDate, int sortOrder)
    {
        Name = name;
        DurationWeeks = durationWeeks;
        StartDate = startDate;
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
