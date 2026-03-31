using ClinicManagementSystem.Application.Exceptions;

namespace ClinicManagementSystem.Application.Mapping.Helpers
{
    public static class GenderHelper
    {
        public static bool Parse(string gender) =>
            gender.Trim().ToLower() switch
            {
                "male" => true,
                "female" => false,
                _ => throw new BadRequestException($"Invalid gender value '{gender}'. Accepted values are 'Male' or 'Female'.")
            };

        public static string Map(bool gender) => gender ? "Male" : "Female";
    }
}
