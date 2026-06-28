namespace Braavo.Core.ValueObjects;

public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
    public static UserId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();
}
