using DividendAlert.Formatters;
using DividendAlert.Services.Auth;
using DividendAlert.Services.Mail;
using DividendAlertApi.Middlewares;
using DividendAlertApi.Services.Push;
using DividendAlertData.MongoDb;
using DividendAlertData.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddMvc(options => options.OutputFormatters.Add(new HtmlOutputFormatter()));
            services.AddControllers();

            services.AddCors();

            services.AddScoped<IMailSender, MailSender>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPushService, PushService>();
            services.AddScoped<IDividendsHtmlBuilder, DividendsHtmlBuilder>();
            services.AddScoped<IDividendListBuilder, DividendListBuilder>();

            string mongoConnectionString = _config["MongoConnectionString"];
            string mongoDatabase = _config["MongoDatabase"];

            services.AddSingleton<IUserRepository>(r => new UserRepository(mongoConnectionString, mongoDatabase));
            services.AddSingleton<IStockRepository>(r => new StockRepository(mongoConnectionString, mongoDatabase));
            services.AddSingleton<IDividendRepository>(r => new DividendRepository(mongoConnectionString, mongoDatabase));


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



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ExceptionHandlerMiddleware>();
            }


            // TODO : define prod options
            app.UseCors(option =>
                option
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());



            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }


}
