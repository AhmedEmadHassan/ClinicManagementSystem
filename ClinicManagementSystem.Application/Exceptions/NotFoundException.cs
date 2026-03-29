namespace ClinicManagementSystem.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string resource, int Id) : base($"{resource} with id {Id} is not found")
        {

        }
    }
}
