namespace Braavo.Core.Entities;

public enum FeaturePhase
{
    Mvp,
    Enhanced,
    Future
}

public enum EffortSize
{
    Small,
    Medium,
    Large
}

public class Feature
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid? ParentId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public FeaturePhase Phase { get; private set; }
    public EffortSize? Effort { get; private set; }
    public Guid[] LinkedStoryIds { get; private set; } = [];
    public int SortOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Feature() { }

    public static Feature Create(Guid productId, string name, string description)
    {
        return new Feature
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = name,
            Description = description,
            Phase = FeaturePhase.Mvp,
            LinkedStoryIds = [],
            SortOrder = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, EffortSize? effort)
    {
        Name = name;
        Description = description;
        Effort = effort;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePhase(FeaturePhase phase)
    {
        Phase = phase;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToStory(Guid storyId)
    {
        if (!LinkedStoryIds.Contains(storyId))
        {
            LinkedStoryIds = [.. LinkedStoryIds, storyId];
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UnlinkStory(Guid storyId)
    {
        LinkedStoryIds = LinkedStoryIds.Where(id => id != storyId).ToArray();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParent(Guid? parentId)
    {
        ParentId = parentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
