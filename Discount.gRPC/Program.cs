using Common.Logging;
using Discount.gRPC.Extensions;
using Discount.gRPC.Repositories;
using Discount.gRPC.Services;
using Discount.Grpc.Protos;
using Serilog;

namespace Discount.gRPC
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Seri Logging
            builder.Host.UseSerilog(SeriLogger.Configure);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddAutoMapper(typeof(Program));

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            app.MigrateDatabase<Program>();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<DiscountService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();

            
        }
    }
}

