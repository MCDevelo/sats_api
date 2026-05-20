using Hangfire.Dashboard;

namespace SchoolERP.Api.Infrastructure;

/// <summary>
/// Restricts Hangfire dashboard access to authenticated Admin users in production.
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole("Admin");
    }
}
