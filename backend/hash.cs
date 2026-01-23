using BCrypt.Net;

Console.WriteLine("admin123 hash: " + BCrypt.HashPassword("admin123"));
Console.WriteLine("editor hash: " + BCrypt.HashPassword("editor"));