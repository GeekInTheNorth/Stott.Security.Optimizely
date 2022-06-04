namespace OptimizelyTwelveTest
{
    using EPiServer.Cms.UI.AspNetIdentity;
    using EPiServer.Scheduler;
    using EPiServer.Web.Routing;

    using MediatR;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using OptimizelyTwelveTest.Features.Common;

    using ServiceExtensions;

    using Stott.Optimizely.RobotsHandler.Configuration;
    using Stott.Security.Optimizely.Features.Configuration;

    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;

        private const string CorsPolicyName = "development-uris";

        public Startup(IWebHostEnvironment webHostingEnvironment)
        {
            _webHostingEnvironment = webHostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_webHostingEnvironment.IsDevelopment())
            {
                services.Configure<SchedulerOptions>(o =>
                {
                    o.Enabled = false;
                });
            }

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder.WithOrigins("http://localhost:3000").WithMethods("GET", "POST").AllowAnyHeader();
                });
            });

            services.AddRazorPages();
            services.AddCmsAspNetIdentity<ApplicationUser>();

            // Various serialization formats.
            //// services.AddMvc().AddNewtonsoftJson();
            services.AddMvc().AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.PropertyNamingPolicy = new UpperCaseNamingPolicy();
            });

            services.AddCms();
            services.AddFind();
            services.AddMediatR(typeof(GroupNames).Assembly);
            services.AddCustomDependencies();
            services.AddRobotsHandler();
            services.AddCspManager(cspSetupOptions =>
            {
                cspSetupOptions.AllowedRoles.Clear();
                cspSetupOptions.AllowedRoles.Add("CspAdmin");
                cspSetupOptions.UseWhitelist = true;
                cspSetupOptions.WhitelistUrl = "https://raw.githubusercontent.com/GeekInTheNorth/Stott.Optimizely.Csp/main/Example%20Documents/whitelistentries.json";
                cspSetupOptions.ConnectionStringName = "EPiServerDB";
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/util/Login";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCspManager();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapRazorPages();
            });
        }
    }
}
