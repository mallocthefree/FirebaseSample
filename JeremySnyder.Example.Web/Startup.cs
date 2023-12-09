/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using JeremySnyder.Common;
using JeremySnyder.Security;
using Swashbuckle.AspNetCore.Filters;

namespace JeremySnyder.Example.Web
{
    [ExcludeFromCodeCoverage]
    internal class Startup
    {
        #region Properties
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        #endregion

        private const string CorsAllowSpecificOrigins = "AllowSpecificOrigin";

        /// <summary>
        /// Called by the runtime
        /// </summary>
        /// <param name="configuration">Application configuration (built-in)</param>
        /// <param name="env">System environment (built-in)</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        /// <summary>
        /// Allows us to bypass security during debug-time only. Flip the configuration value
        /// in appsettings.config
        /// Note: This setting will not impact systems running without a debugger.
        /// </summary>
        private static bool SecurityEnabled =>
            (    
                Debugger.IsAttached &&
                SecurityFactory.Configuration
                    .DisableSecurity
                    .Contains("true", StringComparison.InvariantCultureIgnoreCase)
            ) == false;

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Built-in parameter sent by the runtime</param>
        public void ConfigureServices(IServiceCollection services)
        {
            Logging.Info("Startup.ConfigureServices initiated");
            Logging.Info($"Version [{Assembly.GetExecutingAssembly().GetName().Version}]");

            if (SecurityEnabled)
            {
                Logging.Debug($"Using Tokens via [{SecurityFactory.Configuration.Issuer}]");
                ConfigureAuthentication(services);
            }
            else
            {
                Logging.Warning("Starting without security enabled");
            }

            ConfigureCors(services);
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllers()
                .AddNewtonsoftJson();
            
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Adds a bearer token to the header where needed. Be sure to add the bearer tag.<p/>Example:<p/>bearer eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc2MjNlMTBhMDQ1MTQwZjFjZmQ0YmUwNDY2Y2Y4MDM1MmI1OWY4MWUiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vb3NvdmVnYS1jbGVhciIsImF1ZCI6Im9zb3ZlZ2EtY2xlYXIiLCJhdXRoX3RpbWUiOjE1OTQwMzc2MjgsInVzZXJfaWQiOiJzaFNhVmVXWTdIT2VvMEZYUnhRTWZnSnBGOEQzIiwic3ViIjoic2hTYVZlV1k3SE9lbzBGWFJ4UU1mZ0pwRjhEMyIsImlhdCI6MTU5NDAzNzYyOCwiZXhwIjoxNTk0MDQxMjI4LCJlbWFpbCI6InRlc3R1c2VyQG9zb3ZlZ2EuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbInRlc3R1c2VyQG9zb3ZlZ2EuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGFzc3dvcmQifX0.e3AmYtGTNAsVeHVm8N6m7sFUfaqivMkgrGtMD0IySu8hs2W_xWng64of4QtXdTeS6ozcSHwPAsQqEs01uVgRaYjYn0fFmfUnWXuWC2jKc7fq6FNROM5_jnFmKu3JSLgWRxmwCA6oVnavD7EJOaz3pHPCoBOeV_T6pnLeolBZRwNByREfafLgJ27abHVBj6sEM7iGdWumVI3szcBc4IxWcwtfkLehPIwf_D1fhdIJiPPh4gKPPT7DE-zeWkzmSLwTwqTs2QrgpjpkWndHoO2QwxgcQTsewCxSrkIfGKoRAmCXIoB3F8rtwrAmv1CKtKgD2Ld_cC7JgTU0MOAgmzHHlQ",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer"
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            var assembly = Assembly.GetExecutingAssembly();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"Jeremy Snyder Consulting Security Sample API [{assembly.GetName().Version}]",
                    Description = "Test and validate security API endpoints",
                    TermsOfService = null,
                    Contact = new OpenApiContact
                    {
                        Name = "Jeremy Snyder Consulting",
                        Email = "JeremySnyder.Consulting@gmail.com",
                        Url = new Uri(GetUrl()),
                    },
                    License = null
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                /*
                xmlPath = Path.Combine(AppContext.BaseDirectory, "JeremySnyder.Data.xml");
                c.IncludeXmlComments(xmlPath);
                
                xmlPath = Path.Combine(AppContext.BaseDirectory, "JeremySnyder.Security.Data.xml");
                c.IncludeXmlComments(xmlPath);
                
                xmlPath = Path.Combine(AppContext.BaseDirectory, "JeremySnyder.Shared.Data.xml");
                c.IncludeXmlComments(xmlPath);
                */
            });
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Called automatically during the startup processes in a higher level of the web core.
        /// Configurations to the web API are managed here.
        /// </summary>
        /// <param name="app">Information about this application and it's base configuration</param>
        /// <param name="env">Information about the environment the API is run on</param>
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            Logging.Info("Startup.Configure initiated");
            Logging.Info($"IsDevelopment: [{env.IsDevelopment().ToString()}]");

            if (env.IsDevelopment() || Debugger.IsAttached)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseStaticFiles();
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = false;
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api/swagger";
                c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Jeremy Snyder Consulting Security API");
            });

            app.UseRouting();
            app.UseCors(CorsAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                         .RequireCors(CorsAllowSpecificOrigins);
            });
        }

        private string GetUrl()
        {
            return "http://localhost:8080";
            
            /*
             // Using this, you can create a debugger vs production vs staging automated target
            return Debugger.IsAttached ?
                "http://localhost:8080" :
                $"https://{(Environment.IsDevelopment() ? "staging" : "production")}.jeremysnyder.consulting";
            */
        }
        
        /// <summary>
        /// Builds our CORS ( https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS ),
        /// which is needed for testing on the same system or for running a system
        /// that targets this API from the same computer.
        /// </summary>
        /// <param name="services">Built-in parameter sent from <seealso cref="ConfigureServices"/></param>
        private static void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy(name: CorsAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins(
                                "https://*.jeremysnyder.consulting",
                                "https://localhost:8080",
                                "http://localhost:8080",
                                "https://localhost",
                                "http://localhost")
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }));
        }

        /// <summary>
        /// Configures our authentication integration, which is currently "Firebase"
        /// </summary>
        /// <param name="services">Built-in parameter sent from <seealso cref="ConfigureServices"/></param>
        private static void ConfigureAuthentication(IServiceCollection services)
        {
            // This line establishes the configuration to be used for Firebase validation
            SecurityFactory.Authentication.Configure();
            
            Logging.Info($"Using Tokens via [{SecurityFactory.Configuration.Issuer}]");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        options.Authority = SecurityFactory.Configuration.Authority;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = SecurityFactory.Configuration.Issuer,
                            ValidateAudience = true,
                            ValidAudience = SecurityFactory.Configuration.ProjectId,
                            ValidateLifetime = true
                        };
                    }
                );
        }
    }
}
