namespace N10.Services.Interfaces;

public interface IEmailService
{
    Task<ServiceResponse> SendEmail(EmailDto input);
}