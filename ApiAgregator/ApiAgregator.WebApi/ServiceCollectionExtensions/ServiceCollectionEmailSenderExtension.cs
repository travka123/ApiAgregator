using ApiAgregator.Services;

namespace ApiAgregator.WebApi.ServiceCollectionExtensions;

public static class ServiceCollectionEmailSenderExtension
{
    public static IServiceCollection AddEmailSenderService(this IServiceCollection sc,
        Action<EmailSenderServiceOptions> conf)
    {
        sc.Configure(conf);
        sc.AddSingleton<EmailSenderService>();
        return sc;
    }
}
