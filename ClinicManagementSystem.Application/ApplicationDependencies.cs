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
            AddDependencyInjection(services);
            return services;
        }

        public static void AddDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IDoctorSpecializationService, DoctorSpecializationService>();
            services.AddScoped<IAppointmentStateService, AppointmentStateService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
        }
    }
}
