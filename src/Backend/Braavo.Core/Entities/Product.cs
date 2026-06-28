using System.Text.Json;
using Braavo.Core.ValueObjects;

namespace Braavo.Core.Entities;

public enum ProductStatus
{
    Draft,
    InProgress,
    Review,
    Final
}

public class Product
{
    public Guid Id { get; private set; }
    public UserId OwnerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string[] Categories { get; private set; } = [];
    public ProductStatus Status { get; private set; }
    public int Version { get; private set; }
    public int CompletionPercentage { get; private set; }

    // Overview section
    public string Vision { get; private set; } = string.Empty;
    public string ProblemStatement { get; private set; } = string.Empty;
    public string ValueProposition { get; private set; } = string.Empty;
    public string[] TargetMarket { get; private set; } = [];
    public string[] BusinessGoals { get; private set; } = [];

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product() { }

    public static Product Create(string name, string description, UserId ownerId)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name,
            Description = description,
            Status = ProductStatus.Draft,
            Version = 1,
            CompletionPercentage = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateOverview(string vision, string problemStatement, string valueProposition)
    {
        Vision = vision;
        ProblemStatement = problemStatement;
        ValueProposition = valueProposition;
        IncrementVersion();
    }

    public void UpdateTargetMarket(string[] targetMarket)
    {
        TargetMarket = targetMarket;
        IncrementVersion();
    }

    public void UpdateBusinessGoals(string[] businessGoals)
    {
        BusinessGoals = businessGoals;
        IncrementVersion();
    }

    public void UpdateStatus(ProductStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCompletionPercentage(int percentage)
    {
        CompletionPercentage = Math.Clamp(percentage, 0, 100);
        UpdatedAt = DateTime.UtcNow;
    }

    public ProductVersion CreateVersion(string comment)
    {
        var snapshot = JsonSerializer.Serialize(new
        {
            Name,
            Description,
            Vision,
            ProblemStatement,
            ValueProposition,
            TargetMarket,
            BusinessGoals,
            Status = Status.ToString()
        });

        return ProductVersion.Create(Id, Version, snapshot, comment, OwnerId);
    }

    private void IncrementVersion()
    {
        Version++;
        UpdatedAt = DateTime.UtcNow;
    }
}
