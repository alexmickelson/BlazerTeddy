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

    }
}
