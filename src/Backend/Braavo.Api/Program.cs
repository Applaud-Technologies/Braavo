// src/Backend/Braavo.Api/Program.cs
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

public partial class Program { }
