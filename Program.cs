using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((HostBuilderContext, LoggerConfiguration) =>
    LoggerConfiguration.ReadFrom.Configuration(HostBuilderContext.Configuration)
);


var app = builder.Build();


app.GetHangFireDashboard();

await AddRecurrencyJobs(app);

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapOpenApi();

app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/openapi/v1.json", "AiGenda"));

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseSerilogRequestLogging();

app.UseCors();

app.UseAuthorization();

app.UseRateLimiter();

app.UseStaticFiles();

app.MapControllers();

app.Run();





static async System.Threading.Tasks.Task AddRecurrencyJobs( WebApplication app)
{
    var ServiceFactoryScopped = app.Services.GetRequiredService<IServiceScopeFactory>();

    var serviceScope = ServiceFactoryScopped.CreateScope();

    var TaskJobService = serviceScope.ServiceProvider.GetRequiredService<ITaskServiceNotification>();

    RecurringJob.AddOrUpdate("SendTaskDueDateNotification", () => TaskJobService.SendTaskNotification(), Cron.Daily);
}


