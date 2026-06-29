using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AI_genda_API.HealthChecks;

public class EmailServiceHealthCheck( IOptions<MailSettings> options) : IHealthCheck
{
    private readonly MailSettings _MailSettings = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var SmtpClient = new SmtpClient();

            SmtpClient.Connect(_MailSettings.Host, _MailSettings.port, SecureSocketOptions.StartTls);

            SmtpClient.Authenticate(_MailSettings.User, _MailSettings.Pass);

            return HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Email Service is unhealthy!");
        }

    }
}
