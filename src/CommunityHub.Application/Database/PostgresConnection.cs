using System.Data;
using System.Text.Json;
using Npgsql;

namespace CommunityHub.Application.Database;

public static class PostgresConnection
{
    private static readonly string ConnectionString = LoadConnectionString();

    public static IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    private static string LoadConnectionString()
    {
        var configPath = FindConfigPath()
            ?? throw new FileNotFoundException(
                "Configuration file 'appsettings.json' not found. Expected it next to the executable or in the current working directory.");

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found at: {configPath}");
        }

        var json = File.ReadAllText(configPath);
        using var document = JsonDocument.Parse(json);

        var connectionString = document.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
        }

        return connectionString;
    }

    private static string? FindConfigPath()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }
}
