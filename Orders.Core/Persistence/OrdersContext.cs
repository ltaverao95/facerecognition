namespace Orders.Core.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using Orders.Core.Models;
    using Polly;

    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new EnumToStringConverter<Status>();
            modelBuilder
                .Entity<Order>()
                .Property(x => x.Status)
                .HasConversion(converter);
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public void MigrateDB()
        {
            if (!Database.GetPendingMigrations().Any())
            {
                return;
            }

            Policy.Handle<Exception>()
                .WaitAndRetry(10, x => TimeSpan.FromSeconds(10))
                .Execute(() => Database.Migrate());
        }
    }
}