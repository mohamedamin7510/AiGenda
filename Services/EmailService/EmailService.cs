using AI_genda_API.Api.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AI_genda_API.Services.EmailService;

public class EmailService(ILogger<EmailService> logger ,IOptions<MailSettings> mailsettings ) : IEmailSender
{
    private readonly ILogger<EmailService> _Logger = logger;
    private readonly MailSettings _Mailsettings = mailsettings.Value;

    public async System.Threading.Tasks.Task SendEmailAsync(string email, string subject, string htmlMessage)
    {

        //header assigned
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_Mailsettings.User),
            Subject = subject
        };
        // the reciever email 
        message.To.Add(MailboxAddress.Parse(email));

        //Email Body
        var messageBody = new BodyBuilder()
        {
            HtmlBody = htmlMessage
        };

        message.Body = messageBody.ToMessageBody();

        _Logger.LogInformation("Email Send Service ");

        using (var smtpclient = new SmtpClient())
        {
            smtpclient.Connect(_Mailsettings.Host, _Mailsettings.port, SecureSocketOptions.StartTls);

            smtpclient.Authenticate(_Mailsettings.User, _Mailsettings.Pass);

            await smtpclient.SendAsync(message);

            smtpclient.Disconnect(true);
        }



    }



}
