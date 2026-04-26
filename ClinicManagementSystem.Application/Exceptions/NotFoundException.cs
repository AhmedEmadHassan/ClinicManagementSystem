namespace ClinicManagementSystem.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string resource, int Id, string property = "id") : base($"{resource} with {property} {Id} is not found")
        {

        }
        public NotFoundException(string resource, string Id, string property = "id") : base($"{resource} with {property} {Id} is not found")
        {

        }
    }
}
