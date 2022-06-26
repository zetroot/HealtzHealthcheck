using System.Runtime.CompilerServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using HealthHealthcheck = HealthHealthcheck.HealthHealthcheck;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Configuration.GetSection("Services").Get<List<string>>();

var hcBuilder = builder.Services
    .AddHealthChecks();
var healthEndpoint = new Uri("/health", UriKind.Relative);
foreach (var service in services ?? new List<string>())
{
    var url = new Uri(service);
    hcBuilder.Add(new HealthCheckRegistration(url.Host,
        sp => new global::HealthHealthcheck.HealthHealthcheck(sp.GetRequiredService<IHttpClientFactory>(), new(url)),
        HealthStatus.Unhealthy,
        Enumerable.Empty<string>()));
}
hcBuilder.ForwardToPrometheus();

var app = builder.Build();
app.Urls.Add("http://*:8080");
app.UseRouting();
app.UseHttpMetrics();
app.MapHealthChecks("/health");
app.UseEndpoints(endpoints => endpoints.MapMetrics());
app.Run();
