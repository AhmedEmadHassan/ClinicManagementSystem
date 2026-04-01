namespace ClinicManagementSystem.Application.Exceptions
{
    public class CustomValidationException : Exception
    {
        public List<string> Errors { get; }

        public CustomValidationException(List<FluentValidation.Results.ValidationFailure> failures)
            : base("One or more validation errors occurred.")
        {
            Errors = failures.Select(f => f.ErrorMessage).ToList();
        }
    }
}
