using ApiAgregator.BackgroundService.ExternalApis;
using ApiAgregator.Entities;
using ApiAgregator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiAgregator.ExternalApi.ChuckNorris;

public class ChuckNorrisApi : IExternalApi
{
    private readonly string[] _categories = new string[] { "animal", "career", "dev", "food", "history", "sport", "travel" };
    public string Name => "chucknorris";

    public void BuildForm(IFormBuilder builder)
    {
        builder.AddSelect("category", _categories);
    }

    public Action<IServiceProvider> CreateAction(User user, CronTask cronTask)
    {
        return (serviceProvider) =>
        {
            var request = $"https://api.chucknorris.io/jokes/random?category={cronTask.Parameters["category"]}";

            using (var response = new HttpClient().GetAsync(request).Result)
            {

                response.EnsureSuccessStatusCode();

                var result = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result)!;

                var values = new Dictionary<string, List<string>>();
                values.Add("created_at", new List<string>() { result["created_at"].ToString() });
                values.Add("id", new List<string>() { result["id"].ToString() });
                values.Add("updated_at", new List<string>() { result["updated_at"].ToString() });
                values.Add("url", new List<string>() { result["url"].ToString() });
                values.Add("value", new List<string>() { result["value"].ToString() });

                var emailSender = serviceProvider.GetService<IEmailSenderService>()!;
                var logger = serviceProvider.GetService<ILogger<ChuckNorrisApi>>()!;

                var info = $"Chuck Norris API call: {result["value"]} ";

                logger.LogInformation(info);

                emailSender.SendWithCSVFile(user.Email, info, cronTask.Expression.ToString(), values);
            };
        };
    }

    public bool ValidateParametrs(Dictionary<string, string> parametrs)
    {
        if (parametrs.Count != 1)
        {
            return false;
        }

        if (!parametrs.TryGetValue("category", out var category))
        {
            return false;
        }

        return _categories.Contains(category);
    }
}