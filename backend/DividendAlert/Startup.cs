using DividendAlert.Formatters;
using DividendAlert.Services.Auth;
using DividendAlert.Services.Mail;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DividendAlert
{
    public class Startup
    {

        private IConfiguration _config { get; }

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }




        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.OutputFormatters.Add(new HtmlOutputFormatter()))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors();

            services.AddScoped<IMailSender, MailSender>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDividendsHtmlBuilder, DividendsHtmlBuilder>();
            services.AddScoped<IDividendListBuilder, DividendListBuilder>();

            services.AddScoped<IUserRepository, UserRepository>();


            AddJwtAuth(services);
        }


        private void AddJwtAuth(IServiceCollection services)
        {
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtSecret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                cfg.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }



        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            // TODO : define prod options
            app.UseCors(option =>
                option
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod());



            app.UseMvc();

            app.UseAuthentication();


        }
    }


}
