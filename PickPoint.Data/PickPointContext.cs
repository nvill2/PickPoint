using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using PickPoint.Data.Entities;

namespace PickPoint.Data
{
    public class PickPointContext : DbContext
    {
        public PickPointContext(DbContextOptions<PickPointContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<DeliveryPoint> DeliveryPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeliveryPoint>()
                .HasKey(d => d.Number);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryPoint)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DeliveryPointNumber)
                .HasPrincipalKey(d => d.Number);

            modelBuilder.Entity<DeliveryPoint>().HasData(
                new DeliveryPoint { Address = "Varshavskoye Highway, 160", Number = "1111-111", Status = true },
                new DeliveryPoint { Address = "Vostryakovsky Drive, 17к3А", Number = "2222-222", Status = false },
                new DeliveryPoint { Address = "Varshavskoye Highway, 141к9А", Number = "3333-333", Status = true },
                new DeliveryPoint { Address = "Rossoshanskaya Street, 2с1", Number = "4444-444", Status = false },
                new DeliveryPoint { Address = "Kasimovskaya Street, 4", Number = "5555-555", Status = true });

            modelBuilder.Entity<Order>().HasData(
                new Order(1, "Item1|Item2|Item3", 100, "Ivanov I I", "+7777-777-77-77", OrderStatus.AtWarehouse),
                new Order(2, "Item5|Item6|Item7|Item8|Item9", 400, "Petrov P P", "+7888-888-88-88", OrderStatus.AtPickPoint, "1111-111"),
                new Order(3, "Item1|Item7|Item8", 150, "Sidorov S S", "+7999-999-99-99", OrderStatus.Registered),
                new Order(4, "Item2|Item6|Item9|Item4", 200, "Mikhailov M M", "+7666-666-66-66", OrderStatus.AtPickPoint, "5555-555"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
