using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebScheduleApi.Models;

namespace WebScheduleApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly DatabaseService _databaseService;

    public AuthController(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var query = "SELECT password_hash, role FROM users WHERE username = @username";
            using var reader = await _databaseService.ExecuteReaderAsync(query,
                new NpgsqlParameter("@username", request.Username));

            if (await reader.ReadAsync())
            {
                var passwordHash = reader.GetString(0);
                var role = reader.GetString(1);

                if (BCrypt.Net.BCrypt.Verify(request.Password, passwordHash))
                {
                    var isAdmin = role == "admin";
                    return Ok(new { success = true, isAdmin });
                }
            }

            return Ok(new { success = false, message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            return Ok(new { success = false, message = $"Error during login: {ex.Message}" });
        }
    }

}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}