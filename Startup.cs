using Hangfire;
using Owin;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using System;
using Microsoft.Owin;
using Help_Desk_2.BackgroundJobs;
using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Utilities;

[assembly: OwinStartup(typeof(Help_Desk_2.Startup))]
namespace Help_Desk_2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var sqlOptions = new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(15) // Default value
            };
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(HelpDeskContext.ConnString, sqlOptions);

            //app.UseHangfireDashboard();
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    AuthorizationFilters = new[] { new HangFileAuthorize() }
            //});

            DashboardOptions options = new DashboardOptions {
                AuthorizationFilters = new [] {
                    new AuthorizationFilter { Roles = UserRoles.DomainAdminRole },
                }
            };

            app.UseHangfireDashboard("/hangfire", options);

            app.UseHangfireServer();

            //RecurringJob.AddOrUpdate("ContentNotification", () => Mailer.sendNotification(), "*/5 * * * *");
            //RecurringJob.AddOrUpdate("SubscriptionNotification", () => new Emailer().sendSubscriptions(), "*/1 * * * *");
        }
    }
}