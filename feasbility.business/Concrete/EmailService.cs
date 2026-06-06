using System.Net;
using System.Net.Mail;
using feasibility.Business.Abstract;
using Microsoft.Extensions.Configuration;

namespace feasibility.Business.Concrete;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        var senderEmail = _configuration["MailSettings:PrivateMail"]
            ?? throw new InvalidOperationException("MailSettings:PrivateMail ayarı bulunamadı.");
        var senderPassword = _configuration["MailSettings:PrivatePassword"]
            ?? throw new InvalidOperationException("MailSettings:PrivatePassword ayarı bulunamadı.");

        using var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderEmail, senderPassword)
        };

        using var message = new MailMessage(senderEmail, toEmail, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message, ct);
    }

    public Task SendResetCodeAsync(string toEmail, string code, CancellationToken ct = default)
    {
        var body = $@"
            <div style='font-family:Inter,Arial,sans-serif;background:#f3f4f6;padding:32px;'>
                <div style='max-width:480px;margin:0 auto;background:#ffffff;border-radius:12px;padding:32px;border:1px solid #e5e7eb;'>
                    <h2 style='color:#003e7a;margin:0 0 16px 0;'>Şifre Sıfırlama Kodu</h2>
                    <p style='color:#374151;font-size:14px;line-height:1.6;margin:0 0 24px 0;'>
                        Aşağıdaki 6 haneli kodu kullanarak şifrenizi sıfırlayabilirsiniz. Bu kod 1 dakika içinde geçersiz olacaktır.
                    </p>
                    <div style='background:#f9fafb;border:1px solid #e5e7eb;border-radius:8px;padding:16px;text-align:center;font-size:32px;font-weight:700;letter-spacing:0.5em;color:#003e7a;'>
                        {code}
                    </div>
                    <p style='color:#6b7280;font-size:12px;margin:24px 0 0 0;'>
                        Bu talebi siz yapmadıysanız, bu e-postayı dikkate almayın.
                    </p>
                </div>
            </div>";

        return SendAsync(toEmail, "Şifre Sıfırlama Kodu", body, ct);
    }
}
