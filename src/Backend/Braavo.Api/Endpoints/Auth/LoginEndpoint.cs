// src/Backend/Braavo.Api/Endpoints/Auth/LoginEndpoint.cs
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using FastEndpoints;

namespace Braavo.Api.Endpoints.Auth;

public record LoginRequest(string Email, string Password);

public class LoginEndpoint : Endpoint<LoginRequest, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public LoginEndpoint(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (!Email.TryCreate(req.Email, out var email))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var user = await _users.GetByEmailAsync(email!.Value, ct);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (!_hasher.Verify(req.Password, user.PasswordHash))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var token = _jwt.GenerateToken(user);
        var dto = new UserDto(user.Id.Value, user.Email.Value, user.Name, user.Role);

        await SendOkAsync(new AuthResponse(token, dto), ct);
    }
}
