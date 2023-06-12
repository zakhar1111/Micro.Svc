using Microsoft.OpenApi.Models;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Design;
using MassTransit;
using Ordering.API.EventBusConsumer;
using EventBus.Messages.Common;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Serilog;
using Common.Logging;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Seri Logging
            builder.Host.UseSerilog(SeriLogger.Configure);

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



            // MassTransit-RabbitMQ Configuration
            builder.Services.AddMassTransit(config => {

                config.AddConsumer<BasketCheckoutConsumer>();

                config.UsingRabbitMq((ctx, cfg) => {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
                    //cfg.UseHealthCheck(ctx);

                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c => {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                    });
                });
            });
            builder.Services.AddSingleton<IHostedService, MassTransitConsoleHostedService>(); //builder.Services.AddMassTransitHostedService();
            builder.Services.AddScoped<BasketCheckoutConsumer>();
            builder.Services.AddAutoMapper(typeof(Program));

            // HealthCheck
            builder.Services.AddHealthChecks().AddDbContextCheck<OrderContext>();

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

            // health Check response
            app.MapHealthChecks("/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();
        }
    }
}