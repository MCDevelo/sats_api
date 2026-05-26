namespace SchoolERP.Domain.ValueObjects;

public record PlanLimits(int? MaxSchools, int? MaxStudents, bool AllowsWhatsApp, bool AllowsFinancialModule)
{
    public static readonly PlanLimits Trial        = new(1,    50,   false, false);
    public static readonly PlanLimits Starter      = new(1,    200,  false, false);
    public static readonly PlanLimits Professional = new(3,    1000, true,  true);
    public static readonly PlanLimits Enterprise   = new(null, null, true,  true);

    public static PlanLimits FromPlan(string plan) => plan.ToLower() switch
    {
        "starter"      => Starter,
        "professional" => Professional,
        "enterprise"   => Enterprise,
        _              => Trial
    };
}
