using Microsoft.OpenApi.Models;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
            });

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            app.MigrateDatabase<OrderContext>((context, services) =>
            {
                var logger = services.GetService<ILogger<OrderContextSeed>>();
                OrderContextSeed
                    .SeedAsync(context, logger)
                    .Wait();
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}