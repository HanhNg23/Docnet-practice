using DocnetCorePractice.Data;
using DocnetCorePractice.Extensions;
using DocnetCorePractice.Service;
using DocnetCorePractice.Services;
using Microsoft.OpenApi.Models;
using Serilog;
using static DocnetCorePractice.Extensions.ApiKeyAuthorizationFilter;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DocnetCorePractice
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(WebApplicationBuilder builder, IWebHostEnvironment env)
        {
            _configuration = builder.Configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "DotNetPractice", Version = "v1" });
            });

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = false;
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = false;
            });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            AddDI(services);
            services.AddSingleton<ApiKeyAuthorizationFilter>();
            services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddAuthentication("Bearer").AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("suifbweudfwqudgweufgewufgwefcgweiudgweidgwed"))
                };
            });
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var isUserSwagger = _configuration.GetValue<bool>("UseSwagger", false);
            Console.WriteLine("Is Use Swagger: " + isUserSwagger);
            if (isUserSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DefaultModelsExpandDepth(-1);
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DocNetPractice v1");
                });
            }

            //app.UseMiddleware<ApiKeyAuthenExtension>();
            app.UseAuthorization();
            app.UseSerilogRequestLogging();
            app.MapControllers();
            app.UseRouting();
            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void AddDI(IServiceCollection services)
        {
            services.AddScoped<IInitData, InitData>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICaffeService, CaffeService>();

        }
    }
}
