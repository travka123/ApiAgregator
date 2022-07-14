namespace ApiAgregator.BackgroundService.ExternalApis;

public interface IFormBuilder
{
    public void AddSelect(string name, IEnumerable<string> values);
}
