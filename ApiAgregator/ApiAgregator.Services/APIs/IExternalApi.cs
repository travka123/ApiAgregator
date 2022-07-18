using ApiAgregator.Entities;

namespace ApiAgregator.BackgroundService.ExternalApis;

public interface IExternalApi
{
    public string Name { get; }
    public bool ValidateParametrs(Dictionary<string, string> parametrs);
    public Action<IServiceProvider> CreateAction(User user, CronTask cronTask);
    public void BuildForm(IFormBuilder builder);
}