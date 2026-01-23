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
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                return Unauthorized("Authorization header required");
            }

            var encodedCredentials = authHeader.Substring("Basic ".Length);
            var credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var parts = credentials.Split(':');
            if (parts.Length != 2)
            {
                return Unauthorized("Invalid credentials format");
            }

            var username = parts[0];
            var password = parts[1];

            var query = "SELECT password_hash, role FROM users WHERE username = @username";
            using var reader = await _databaseService.ExecuteReaderAsync(query,
                new NpgsqlParameter("@username", username));

            if (await reader.ReadAsync())
            {
                var passwordHash = reader.GetString(0);
                var role = reader.GetString(1);

                if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
                {
                    return Ok(new { success = true, role });
                }
            }

            return Ok(new { success = false, message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error during login: {ex.Message}");
        }
    }

}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}