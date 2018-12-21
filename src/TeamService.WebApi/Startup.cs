﻿using DFDS.TeamService.WebApi.Controllers;
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
                .AddEntityFrameworkNpgsql()
                .AddDbContext<TeamServiceDbContext>((serviceProvider, options) =>
                {
                    var connectionString = Configuration["TEAMSERVICE_DATABASE_CONNECTIONSTRING"];
                    options.UseNpgsql(connectionString);
                });


            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<ITeamApplicationService, TeamApplicationService>();
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