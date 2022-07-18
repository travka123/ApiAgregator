using ApiAgregator.BackgroundService.ExternalApis;

namespace ApiAgregator.WebApi.Models.Response
{
    public class FormBuilder : IFormBuilder
    {
        public Dictionary<string, dynamic> From { get; private set; } = new();

        public void AddSelect(string name, IEnumerable<string> values)
        {
            From.Add(name, new { type = "select", values = values.ToList() });
        }

        public void AddTextInput(string name)
        {
            From.Add(name, new { type = "text" });
        }
    }
}
