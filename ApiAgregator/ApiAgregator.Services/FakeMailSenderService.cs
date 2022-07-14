namespace ApiAgregator.Services;

public class FakeMailSenderService : IEmailSenderService
{
    public void Send(string to, string subject, string htmlBody)
    {
        Console.WriteLine(to);
        Console.WriteLine(subject);
        Console.WriteLine(htmlBody);
    }
}
