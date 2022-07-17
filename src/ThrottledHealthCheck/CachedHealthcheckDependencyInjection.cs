using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ThrottledHealthCheck;

/// <summary>
/// методы регистрации сервиса троттлящих хелсчеков
/// </summary>
public static class CachedHealthcheckDependencyInjection
{
    /// <summary>
    /// Заменить используемый healthcheck service на троттлящий
    /// </summary>
    /// <param name="services">di-контейнер, в котором собственно регистрируются сервисы</param>
    /// <param name="throttleInterval">интервал троттлинга</param>
    public static void ThrottleHealthChecks(this IServiceCollection services, TimeSpan throttleInterval)
    {
        services.Configure<ThrottleOptions>(opts => opts.ThrottleInterval = throttleInterval);
        var descriptor = new ServiceDescriptor(typeof(HealthCheckService), typeof(CachedHealthCheckService), ServiceLifetime.Singleton);
        services.Replace(descriptor);
    }

    /// <summary>
    /// регистрация троттлящих хелсчеков с интервалом по умолчанию - 15 секунд
    /// </summary>
    /// <param name="services">di-контейнер, в котором собственно регистрируются сервисы</param>
    public static void ThrottleHealthChecks(this IServiceCollection services)
    {
        ThrottleHealthChecks(services, TimeSpan.FromSeconds(15));
    }
}
