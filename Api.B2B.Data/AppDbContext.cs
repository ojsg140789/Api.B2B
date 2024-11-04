using Api.B2B.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.B2B.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklist { get; set; }

    }
}