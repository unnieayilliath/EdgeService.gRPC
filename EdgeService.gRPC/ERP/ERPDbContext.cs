using Microsoft.EntityFrameworkCore;

namespace EdgeService.gRPC.ERP
{
    public class ERPDbContext: DbContext
    {
        public DbSet<LocationData> LocationDatas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ERP;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationData>(entity =>
            {
                entity.ToTable("LocationData");
                entity.HasNoKey();
                // Other configurations
            });
        }
    }
}
