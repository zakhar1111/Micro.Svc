using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Serilog;
using Common.Logging;

var builder = WebApplication.CreateBuilder(args);

// Seri Logging
builder.Host.UseSerilog(SeriLogger.Configure);


builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.AddJsonFile($"ocelot.{env}.json");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
//app.UseHttpsRedirection();



app.MapGet("/weatherforecast", () => Results.Ok("hey ocelot"));

app.UseOcelot().Wait(); 

app.Run();

