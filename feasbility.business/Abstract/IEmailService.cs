namespace feasibility.Business.Abstract;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    Task SendResetCodeAsync(string toEmail, string code, CancellationToken ct = default);
}
