using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() 
                {
                    UserName = "swn", 
                    FirstName = "Zakhar", 
                    LastName = "Seva", 
                    EmailAddress = "zakharseva1980@gmail.com", 
                    AddressLine = "Dnipro", 
                    Country = "Ukraine", 
                    TotalPrice = 350,
                    State = "",
                    ZipCode =  "",
                    CardName = "",
                    CardNumber = "",  
                    Expiration  =  "",
                    CVV = "",

                    CreatedBy = "seeding",
                    CreatedDate = DateTime.Now,
                    LastModifiedBy = "seeding",
                    LastModifiedDate = DateTime.Now
                }
            };
        }
    }
}
