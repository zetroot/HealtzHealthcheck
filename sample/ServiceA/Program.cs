using Prometheus;
using ThrottledHealthCheck;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Configuration.GetSection("Services").Get<List<string>>();


var hcBuilder = builder.Services
    .AddHealthChecks();
if (builder.Configuration.GetSection("THROTTLE").Get<bool>()) 
    builder.Services.ThrottleHealthChecks();
var healthEndpoint = new Uri("/health", UriKind.Relative);
foreach (var service in services ?? new List<string>())
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
