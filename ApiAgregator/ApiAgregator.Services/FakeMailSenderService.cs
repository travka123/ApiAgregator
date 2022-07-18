namespace ApiAgregator.Services;

public class FakeMailSenderService : IEmailSenderService
{
    public void Send(string to, string subject, string htmlBody)
    {
        Console.WriteLine(to);
        Console.WriteLine(subject);
        Console.WriteLine(htmlBody);
    }

    public void SendWithCSVFile(string to, string subject, string htmlBody, Dictionary<string, List<string>> values)
    {
        Console.WriteLine(to);
        Console.WriteLine(subject);
        Console.WriteLine(htmlBody);
        Console.WriteLine("+ CSV FILE");
    }
}
