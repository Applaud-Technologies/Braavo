namespace Braavo.Core.Entities;

public enum StoryPriority
{
    Must,
    Should,
    Could,
    Wont
}

public class UserStory
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? PersonaId { get; private set; }
    public string AsA { get; private set; } = string.Empty;
    public string IWant { get; private set; } = string.Empty;
    public string SoThat { get; private set; } = string.Empty;
    public StoryPriority Priority { get; private set; }
    public string[] AcceptanceCriteria { get; private set; } = [];
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserStory() { }

    public static UserStory Create(Guid productId, string asA, string iWant, string soThat)
    {
        return new UserStory
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            AsA = asA,
            IWant = iWant,
            SoThat = soThat,
            Priority = StoryPriority.Should,
            AcceptanceCriteria = [],
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string asA, string iWant, string soThat, StoryPriority priority)
    {
        AsA = asA;
        IWant = iWant;
        SoThat = soThat;
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToPersona(Guid? personaId)
    {
        PersonaId = personaId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAcceptanceCriteria(string criteria)
    {
        AcceptanceCriteria = [.. AcceptanceCriteria, criteria];
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAcceptanceCriteria(string[] criteria)
    {
        AcceptanceCriteria = criteria;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
