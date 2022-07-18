using ApiAgregator.BackgroundService.ExternalApis;
using ApiAgregator.Entities;
using ApiAgregator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ApiAgregator.ExternalApi.WeatherApi;

public class WeatherApi : IExternalApi
{
    public string Name => "currentweather";

    private readonly string[] _days = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

    private readonly string _key;

    public WeatherApi(IOptions<WeatherApiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.Key);

        _key = options.Value.Key;
    }

    public void BuildForm(IFormBuilder builder)
    {
        builder.AddTextInput("city");
        builder.AddSelect("days", _days);
    }

    public Action<IServiceProvider> CreateAction(User user, CronTask cronTask)
    {
        return (serviceProvider) =>
        {
            var request = $"http://api.weatherapi.com/v1/forecast.json?key={_key}&q={cronTask.Parameters["city"]}" +
                $"&days={cronTask.Parameters["days"]}&aqi=no&alerts=no";

            using (var response = new HttpClient().GetAsync(request).Result)
            {

                var result = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result)!;

                var values = new Dictionary<string, List<string>>();

                values.Add("location.name", new List<string> { result.location.name.ToString() });
                values.Add("location.region", new List<string> { result.location.region.ToString() });
                values.Add("location.country", new List<string> { result.location.country.ToString() });
                values.Add("location.lat", new List<string> { result.location.lat.ToString() });
                values.Add("location.lon", new List<string> { result.location.lon.ToString() });
                values.Add("location.tz_id", new List<string> { result.location.tz_id.ToString() });
                values.Add("current.temp_c", new List<string> { result.current.temp_c.ToString() });
                values.Add("current.condition.text", new List<string> { result.current.condition.text.ToString() });
                values.Add("current.condition.icon", new List<string> { result.current.condition.icon.ToString() });
                values.Add("current.condition.code", new List<string> { result.current.condition.code.ToString() });
                values.Add("current.wind_mph", new List<string> { result.current.wind_mph.ToString() });
                values.Add("current.wind_degree", new List<string> { result.current.wind_degree.ToString() });
                values.Add("current.humidity", new List<string> { result.current.humidity.ToString() });
                values.Add("current.feelslike_c", new List<string> { result.current.feelslike_c.ToString() });

                var time = new List<string>();
                var temp_c = new List<string>();
                var wind_mph = new List<string>();
                var wind_degree = new List<string>();
                var humidity = new List<string>();
                var cloud = new List<string>();
                var feelslike_c = new List<string>();
                var heatindex_c = new List<string>();
                var will_it_rain = new List<string>();
                var will_it_snow = new List<string>();

                foreach (var day in result.forecast.forecastday)
                {
                    foreach (var hour in day.hour)
                    {
                        time.Add(hour.time.ToString());
                        temp_c.Add(hour.temp_c.ToString());
                        wind_mph.Add(hour.wind_mph.ToString());
                        wind_degree.Add(hour.wind_degree.ToString());
                        humidity.Add(hour.humidity.ToString());
                        cloud.Add(hour.cloud.ToString());
                        feelslike_c.Add(hour.feelslike_c.ToString());
                        heatindex_c.Add(hour.heatindex_c.ToString());
                        will_it_rain.Add(hour.will_it_rain.ToString());
                        will_it_snow.Add(hour.will_it_snow.ToString());
                    }
                }

                values.Add("forecast.time", time);
                values.Add("forecast.temp_c", temp_c);
                values.Add("forecast.wind_mph", wind_mph);
                values.Add("forecast.wind_degree", wind_degree);
                values.Add("forecast.humidity", humidity);
                values.Add("forecast.cloud", cloud);
                values.Add("forecast.feelslike_c", feelslike_c);
                values.Add("forecast.heatindex_c", heatindex_c);
                values.Add("forecast.will_it_rain", will_it_rain);
                values.Add("forecast.will_it_snow", will_it_snow);

                var emailSender = serviceProvider.GetService<IEmailSenderService>()!;
                var logger = serviceProvider.GetService<ILogger<WeatherApi>>()!;

                var info = $"Weather API call: {result.location.country}, {result.location.name}: {result.current.temp_c} Celsius";

                logger.LogInformation(info);

                emailSender.SendWithCSVFile(user.Email, info, cronTask.Expression.ToString(), values);
            };
        };
    }

    public bool ValidateParametrs(Dictionary<string, string> parametrs)
    {
        return (parametrs.Count == 2) && parametrs.ContainsKey("city") && !parametrs["city"].Contains('&') &&
            !parametrs["city"].Contains('=');
    }
}

public class WeatherApiOptions
{
    public string? Key { get; set; } 
}