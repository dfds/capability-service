using Amazon.Runtime;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
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

            services.AddTransient<IAwsIdentityClient>(serviceCollection =>
                {
                    var vars = serviceCollection.GetService<IVariables>();

                    var awsCredentials = new BasicAWSCredentials(
                        vars.AwsCognitoAccessKey,
                        vars.AwsCognitoSecretAccessKey
                    );
                    
                    
                    return new AwsIdentityClient(awsCredentials);
                }
            );
         
            services.AddTransient<TeamApplicationService>();
            services.AddTransient<ITeamService>(serviceProvider => new TeamApplicationServiceTransactionDecorator(
                inner: serviceProvider.GetRequiredService<TeamApplicationService>(),
                dbContext: serviceProvider.GetRequiredService<TeamServiceDbContext>()
            ));
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