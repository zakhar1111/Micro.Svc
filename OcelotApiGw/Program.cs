using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

