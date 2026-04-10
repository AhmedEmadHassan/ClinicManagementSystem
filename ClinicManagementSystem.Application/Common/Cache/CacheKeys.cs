namespace ClinicManagementSystem.Application.Common.Cache
{
    public static class CacheKeys
    {
        public const string DoctorSpecialization = "DoctorSpecialization";
        public const string AppointmentState = "AppointmentState";
        public const string Doctor = "Doctor";
        public const string Patient = "Patient";
        public const string Appointment = "Appointment";
        public const string Session = "Session";
        public const string Billing = "Billing";

        public static string GetAll(string entity) => $"{entity}:GetAll";
        public static string GetById(string entity, int id) => $"{entity}:GetById:{id}";
    }
}
