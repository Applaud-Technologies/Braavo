using System.Text.RegularExpressions;

namespace Braavo.Core.ValueObjects;

public readonly partial record struct Email
{
    public string Value { get; }

    private Email(string value) => Value = value.ToLowerInvariant();

    public static bool TryCreate(string? input, out Email? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(input)) return false;
        if (!EmailRegex().IsMatch(input)) return false;

        email = new Email(input);
        return true;
    }

    public static Email Create(string input)
    {
        if (!TryCreate(input, out var email))
            throw new ArgumentException("Invalid email format", nameof(input));
        return email!.Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    public override string ToString() => Value;
}
