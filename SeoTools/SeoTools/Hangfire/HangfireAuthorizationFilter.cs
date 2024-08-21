using Hangfire.Dashboard;

namespace SeoTools.Hangfire;

public class HangfireAuthorizationFilter:IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {

        // Allow all users to see the Dashboard (potentially dangerous).
        return true;

    }
}