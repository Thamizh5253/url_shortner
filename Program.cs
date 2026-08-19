using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Services;

LoadDotEnv();

var builder   = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(GetConnectionString())
);
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

static void LoadDotEnv()
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(path))
    {
        return;
    }

    foreach (var line in File.ReadAllLines(path))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
        {
            continue;
        }

        var idx = trimmed.IndexOf('=');
        if (idx <= 0)
        {
            continue;
        }

        var key = trimmed[..idx].Trim();
        var value = trimmed[(idx + 1)..].Trim();
        Environment.SetEnvironmentVariable(key, value);
    }
}

static string GetConnectionString()
{
    string Env(string name) =>
        Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"Missing environment variable {name}");

    return $"Host={Env("DB_HOST")};Port={Env("DB_PORT")};Database={Env("DB_NAME")};" +
           $"Username={Env("DB_USER")};Password={Env("DB_PASSWORD")};SSL Mode=Disable";
}