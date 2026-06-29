namespace AI_genda_API.Extenstions;

public static class HangFireExtentstion
{
    public static void GetHangFireDashboard(this WebApplication app)
    {
        app.UseHangfireDashboard("/jobs/ai-genda/dash-board",
            new DashboardOptions()
            {
                Authorization =
                [
                    new HangfireCustomBasicAuthenticationFilter()
                    {
                        User = app.Configuration["HangfireAuth:User"]!.ToString(),
                        Pass = app.Configuration["HangfireAuth:Pass"]!.ToString()
                    }
                ],
                DashboardTitle = "AiGenda-Jobs-DashBoard"
            }
        );
    }

}
