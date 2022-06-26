using Prometheus;
using ServiceA;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Configuration.GetSection("Services").Get<Services>();

var hcBuilder = builder.Services
    .AddHealthChecks();
var healthEndpoint = new Uri("/health", UriKind.Relative);
foreach (var service in services?.Urls ?? new List<string>())
{
    hcBuilder.AddUrlGroup(new Uri(new Uri(service), healthEndpoint), name: service);
}
hcBuilder.ForwardToPrometheus();

var app = builder.Build();
app.Urls.Add("http://*:8080");
app.UseRouting();
app.UseHttpMetrics();
app.MapHealthChecks("/health");
app.UseEndpoints(endpoints => endpoints.MapMetrics());
app.Run();
