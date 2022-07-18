using ApiAgregator.Services;

namespace ApiAgregator.WebApi.ServiceCollectionExtensions;

public static class ServiceCollectionEmailValidationServiceExtension
{
    public static IServiceCollection AddEmailValidationService(this IServiceCollection sc,
        Action<EmailValidationServiceOptions> conf)
    {
        sc.Configure(conf);
        sc.AddScoped<EmailValidationService>();
        return sc;
    }
}
