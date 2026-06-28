// src/Backend/Braavo.Api/Endpoints/Auth/RegisterEndpoint.cs
using Braavo.Core.Interfaces;
using Braavo.Core.ValueObjects;
using FastEndpoints;
using UserEntity = Braavo.Core.Entities.User;

namespace Braavo.Api.Endpoints.Auth;

public record RegisterRequest(string Email, string Name, string Password);
public record AuthResponse(string Token, UserDto User);
public record UserDto(Guid Id, string Email, string Name, string Role);

public class RegisterEndpoint : Endpoint<RegisterRequest, AuthResponse>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;

    public RegisterEndpoint(IUserRepository users, IPasswordHasher hasher, IJwtService jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (!Email.TryCreate(req.Email, out var email))
        {
            await SendAsync(new AuthResponse("", null!), 400, ct);
            return;
        }

        var existing = await _users.GetByEmailAsync(email!.Value, ct);
        if (existing is not null)
        {
            await SendAsync(new AuthResponse("", null!), 409, ct);
            return;
        }

        var user = UserEntity.Create(req.Email, req.Name);
        user.SetPasswordHash(_hasher.Hash(req.Password));
        await _users.AddAsync(user, ct);

        var token = _jwt.GenerateToken(user);
        var dto = new UserDto(user.Id.Value, user.Email.Value, user.Name, user.Role);

        await SendAsync(new AuthResponse(token, dto), 201, ct);
    }
}
