using ApiAgregator.WebApi.Utils;

namespace ApiAgregator.WebApi.ServiceCollectionExtensions
{
    public static class ServiceCollectionJwtServiceExtension
    {
        public static IServiceCollection AddJwtService(this IServiceCollection sc,
        Action<JwtServiceOptions> conf)
        {
            sc.Configure(conf);
            sc.AddScoped<JwtService>();
            return sc;
        }
    }
}
