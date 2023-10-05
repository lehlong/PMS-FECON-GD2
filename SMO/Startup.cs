using Microsoft.Owin;
using Owin;
using Hangfire;
using SMO.Service.AD;

[assembly: OwinStartupAttribute(typeof(SMO.Startup))]
namespace SMO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();

            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireDB");

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });
            app.UseHangfireServer();

            InitBackgroundJob();
        }

        protected void InitBackgroundJob()
        {
            var service = new SystemConfigService();
            service.GetConfig();
            service.ObjDetail.JOB_SEND_EMAIL = service.ObjDetail.JOB_SEND_EMAIL == 0 ? 5 : service.ObjDetail.JOB_SEND_EMAIL;

            RecurringJob.AddOrUpdate("SendEmail", () => SMOUtilities.SendEmail(), Cron.MinuteInterval(1));
            //RecurringJob.AddOrUpdate("UpdateQuantityEGAS", () => SMOUtilities.UpdateQuantityEGAS(), Cron.MinuteInterval(1));
        }
    }
}
