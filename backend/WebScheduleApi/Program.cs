using WebScheduleApi;
using WebScheduleApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<WebScheduleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<IHomeworkRepository, HomeworkDbRepository>(); // Use PostgreSQL repository
builder.Services.AddScoped<ILessonsRepository, LessonsDbRepository>(); // Use PostgreSQL repository
builder.Services.AddScoped<ILessonDetailRepository, LessonDetailEfRepository>(); // Use EF repository
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WebScheduleDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseService>();

    // Check if tables exist and create them if not
    try
    {
        var checkTableQuery = "SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'subjects');";
        var tableExists = await databaseService.ExecuteScalarAsync(checkTableQuery);

        if (!(bool)(tableExists ?? false))
        {
            // Create tables
            var createTablesQuery = @"
                CREATE TABLE IF NOT EXISTS subjects (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS teachers (
                    id SERIAL PRIMARY KEY,
                    full_name VARCHAR(255) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS rooms (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(255) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS schedule_templates (
                    id SERIAL PRIMARY KEY,
                    subject_id INT REFERENCES subjects(id),
                    teacher_id INT REFERENCES teachers(id),
                    room_id INT REFERENCES rooms(id),
                    day_of_week INT NOT NULL CHECK (day_of_week BETWEEN 1 AND 7),
                    lesson_order INT NOT NULL,
                    start_time TIME NOT NULL,
                    end_time TIME NOT NULL,
                    is_active BOOLEAN DEFAULT true
                );

                CREATE TABLE IF NOT EXISTS actual_lessons (
                    id SERIAL PRIMARY KEY,
                    subject_id INT REFERENCES subjects(id),
                    teacher_id INT REFERENCES teachers(id),
                    room_id INT REFERENCES rooms(id),
                    date DATE NOT NULL,
                    lesson_order INT NOT NULL,
                    start_time TIME NOT NULL,
                    end_time TIME NOT NULL,
                    is_cancelled BOOLEAN DEFAULT false
                );

                CREATE TABLE IF NOT EXISTS homework (
                    id SERIAL PRIMARY KEY,
                    date DATE NOT NULL,
                    lesson_order INT NOT NULL,
                    homework_text TEXT,
                    UNIQUE(date, lesson_order)
                );

                CREATE TABLE IF NOT EXISTS users (
                    id SERIAL PRIMARY KEY,
                    username VARCHAR(255) UNIQUE NOT NULL,
                    password_hash VARCHAR(255) NOT NULL,
                    role VARCHAR(50) NOT NULL DEFAULT 'user'
                );
            ";
            await databaseService.ExecuteNonQueryAsync(createTablesQuery);

            // Insert sample data
            var insertDataQuery = @"
                INSERT INTO subjects (name) VALUES ('Математика'), ('Физика'), ('Химия');
                INSERT INTO teachers (full_name) VALUES ('Иванов И.И.'), ('Петров П.П.'), ('Сидоров С.С.');
                INSERT INTO rooms (name) VALUES ('101'), ('102'), ('103');

                INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
                VALUES
                (1, 1, 1, 1, 1, '08:00', '09:30'),
                (2, 2, 2, 1, 2, '09:45', '11:15'),
                (3, 3, 3, 1, 3, '11:30', '13:00');

                INSERT INTO users (username, password_hash, role) VALUES ('admin', '$2a$11$example.hash.for.admin', 'admin');
            ";
            await databaseService.ExecuteNonQueryAsync(insertDataQuery);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
