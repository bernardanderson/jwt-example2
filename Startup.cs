using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using jwt_example.Options;

namespace jwt_example
{
    public class Startup
    {
        private const string SecretKey = "MIICWwIBAAKBgHRIg4okp4eH8m8h6EEbJ6U7UxQoRx1OM3TY0jyZy5LW9XONbfYEB1uKYI6fwvhioivD3bYLbwG2Eeecj3lthrPkS1H4OEm0YOzb3sX2iOIF9LBtvOr3Da30T9aoxkbPAmYIAPELJ3whb5OUQ/jBqiPtxl69W1M0ZFm8/W6XasnAgMBAAECgYAyU5/6jdqj4cafKoGmi+YgsQhH1RbSLCJBFAYJjFZ6+uXIWRGaRCwbBky77ZEoyhyfA4Uh3nYMxrmcZYr0FxM1jQJEdZ6/NLD5ENA+tf7MMmDhIEwRLzNTrh4mNgDiOib0rzriL68uTnqQQjyilPpektJAeaNcOIctWM4fcTi8xQQJBALRO0rThfPQd9wWH5sS401TWC5YdL7NCosodFDGQikGCL7qCDn2vpRA20gw9RaNvvm2UqnbdOyWqQ62grV33VS8CQQClGST25mf9WfQMGLCg5Nl75bVPqPv8VzBPTS1P1w1K2+XCiRP5KkxOj+CoixUbJsNmhKPXGrbTr8wPl0awRPuJAkAbOevHNGVR11R9mU/XVLFUlh2ZxT52qxE5w7pQ4ap+ydG7L/hQMj2SfTSqdHYXf8AZe+FhoXZU8ajWhvSmw7oxAkEAje89/h07DW4GB1g6kTftWTy0UVW/vMLgP2zkJZ2GSfMeZc4fI7iffXLn+z4G2R7MIhiqGkVXJEogLs8o8GV9sQJASRKHed2RnSniuTXw+KPLr3SoyPiMquFQkwqYG882FmlhUI5gnJ6nm7u8ji1fKZq7XM58roFqFnf5i21O0ZPqjA==";
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Add framework services.
            services.AddOptions();

            // Add framework services.
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
