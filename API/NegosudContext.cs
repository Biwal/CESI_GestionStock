using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;

namespace Api
{
    public class NegosudContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ClientOrder> ClientOrders { get; set; }
        public DbSet<ClientOrderItem> ClientOrderItems { get; set; }
        public DbSet<ProviderOrder> ProviderOrders { get; set; }
        public DbSet<ProviderOrderItem> ProviderOrderItems { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<Provider> Providers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientOrder>()
            .HasOne<Client>(c => c.Client)
            .WithMany(c => c.ClientOrders)
            .HasForeignKey(c => c.ClientId);

            modelBuilder.Entity<ProviderOrder>()
            .HasOne<Provider>(c => c.Provider);

            modelBuilder.Entity<ClientOrderItem>()
            .HasOne<ClientOrder>(p => p.ClientOrder)
            .WithMany(b => b.ClientOrderItems)
            .HasForeignKey(p => p.ClientOrderId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProviderOrderItem>()
            .HasOne<ProviderOrder>(p => p.ProviderOrder)
            .WithMany(b => b.ProviderOrderItems)
            .HasForeignKey(p => p.ProviderOrderId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientOrderItem>()
            .HasOne<Product>(p => p.Product);

            modelBuilder.Entity<ProviderOrderItem>()
            .HasOne<Product>(p => p.Product);

            modelBuilder.Entity<Product>().HasOne<Family>(p => p.Family);
            modelBuilder.Entity<Product>().HasOne<Provider>(p => p.Provider);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={AppContext.BaseDirectory}/negosud.db");
        }
    }
}
