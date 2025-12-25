using Npgsql;
using System.Data;

namespace WebScheduleApi;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<int> ExecuteNonQueryAsync(string query, params NpgsqlParameter[] parameters)
    {
        using var connection = await GetConnectionAsync();
        using var command = new NpgsqlCommand(query, connection);
        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<object?> ExecuteScalarAsync(string query, params NpgsqlParameter[] parameters)
    {
        using var connection = await GetConnectionAsync();
        using var command = new NpgsqlCommand(query, connection);
        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        return await command.ExecuteScalarAsync();
    }

    public async Task<NpgsqlDataReader> ExecuteReaderAsync(string query, params NpgsqlParameter[] parameters)
    {
        var connection = await GetConnectionAsync();
        var command = new NpgsqlCommand(query, connection);
        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    }
}