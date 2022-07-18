using ApiAgregator.Services;

namespace ApiAgregator.WebApi.ServiceCollectionExtensions
{
    public static class ServiceCollectionCronTaskSchedulerExtension
    {
        public static IServiceCollection AddCronTaskScheduler(this IServiceCollection sc,
            Action<CronTaskSchedulerOptions> conf)
        {
            sc.Configure(conf);
            sc.AddSingleton<CronTaskScheduler>();
            sc.AddHostedService<BackgroundServiceStarter<CronTaskScheduler>>();
            return sc;
        }
    }
}
