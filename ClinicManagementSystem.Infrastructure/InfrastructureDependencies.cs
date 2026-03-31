using ClinicManagementSystem.Application.RepositoryInterfaces;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using ClinicManagementSystem.Domain.Entities.Identity;
using ClinicManagementSystem.Infrastructure.Context;
using ClinicManagementSystem.Infrastructure.Repositories;
using ClinicManagementSystem.Infrastructure.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagementSystem.Infrastructure
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SqlServerConnection")
                ));

            AddIdentityDependencies(services, configuration);
            AddDependencyInjection(services, configuration);
            return services;
        }
        public static void AddDependencyInjection(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAppointmentStateRepository, AppointmentStateRepository>();
            services.AddScoped<IBillingRepository, BillingRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IDoctorSpecializationRepository, DoctorSpecializationRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();

        }
        public static void AddIdentityDependencies(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
