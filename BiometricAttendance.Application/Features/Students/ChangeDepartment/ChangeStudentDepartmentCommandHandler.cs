namespace BiometricAttendance.Application.Features.Students.ChangeDepartment;

public class ChangeStudentDepartmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeStudentDepartmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<Result> Handle(ChangeStudentDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
