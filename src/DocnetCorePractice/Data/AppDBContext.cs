using DocnetCorePractice.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace DocnetCorePractice.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext() { }
        public AppDBContext(DbContextOptions<DbContext> options) : base(options)
        {

        }

      public DbSet<RefreshTokens> RefreshTokens { get; set; }
       public DbSet<CaffeEntity> Caffe { get; set; }
       public DbSet<UserEntity> Users { get; set; }
       public DbSet<OrderEntity> Orders { get; set; }
       public DbSet<OrderItemEntity> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-PD8BQA1;Database=DotNetPracticeDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

    }
}
