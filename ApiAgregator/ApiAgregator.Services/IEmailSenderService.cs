namespace ApiAgregator.Services;

public interface IEmailSenderService
{
    void Send(string to, string subject, string htmlBody);
    void SendWithCSVFile(string to, string subject, string htmlBody, Dictionary<string, List<string>> values);
}
