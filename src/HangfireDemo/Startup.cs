using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HangfireDemo.Startup))]

namespace HangfireDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
            ConfigureAuth(app);
        }
    }
}
