using Basket.API;
using Basket.API.GrpcServices;
using Basket.API.Repos;
using Common.Logging;
using DiscountProtoServiceClient;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Seri Logging
            builder.Host.UseSerilog(SeriLogger.Configure);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
            });

            // Redis 
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
            });

            // Basket Repo
            builder.Services.AddScoped<IBasketRepository, BasketRepository>();

            // Grpc 
            builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));
            builder.Services.AddScoped<DiscountGrpcService>();

            // MassTransit-RabbitMQ 
            builder.Services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
                    //TODO: cfg.UseHealthCheck(ctx);
                });
            });
            //builder.Services.AddMassTransitHostedService();  
            builder.Services.AddSingleton<IHostedService, MassTransitConsoleHostedService>();

            //AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            // HealthCheck
            var redisStrCon = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
            builder.Services.AddHealthChecks()
                .AddRedis(redisStrCon, "Redis Health", HealthStatus.Degraded);
            // .AddRedis(builder.Configuration["CacheSettings: ConnectionString"], "Redis Health", HealthStatus.Degraded);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            

            
            app.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.Run();

        }
    }
}
