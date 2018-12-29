using System;
using DFDS.TeamService.WebApi.Models;
using DFDS.TeamService.WebApi.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DFDS.TeamService.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<TeamServiceDbContext>((serviceProvider, options) =>
                {
                    var connectionString = Configuration["TEAMSERVICE_DATABASE_CONNECTIONSTRING"];
                    options.UseNpgsql(connectionString);
                });

            services.AddHttpClient<IAMRoleServiceFacade>(cfg =>
            {
                var baseUrl = Configuration["IAMROLESERVICE_URL"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            });

            services.AddHttpClient<RoleMapperServiceFacade>(cfg =>
            {
                var baseUrl = Configuration["ROLEMAPPERSERVICE_URL"];
                if (baseUrl != null)
                {
                    cfg.BaseAddress = new Uri(baseUrl);
                }
            });

            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<IRoleService, RoleService>();

            services.AddTransient<TeamApplicationService>();
            services.AddTransient<ITeamApplicationService>(serviceProvider => new TeamTransactionalDecorator(
                inner: serviceProvider.GetRequiredService<TeamApplicationService>(),
                dbContext: serviceProvider.GetRequiredService<TeamServiceDbContext>()
            ));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc();
        }
    }
}