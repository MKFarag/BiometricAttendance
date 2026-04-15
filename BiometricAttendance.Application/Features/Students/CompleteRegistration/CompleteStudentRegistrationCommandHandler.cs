namespace BiometricAttendance.Application.Features.Students.CompleteRegistration;

public class CompleteStudentRegistrationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CompleteStudentRegistrationCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CompleteStudentRegistrationCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(command.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (!await _unitOfWork.Departments.AnyAsync(x => x.Id == command.Request.DepartmentId, cancellationToken))
            return Result.Failure(DepartmentErrors.NotFound);

        var student = new Student { Level = command.Request.Level, DepartmentId = command.Request.DepartmentId };

        await _unitOfWork.Students.AddAsync(student, cancellationToken);

        await _unitOfWork.Users.DeleteAllRolesAsync(user);

        await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Student.Name);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
