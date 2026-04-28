namespace BiometricAttendance.Application.Features.Instructors.GetAll;

public class GetAllInstructorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllInstructorsQuery, IEnumerable<InstructorResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<InstructorResponse>> Handle(GetAllInstructorsQuery request, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.FindAllByRoleAsync(DefaultRoles.Instructor.Name, cancellationToken);

        var response = users.Adapt<IEnumerable<InstructorResponse>>();

        return response;
    }
}
