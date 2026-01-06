using Microsoft.EntityFrameworkCore;
using WebScheduleApi.Entities;

namespace WebScheduleApi;

public class WebScheduleDbContext : DbContext
{
    public WebScheduleDbContext(DbContextOptions<WebScheduleDbContext> options)
        : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
            .HaveColumnType("timestamp with time zone");

        base.ConfigureConventions(configurationBuilder);
    }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<ScheduleTemplate> ScheduleTemplates { get; set; }
    public DbSet<ActualLesson> ActualLessons { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to match database schema
        modelBuilder.Entity<Subject>().ToTable("subjects");
        modelBuilder.Entity<Teacher>().ToTable("teachers");
        modelBuilder.Entity<Room>().ToTable("rooms");
        modelBuilder.Entity<ScheduleTemplate>().ToTable("schedule_templates");
        modelBuilder.Entity<ActualLesson>().ToTable("actual_lessons");
        modelBuilder.Entity<Homework>().ToTable("homework");
        modelBuilder.Entity<User>().ToTable("users");

        // Configure unique constraint for homework
        modelBuilder.Entity<Homework>()
            .HasIndex(h => new { h.Date, h.LessonOrder })
            .IsUnique();

        // Configure DateTime properties to be stored as 'date' type
        modelBuilder.Entity<ActualLesson>()
            .Property(al => al.Date)
            .HasColumnType("date");

        modelBuilder.Entity<Homework>()
            .Property(h => h.Date)
            .HasColumnType("date");

        // Configure relationships
        modelBuilder.Entity<ScheduleTemplate>()
            .HasOne(st => st.Subject)
            .WithMany(s => s.ScheduleTemplates)
            .HasForeignKey(st => st.SubjectId);

        modelBuilder.Entity<ScheduleTemplate>()
            .HasOne(st => st.Teacher)
            .WithMany(t => t.ScheduleTemplates)
            .HasForeignKey(st => st.TeacherId);

        modelBuilder.Entity<ScheduleTemplate>()
            .HasOne(st => st.Room)
            .WithMany(r => r.ScheduleTemplates)
            .HasForeignKey(st => st.RoomId);

        modelBuilder.Entity<ActualLesson>()
            .HasOne(al => al.Subject)
            .WithMany(s => s.ActualLessons)
            .HasForeignKey(al => al.SubjectId);

        modelBuilder.Entity<ActualLesson>()
            .HasOne(al => al.Teacher)
            .WithMany(t => t.ActualLessons)
            .HasForeignKey(al => al.TeacherId);

        modelBuilder.Entity<ActualLesson>()
            .HasOne(al => al.Room)
            .WithMany(r => r.ActualLessons)
            .HasForeignKey(al => al.RoomId);
    }
}