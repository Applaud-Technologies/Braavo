using Braavo.Core.ValueObjects;

namespace Braavo.Core.Entities;

public class ProductVersion
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int VersionNumber { get; private set; }
    public string Snapshot { get; private set; } = string.Empty;
    public string Comment { get; private set; } = string.Empty;
    public UserId CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ProductVersion() { }

    public static ProductVersion Create(Guid productId, int versionNumber, string snapshot, string comment, UserId createdBy)
    {
        return new ProductVersion
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            VersionNumber = versionNumber,
            Snapshot = snapshot,
            Comment = comment,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
