using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Application.Services.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ClinicManagementSystem.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services, IConfiguration configuration)
        {
            // Added: JWT Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Added: JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
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
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });
            services.AddAutoMapper(cfg => { }, typeof(ApplicationDependencies).Assembly);
            AddDependencyInjection(services);
            return services;
        }

        public static void AddDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IDoctorSpecializationService, DoctorSpecializationService>();
            services.AddScoped<IAppointmentStateService, AppointmentStateService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IBillingService, BillingService>();
        }
    }
}
