using ApiAgregator.BackgroundService.ExternalApis;
using ApiAgregator.Entities;
using ApiAgregator.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiAgregator.ExternalApi.CovidApi;

public class CovidApi : IExternalApi
{
    private readonly string _xRapidApiKey;
    private readonly string _xRapidApiHost;

    public CovidApi(IOptions<CovidApiOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.XRapidApiKey);
        ArgumentNullException.ThrowIfNull(options.Value.XRapidApiHost);

        _xRapidApiKey = options.Value.XRapidApiKey;
        _xRapidApiHost = options.Value.XRapidApiHost;
    }

    public string Name => "covid";

    private static string[]? _countries;
    private readonly object _countriesLock = new object();
    private string[] GetCountries()
    {
        if (_countries is null)
        {
            lock (_countriesLock)
            {
                if (_countries is null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("https://covid-193.p.rapidapi.com/countries"),
                        Headers =
                        {
                            { "X-RapidAPI-Key", _xRapidApiKey },
                            { "X-RapidAPI-Host", _xRapidApiHost },
                        },
                    };
                    using (var response = client.SendAsync(request).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        var result = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result)!;
                        _countries = ((JArray)result.response).Select(c => (string)c!).ToArray()!;
                    }
                }
            }
        }

        if (_countries is null)
            throw new Exception();

        return _countries;
    }

    public void BuildForm(IFormBuilder builder)
    {
        GetCountries();

        if (_countries is null)
            builder.AddSelect("country", new string[] { "error" });
        else
            builder.AddSelect("country", _countries);
    }

    public Action<IServiceProvider> CreateAction(User user, CronTask cronTask)
    {
        return (serviceProvider) =>
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://covid-193.p.rapidapi.com/statistics?country={cronTask.Parameters["country"]}"),
                Headers =
                {
                    { "X-RapidAPI-Key", _xRapidApiKey },
                    { "X-RapidAPI-Host", _xRapidApiHost },
                },
            };
            using (var response = client.SendAsync(request).Result)
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<dynamic>(body)!;

                var values = new Dictionary<string, List<string>>();

                values.Add("continent", new List<string>() { result.response[0].continent.ToString() });
                values.Add("country", new List<string>() { result.response[0].country.ToString() });
                values.Add("population", new List<string>() { result.response[0].population.ToString() });
                values.Add("cases.new", new List<string>() { result.response[0].cases["new"].ToString() });
                values.Add("cases.active", new List<string>() { result.response[0].cases["active"].ToString() });
                values.Add("cases.critical", new List<string>() { result.response[0].cases["critical"].ToString() });
                values.Add("cases.recovered", new List<string>() { result.response[0].cases["recovered"].ToString() });
                values.Add("cases.1M_pop", new List<string>() { result.response[0].cases["1M_pop"].ToString() });
                values.Add("cases.total", new List<string>() { result.response[0].cases["total"].ToString() });
                values.Add("deaths.new", new List<string>() { result.response[0].deaths["new"].ToString() });
                values.Add("deaths.1M_pop", new List<string>() { result.response[0].deaths["1M_pop"].ToString() });
                values.Add("deaths.total", new List<string>() { result.response[0].deaths["total"].ToString() });
                values.Add("tests.1M_pop", new List<string>() { result.response[0].tests["1M_pop"].ToString() });
                values.Add("tests.total", new List<string>() { result.response[0].tests["total"].ToString() });
                values.Add("day", new List<string>() { result.response[0].day.ToString() });
                values.Add("time", new List<string>() { result.response[0].time.ToString() });

                var emailSender = serviceProvider.GetService<IEmailSenderService>()!;
                var logger = serviceProvider.GetService<ILogger<CovidApi>>()!;

                var info = $"Covid API call: {result.response[0].country.ToString()} total cases: {result.response[0].cases["total"].ToString()}";

                logger.LogInformation(info);

                emailSender.SendWithCSVFile(user.Email, info, cronTask.Expression.ToString(), values);
            }
        };
    }

    public bool ValidateParametrs(Dictionary<string, string> parametrs)
    {
        GetCountries();

        return (_countries is not null) && (parametrs.Count == 1) && parametrs.ContainsKey("country") &&
            _countries.Contains(parametrs["country"]);
    }
}

public class CovidApiOptions
{
    public string? XRapidApiKey { get; set; }
    public string? XRapidApiHost { get; set; }
}