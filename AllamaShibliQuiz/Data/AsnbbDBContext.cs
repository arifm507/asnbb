using AllamaShibliQuiz.Models;
using Microsoft.EntityFrameworkCore;

namespace AllamaShibliQuiz.Data
{
    public class AsnbbDBContext : DbContext
    {
        public AsnbbDBContext(DbContextOptions<AsnbbDBContext> options) : base(options) {
        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<School> Schools { get; set; }
    }
}
