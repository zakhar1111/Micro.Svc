using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "swn";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = "swn";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        /* //for migration
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string connection = @"Server = localhost; Database = OrderDb; User Id = sa; Password = SwN12345678; TrustServerCertificate=True";
            options.UseSqlServer(connection, b => b.MigrationsAssembly("Ordering.API"));
        }
        */
    }
}

/* //migration script
       CREATE TABLE [Orders] (
          [Id] int NOT NULL IDENTITY,
          [UserName] nvarchar(max) NOT NULL,
          [TotalPrice] decimal(18,2) NOT NULL,
          [FirstName] nvarchar(max) NOT NULL,
          [LastName] nvarchar(max) NOT NULL,
          [EmailAddress] nvarchar(max) NOT NULL,
          [AddressLine] nvarchar(max) NOT NULL,
          [Country] nvarchar(max) NOT NULL,
          [State] nvarchar(max) NOT NULL,
          [ZipCode] nvarchar(max) NOT NULL,
          [CardName] nvarchar(max) NOT NULL,
          [CardNumber] nvarchar(max) NOT NULL,
          [Expiration] nvarchar(max) NOT NULL,
          [CVV] nvarchar(max) NOT NULL,
          [PaymentMethod] int NOT NULL,
          [CreatedBy] nvarchar(max) NOT NULL,
          [CreatedDate] datetime2 NOT NULL,
          [LastModifiedBy] nvarchar(max) NOT NULL,
          [LastModifiedDate] datetime2 NULL,
          CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
      );
 */