using Microsoft.EntityFrameworkCore;
using westcoast_education.api.Data.Models;

namespace westcoast_education.api.Data;

public class EducationContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Course> Courses => Set<Course>();
    public EducationContext(DbContextOptions options) : base(options)
    {
    }
}