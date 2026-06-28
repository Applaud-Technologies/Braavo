namespace Braavo.Core.Entities;

public class Milestone
{
    public Guid Id { get; private set; }
    public Guid PhaseId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int WeekNumber { get; private set; }
    public string[] Deliverables { get; private set; } = [];
    public string Status { get; private set; } = "planned";
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Milestone() { }

    public static Milestone Create(
        Guid phaseId,
        string name,
        int weekNumber,
        string[]? deliverables = null,
        string status = "planned")
    {
        return new Milestone
        {
            Id = Guid.NewGuid(),
            PhaseId = phaseId,
            Name = name,
            WeekNumber = weekNumber,
            Deliverables = deliverables ?? [],
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, int weekNumber, string[] deliverables, string status)
    {
        Name = name;
        WeekNumber = weekNumber;
        Deliverables = deliverables;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}
