using DFDS.TeamService.WebApi.Clients;
using DFDS.TeamService.WebApi.Controllers;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using DFDS.TeamService.WebApi.Features.Teams.Infrastructure;
using DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Persistence;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<TeamServiceDbContext>(options =>
                {
                    options.UseNpgsql("User ID=1;Password=1;Host=localhost;Port=1433;Database=teamservice;");
                });
           
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                //.AddJsonOptions(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });

            // Register the Swagger generator, defining 1 or more Swagger documents
            //var apiVersion = "v1";
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc(apiVersion, new Info {Title = "Cognito API", Version = apiVersion});
            //});

            ConfigureDependencyInjectionContainer(services);
        }

        public void ConfigureDependencyInjectionContainer(IServiceCollection services)
        {
            services.AddTransient<IAwsConsoleLinkBuilder>(s =>
            {
                var vars = s.GetRequiredService<IVariables>();

                var awsConsoleLinkBuilder = new AwsConsoleLinkBuilder(
                    vars.AwsCognitoIdentityPoolId,
                    vars.AwsCognitoLoginProviderName
                );

                return awsConsoleLinkBuilder;
            });
            
            
            var variables = new Variables();
            //if (Environment.GetEnvironmentVariable("VALIDATE_ENVIRONMENT_VARIABLES")?.ToLower() != "false")
            //{
            //    variables.Validate();
            //}
            services.AddSingleton<IVariables>(variables);

            services.AddTransient<UserPoolClient>((s) =>
            {
                var vars = s.GetRequiredService<IVariables>();

                return new UserPoolClient(
                    vars.AwsCognitoAccessKey,
                    vars.AwsCognitoSecretAccessKey,
                    vars.AwsCognitoUserPoolId);
            });

            services.AddTransient<CognitoClient>(s =>
            {
                var vars = s.GetService<IVariables>();
                
                return new CognitoClient(
                    vars.AwsCognitoAccessKey,
                    vars.AwsCognitoSecretAccessKey
                );
            });

            services.AddTransient<Features.Teams.Application.TeamService>();
            services.AddTransient<ITeamService>(serviceProvider =>
            {
                var inner = serviceProvider.GetRequiredService<Features.Teams.Application.TeamService>();
                var dbContext = serviceProvider.GetRequiredService<TeamServiceDbContext>();
                return new TeamServiceTransactionDecorator(inner, dbContext);
            });

            services.AddTransient<ITeamRepository, DbTeamRepository>();
            services.AddTransient<IUserRepository, DbUserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            //app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cognito API"); });

            //app.UseExceptionHandler(new ExceptionHandlerOptions
            //{
            //    ExceptionHandler = new JsonExceptionMiddleware().Invoke
            //});

            app.UseMvc();
        }
    }
}