using ClinicManagementSystem.Application.DTOs.ResponseDTOs;
using ClinicManagementSystem.Application.RepositoryInterfaces.UnitOfWorkInterface;
using MediatR;

namespace ClinicManagementSystem.Application.Features.Doctors.Queries.GetAll
{
    // Queries/GetAll
    public record GetAllDoctorsQuery : IRequest<List<ResponseDoctorDTO>>;

    public class GetAllDoctorsHandler : IRequestHandler<GetAllDoctorsQuery, List<ResponseDoctorDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDoctorsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ResponseDoctorDTO>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Doctors.GetAllAsync(d => new ResponseDoctorDTO
            {
                Id = d.Id,
                Name = d.Name,
                Phone = d.Phone,
                Gender = d.Gender ? "Male" : "Female",
                Email = d.Email,
                Address = d.Address,
                DateOfBirth = d.DateOfBirth,
                Summary = d.Summary,
                DoctorSpecializationId = d.DoctorSpecializationId,
                DoctorSpecializationName = d.DoctorSpecialization.Name
            });
        }
    }
}
