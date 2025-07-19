using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CYS.Data;
using CYS.Repos;
using CYS.Helper;
using CYS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
namespace CYS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Uygulamayı tüm IP adreslerinden erişilebilir yap
            builder.WebHost.UseUrls("http://0.0.0.0:5050");

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // MySQL connection
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, 
                    ServerVersion.AutoDetect(connectionString)));

            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    // JSON yapılandırması
                });
                
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CYS API", Version = "v1" });
                
                // JWT Authentication for Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Repository sınıflarını servis olarak ekleyin
            builder.Services.AddScoped<AnimalRepository>();
            builder.Services.AddScoped<WeightMeasurementRepository>();
            builder.Services.AddScoped<BirthWeightMeasurementRepository>();
            builder.Services.AddScoped<WeaningWeightMeasurementRepository>();
            builder.Services.AddScoped<MilkDataRepository>();
            builder.Services.AddScoped<TestWeightRepository>(); // Test API için repository
            
            // JWT Token Service
            builder.Services.AddScoped<JwtTokenService>();

            // Diğer servisleri de burada ekleyebilirsiniz
            // Authentication ayarları - JWT + Cookie
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.AccessDeniedPath = "/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;
                options.Cookie.Name = "CYSCookie";
            });

            builder.Services.AddHostedService<CYSBackgroundService>();
            // CORS politikası ekle
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CYS API V1");
            });
            
            app.UseStaticFiles();
            app.UseRouting();
            
            // CORS middleware'ini etkinleştir
            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // Önceki app.Urls.Add() satırını kaldırın
            app.Run();
        }
    }
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            // MySQL connection
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, 
                    ServerVersion.AutoDetect(connectionString)));

            services.AddControllers()
                .AddJsonOptions(options => {
                    // JSON yapılandırması
                });
                
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CYS API", Version = "v1" });
                
                // JWT Authentication for Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Repository sınıflarını servis olarak ekleyin
            services.AddScoped<AnimalRepository>();
            services.AddScoped<WeightMeasurementRepository>();
            services.AddScoped<BirthWeightMeasurementRepository>();
            services.AddScoped<WeaningWeightMeasurementRepository>();
            services.AddScoped<MilkDataRepository>();
            services.AddScoped<TestWeightRepository>(); // Test API için repository
            
            // JWT Token Service
            services.AddScoped<JwtTokenService>();

            // Diğer servisleri de burada ekleyebilirsiniz
            // Authentication ayarları - JWT + Cookie
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    ValidAudience = Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.AccessDeniedPath = "/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;
                options.Cookie.Name = "CYSCookie";
            });

            services.AddHostedService<CYSBackgroundService>();
            // CORS politikası ekle
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
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
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CYS API V1");
            });
            
            app.UseStaticFiles();
            app.UseRouting();
            
            // CORS middleware'ini etkinleştir
            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // IApplicationBuilder'da Urls özelliği bulunmuyor
            // Konfigürasyon builder.WebHost.UseUrls ile yapılmalı
        }
    }
}