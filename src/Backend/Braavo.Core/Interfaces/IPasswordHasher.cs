// src/Backend/Braavo.Core/Interfaces/IPasswordHasher.cs
namespace Braavo.Core.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
