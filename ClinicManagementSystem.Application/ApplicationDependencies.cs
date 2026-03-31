using ClinicManagementSystem.Application.Services.Abstraction;
using ClinicManagementSystem.Application.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagementSystem.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                // Optional: add custom config here if needed
            }, typeof(ApplicationDependencies).Assembly);
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
