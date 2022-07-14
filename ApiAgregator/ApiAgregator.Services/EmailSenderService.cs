using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ApiAgregator.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly string _from;
    private readonly NetworkCredential _credential;
    private readonly string _smtpHost;
    private readonly int _smtpPort;

    public EmailSenderService(IOptions<EmailSenderServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.From);
        ArgumentNullException.ThrowIfNull(options.Value.Credential);
        ArgumentNullException.ThrowIfNull(options.Value.SmtpHost);
        ArgumentNullException.ThrowIfNull(options.Value.SmtpPort);

        _from = options.Value.From;
        _credential = options.Value.Credential;
        _smtpHost = options.Value.SmtpHost;
        _smtpPort = options.Value.SmtpPort.Value;
    }

    public void Send(string to, string subject, string htmlBody)
    {
        using (var mail = new MailMessage())
        {
            mail.From = new MailAddress(_from);
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = htmlBody;
            mail.IsBodyHtml = true;

            using (var smpt = new SmtpClient(_smtpHost, _smtpPort))
            {
                smpt.Credentials = _credential;
                smpt.EnableSsl = true;
                smpt.Send(mail);
            }
        }
    }
}

public class EmailSenderServiceOptions
{
    public string? From { get; set; } 
    public NetworkCredential? Credential { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
}
