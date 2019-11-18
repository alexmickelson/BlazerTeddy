using Microsoft.EntityFrameworkCore;
using Student.Models;

namespace Student.Data
{
    public class OurDbContext : DbContext
    {
        public OurDbContext(DbContextOptions<OurDbContext> options) : base(options)
        {
        }
        public DbSet<StudentInfo> Students { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Course> Courses { get; set; }

        // public DbSet<StudentCourseRelationship> StudentCourseRelationships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourseRelationship>()
                .HasKey(sc => new { sc.StudentInfoId, sc.CourseId });
            modelBuilder.Entity<StudentCourseRelationship>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);
            modelBuilder.Entity<StudentCourseRelationship>()
                .HasOne(sc => sc.student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentInfoId);
        }

    }
}
