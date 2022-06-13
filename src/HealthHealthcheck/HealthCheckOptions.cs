namespace HealthHealthcheck;

public class HealthCheckOptions
{
    public HealthCheckOptions(Uri endpointUri)
    {
        EndpointUri = endpointUri;
    }

    public Uri EndpointUri { get; }
}