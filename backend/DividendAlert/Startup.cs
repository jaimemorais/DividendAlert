using DividendAlert.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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


            services.AddCors();


            services.AddSingleton<IMailSender, MailSender>();


            /*
            var jwtSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtDividendAlertSecret")));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateLifetime = false, 
                            ValidateAudience = false, 
                            ValidateIssuer = false,   
                            IssuerSigningKey = jwtSecret,
                            RequireSignedTokens = true
                        };
                    });
                    */
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



            app.UseAuthentication();

            app.UseMvc();
        }
    }


}
