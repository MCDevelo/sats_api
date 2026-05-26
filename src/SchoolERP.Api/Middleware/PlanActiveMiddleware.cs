using System.Net;
using System.Text.Json;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Api.Middleware;

public class PlanActiveMiddleware
{
    private readonly RequestDelegate _next;

    public PlanActiveMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUser, IPlanService planService)
    {
        // Skip unauthenticated requests
        if (!currentUser.IsAuthenticated)
        {
            await _next(context);
            return;
        }

        // Skip PlatformAdmin — they are exempt from tenant plan checks
        if (string.Equals(currentUser.Role, "PlatformAdmin", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Skip read-only methods
        var method = context.Request.Method;
        if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
        {
            await _next(context);
            return;
        }

        var tenantId = currentUser.TenantId;
        if (tenantId is null)
        {
            await _next(context);
            return;
        }

        var isActive = await planService.IsPlanActiveAsync(tenantId.Value, context.RequestAborted);
        if (!isActive)
        {
            context.Response.StatusCode = (int)HttpStatusCode.PaymentRequired;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new
            {
                success = false,
                error = new
                {
                    code = "Tenant.PlanExpired",
                    message = "Su período de prueba ha vencido. Contacte con soporte para activar su suscripción."
                }
            });

            await context.Response.WriteAsync(body);
            return;
        }

        await _next(context);
    }
}
