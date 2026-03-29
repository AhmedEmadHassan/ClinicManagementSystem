using ClinicManagementSystem.Application.RepositoryInterfaces.Generic;
using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Application.RepositoryInterfaces
{
    public interface IPatientRepository : IGenericRepositoryAsync<Patient>
    {

    }
}
