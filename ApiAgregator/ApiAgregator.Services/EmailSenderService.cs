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
            Send(mail);
        }
    }

    public void SendWithCSVFile(string to, string subject, string htmlBody, Dictionary<string, List<string>> values)
    {
        using (var mail = new MailMessage())
        {
            mail.From = new MailAddress(_from);
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = htmlBody;
            mail.IsBodyHtml = true;

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    mail.Attachments.Add(new Attachment(AsCVS(writer, values), "values.csv", "text/csv"));
                    Send(mail);
                }
            }
        }
    }

    private void Send(MailMessage mail)
    {
        using (var smpt = new SmtpClient(_smtpHost, _smtpPort))
        {
            smpt.Credentials = _credential;
            smpt.EnableSsl = true;
            smpt.Send(mail);
        }
    }

    private Stream AsCVS(StreamWriter writer, Dictionary<string, List<string>> values)
    {
        long pos = writer.BaseStream.Position;

        int rows = ValidateCSV(values);
  
        writer.Write(String.Join(", ", values.Keys.Select(k => k.Replace(',', '.'))));
        writer.Write("\n");

        for (int i = 0; i < rows; i++)
        {
            writer.Write(String.Join(", ", values.Values.Select((v) => (v.Count == 1 ? v[0] : v[i]).Replace(',', '.'))));
            writer.Write("\n");
        }

        writer.Flush();
        writer.BaseStream.Position = pos;
        return writer.BaseStream;
    }

    private int ValidateCSV(Dictionary<string, List<string>> values)
    {
        int rows = 1;

        foreach (var kv in values)
        {
            int count = kv.Value.Count;

            if (count == 0)
                throw new Exception();

            if (count != 1)
            {
                if (rows == 1)
                {
                    rows = count;
                }
                else if (rows != count)
                {
                    throw new Exception();
                }
            }
        }

        return rows;
    }
}

public class EmailSenderServiceOptions
{
    public string? From { get; set; }
    public NetworkCredential? Credential { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
}
