using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Infrastructure.Jobs;
using SchoolERP.Infrastructure.Persistence;
using SchoolERP.Infrastructure.Persistence.Interceptors;
using SchoolERP.Infrastructure.Reports;
using SchoolERP.Infrastructure.Services;

namespace SchoolERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // AuditInterceptor is scoped so it can access ICurrentUserService and IHttpContextAccessor.
        // DbContext must be registered with the factory overload to receive scoped services.
        services.AddScoped<AuditInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<INotificationSender, CompositeNotificationSender>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        services.AddScoped<IReportGeneratorService, QuestPdfReportGeneratorService>();
        services.AddScoped<IStorageService, LocalStorageService>();
        services.AddScoped<IPlanService, PlanService>();

        // Hangfire
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.Queues = ["default"];
        });

        // Hangfire job classes (transient — Hangfire creates a scope per execution)
        services.AddTransient<OverdueInvoiceMarkingJob>();
        services.AddTransient<AttendanceReminderJob>();
        services.AddTransient<NotificationRetryJob>();
        services.AddTransient<MonthlyInvoiceGenerationJob>();
        services.AddScoped<HangfireJobScheduler>();

        return services;
    }
}
