using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.Extensions.Emails;
using MailKit.Net.Smtp;
namespace SurveyBasket.Health;

public class MailProviderHealthCheck(IOptions<EmailSettings> options) : IHealthCheck
{
    private readonly EmailSettings _options = options.Value;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var smtpClient = new SmtpClient();
            smtpClient.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            smtpClient.Authenticate(_options.Email, _options.Password);
            return Task.FromResult(HealthCheckResult.Healthy("Sucess connect with Mail provider"));

        }
        catch (System.Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
        }
    }
}
