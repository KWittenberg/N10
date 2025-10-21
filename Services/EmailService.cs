using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace N10.Services;

public class EmailService(IOptions<GmailOptions> gmail) : IEmailService
{
    public async Task<Result> SendEmail(EmailInput input)
    {
        MimeMessage email = new();
        email.From.Add(MailboxAddress.Parse(gmail.Value.Username));
        email.To.Add(MailboxAddress.Parse(input.To));
        email.Subject = input.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = input.Body };

        using SmtpClient smtp = new();

        // Use local SMTP server for testing
        // await smtp.ConnectAsync("localhost", 1024);

        // Gmail SMTP
        await smtp.ConnectAsync(gmail.Value.Host, 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(gmail.Value.Username, gmail.Value.Password);

        await smtp.SendAsync(email);
        //var sendResponse = await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        //return !sendResponse.Contains("250") ? new ServiceResponse(HttpStatusCode.InternalServerError, $"Failed to send email: {sendResponse}")
        //    : new ServiceResponse(HttpStatusCode.OK, $"Email Sent Successfully - {DateTime.Now.ToString("f")}");


        return Result.Ok($"Email Sent Successfully - {DateTime.Now:f}");
    }

    //public Task SendEmail(string to, string subject, string body)
    //{
    //    var email = new MimeMessage();
    //    email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
    //    email.To.Add(MailboxAddress.Parse(to));
    //    email.Subject = subject;
    //    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

    //    //send email
    //    using (var smtp = new SmtpClient())
    //    {
    //        smtp.Connect(config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
    //        smtp.Authenticate(config.GetSection("EmailUsername").Value, config.GetSection("EmailPassword").Value);
    //        smtp.Send(email);
    //        smtp.Disconnect(true);
    //    }
    //    return Task.CompletedTask;
    //}



    //public void SendEmailMessage(ContactBinding model)
    //{
    //    model.To = config.GetSection("EmailUsername").Value;
    //    model.Subject = "Bolta WebShop Message from: " + model.MessageName;
    //    model.Body = "<h3>" + model.MessageName + "</h3><br/>" + model.MessageEmail + "<br/><br/><h4>" + model.Body + "</h4>";
    //    var email = new MimeMessage();
    //    email.From.Add(MailboxAddress.Parse(config.GetSection("EmailUsername").Value));
    //    email.To.Add(MailboxAddress.Parse(model.To));
    //    email.Subject = model.Subject;
    //    email.Body = new TextPart(TextFormat.Html) { Text = model.Body };

    //    using var smtp = new SmtpClient();
    //    smtp.Connect(config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
    //    smtp.Authenticate(config.GetSection("EmailUsername").Value, config.GetSection("EmailPassword").Value);
    //    smtp.Send(email);
    //    smtp.Disconnect(true);
    //}
}