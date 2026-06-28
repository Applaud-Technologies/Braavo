// src/Backend/Braavo.Core/Interfaces/IJwtService.cs
using Braavo.Core.Entities;

namespace Braavo.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
