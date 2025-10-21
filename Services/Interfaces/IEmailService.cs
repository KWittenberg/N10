namespace N10.Services.Interfaces;

public interface IEmailService
{
    Task<Result> SendEmail(EmailInput input);
}