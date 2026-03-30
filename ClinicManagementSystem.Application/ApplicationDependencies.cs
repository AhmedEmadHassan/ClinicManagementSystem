using ClinicManagementSystem.Application.Services;
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
        }
    }
}
