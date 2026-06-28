namespace Braavo.Core.Entities;

public enum TechnicalLevel
{
    Low,
    Medium,
    High
}

public class Persona
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public TechnicalLevel TechnicalLevel { get; private set; }
    public string[] Goals { get; private set; } = [];
    public string[] PainPoints { get; private set; } = [];
    public string[] Motivations { get; private set; } = [];
    public string Quote { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Persona() { }

    public static Persona Create(Guid productId, string name, string role)
    {
        return new Persona
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            Role = role,
            TechnicalLevel = TechnicalLevel.Medium,
            Goals = [],
            PainPoints = [],
            Motivations = [],
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string role, TechnicalLevel technicalLevel, string[] goals, string[] painPoints, string quote)
    {
        Name = name;
        Role = role;
        TechnicalLevel = technicalLevel;
        Goals = goals;
        PainPoints = painPoints;
        Quote = quote;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
