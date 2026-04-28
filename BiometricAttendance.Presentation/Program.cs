using BiometricAttendance.Application.Interfaces;
using BiometricAttendance.Application.Services;
using BiometricAttendance.Domain.Entities;
using BiometricAttendance.Domain.Repositories;
using BiometricAttendance.Presentation;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.Path = "/swagger";
    });

    app.UseReDoc(options =>
    {
        options.Path = "/redoc";
        options.DocumentPath = "/swagger/v1/swagger.json";
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("Hangfire:Username"),
            Pass = app.Configuration.GetValue<string>("Hangfire:Password")
        }
    ],
    DashboardTitle = "Hangfire Dashboard",
    IsReadOnlyFunc = context => true
});

RecurringJob.AddOrUpdate<IRecurringJobService>(
    "RemoveExpiredRefreshTokens",
    x => x.RemoveExpiredRefreshTokenAsync(),
    Cron.DayInterval(RefreshToken.AutoRemoveAfterDays));

RecurringJob.AddOrUpdate<IInstructorPassService>(
    "CreateNewPass",
    x => x.CreateNewPassAsync(),
    Cron.Daily);

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
