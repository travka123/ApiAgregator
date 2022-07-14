namespace ApiAgregator.Services;

public interface IEmailSenderService
{
    void Send(string to, string subject, string htmlBody);
}
