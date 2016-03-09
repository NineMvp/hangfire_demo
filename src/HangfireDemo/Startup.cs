using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HangfireDemo.Startup))]

namespace HangfireDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var options = new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(10) //Change default value
            };

            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection", options);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            ConfigureAuth(app);
        }
    }
}
