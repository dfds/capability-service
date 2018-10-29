using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using DFDS.TeamService.WebApi.DomainEvents;
using DFDS.TeamService.WebApi.Features.AwsConsoleLogin;
using DFDS.TeamService.WebApi.Features.AwsRoles;
using DFDS.TeamService.WebApi.Features.Teams.Application;
using DFDS.TeamService.WebApi.Features.Teams.Domain.Repositories;
using DFDS.TeamService.WebApi.Features.Teams.Infrastructure.Persistence;
using DFDS.TeamService.WebApi.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;

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
                .AddDbContext<TeamServiceDbContext>((serviceProvider, options)  =>
                {
                    var connectionString = serviceProvider.GetRequiredService<IVariables>().TeamDatabaseConnectionString;
                    options.UseNpgsql(connectionString);
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
           var variables = new Variables();
            //if (Environment.GetEnvironmentVariable("VALIDATE_ENVIRONMENT_VARIABLES")?.ToLower() != "false")
            //{
            //    variables.Validate();
            //}

            services.AddSingleton<IVariables>(variables);

            services.AddSingleton<AWSCredentials>(serviceProvider =>
            {
                var vars = serviceProvider.GetService<IVariables>();

                var awsCredentials = new BasicAWSCredentials(
                    vars.AwsCognitoAccessKey,
                    vars.AwsCognitoSecretAccessKey
                );

                return awsCredentials;
            });
            
            services.AddTransient<IAwsConsoleUrlBuilder>(s =>
            {
                var vars = s.GetRequiredService<IVariables>();

                var awsConsoleLinkBuilder = new AwsConsoleUrlBuilder(
                    vars.AwsCognitoIdentityPoolId,
                    vars.AwsCognitoLoginProviderName
                );

                return awsConsoleLinkBuilder;
            });
            services.AddTransient<IExternalDependent>(s => (IExternalDependent)s.GetService<IAwsConsoleUrlBuilder>());
     
                
            services.AddTransient<IAwsIdentityClient, AwsIdentityClient>();

            services.AddSingleton(serviceProvider =>
            {
                var vars = serviceProvider.GetService<IVariables>();

                var awsAccountId = new AwsAccountId(vars.AwsAccountId);


                return awsAccountId;
            });
            services.AddTransient<ITeamIdToAwsConsoleUrl,TeamIdToAwsConsoleUrl>();
            services.AddTransient<TeamNameToRoleNameConverter>();
            
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDomainEvents();

            services.AddScoped(typeof(IUnitOfWork<>), typeof(EventDispatchingUnitOfWork<>));

            services.AddTransient<TeamApplicationService>();
            services.AddTransient<ITeamApplicationService>(serviceProvider => new TeamApplicationServiceTransactionDecorator(
                inner: serviceProvider.GetRequiredService<TeamApplicationService>(),
                unitOfWork: serviceProvider.GetRequiredService<IUnitOfWork<TeamServiceDbContext>>()
            ));
            services.AddTransient<ITeamRepository, DbTeamRepository>();

            services.AddTransient<CognitoUserProvider>(serviceProvider =>
            {
                return new CognitoUserProvider(
                    awsCredentials: serviceProvider.GetRequiredService<AWSCredentials>(),
                    userPoolId: serviceProvider.GetRequiredService<IVariables>().AwsCognitoUserPoolId
                );
            });

            services.AddTransient<DbUserRepository>();
            services.AddTransient<IUserRepository>(serviceProvider =>
            {
                return new CognitoDecorator(
                    inner: serviceProvider.GetRequiredService<DbUserRepository>(),
                    userProvider: serviceProvider.GetRequiredService<CognitoUserProvider>()
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
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