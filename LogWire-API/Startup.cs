using System;
using System.Reflection;
using System.Text;
using LogWire.API.Data.Context;
using LogWire.API.Data.Model;
using LogWire.API.Data.Repository;
using LogWire.API.Services.Hosted;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LogWire.API
{
    public class Startup
    {

        public static string JwtKey = null;
        public static string EncryptionKey = null;


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtKey = Configuration["api.jwt.key"];
            if (JwtKey == null)
            {
                JwtKey = Guid.NewGuid().ToString();
                Configuration["api.jwt.key"] = JwtKey;
            }

            EncryptionKey = Configuration["api.access.key"];
            if (EncryptionKey == null)
            {
                EncryptionKey = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
                Configuration["api.access.key"] = EncryptionKey;
            }

            var key = Encoding.ASCII.GetBytes(JwtKey);

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddDbContext<DataContext>(opt => opt.UseMySql("server=localhost;port=3306;database=lw_api;uid=lwuser;password=lwpassword"));
            services.AddScoped<IDataRepository<RefreshTokenEntry>, RefreshTokenRepository>();

            services.AddSingleton<TokenManagementService>();
            services.AddSingleton<IHostedService, TokenManagementService>(serviceProvider => serviceProvider.GetService<TokenManagementService>());

            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowCredentials();
            corsBuilder.WithOrigins("http://localhost:4100"); // for a specific url. Don't add a forward slash on the end!

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "LogWire Api",
                    Version = "v1"
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
                builder.WithOrigins(new[] {"http://localhost:4100"});
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogWire Api V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
