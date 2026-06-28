using Braavo.Core.Interfaces;
using Braavo.Infrastructure.Auth;
using Braavo.Infrastructure.Data;
using Braavo.Infrastructure.ExternalServices;
using Braavo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Braavo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=braavo_db;Username=braavo;Password=braavo_dev";

        services.AddDbContext<BraavoDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();

        var useMockLlm = configuration.GetValue<bool>("UseMockLlm");
        if (useMockLlm)
            services.AddSingleton<ILlmProvider, MockLlmProvider>();
        else
            services.AddSingleton<ILlmProvider, OpenAiLlmProvider>();

        return services;
    }
}
