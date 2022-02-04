using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPPP_WebApp.Models;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using NLog;
using WebApi.Controllers;

namespace RPPP_WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSection);
            services.AddDbContext<RPPP02Context>(options => options.UseSqlServer(Configuration.GetConnectionString("RPPP02")));
            services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddTransient<PodrucjeController>();

            services.AddCors();

            services.AddControllers()
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                    .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            GlobalDiagnosticsContext.Set("connectionString", Configuration.GetConnectionString("RPPP02"));
            #region Needed for nginx and Kestrel
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                 ForwardedHeaders.XForwardedProto
            });
            string pathBase = Configuration["PathBase"];
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                app.UsePathBase(pathBase);
            }
            #endregion
            app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles()
               .UseRouting()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapDefaultControllerRoute();
                   endpoints.MapControllers();
               });

        }
    }
}
