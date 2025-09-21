using Microsoft.EntityFrameworkCore;
using BBUAPI.Models;

namespace BBUAPI.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Advertisements> Advertisements { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<NewsImage>()
             .HasOne(i => i.News)
             .WithMany(b => b.Images)
             .HasForeignKey(i => i.NewsId)
             .OnDelete(DeleteBehavior.Cascade);
        }




    }
}
