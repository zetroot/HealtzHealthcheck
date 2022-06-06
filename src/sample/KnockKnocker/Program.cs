var url = Environment.GetEnvironmentVariable("TARGET");
var httpClient = new HttpClient();
while (true)
{
    await httpClient.GetAsync(url);
    await Task.Delay(TimeSpan.FromSeconds(1));
}
