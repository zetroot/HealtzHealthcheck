namespace ThrottledHealthCheck;

/// <summary>
/// настройки троттлинга
/// </summary>
public class ThrottleOptions
{
    /// <summary>
    /// интревал для троттлинга значения Health report
    /// </summary>
    public TimeSpan ThrottleInterval { get; set; }
}