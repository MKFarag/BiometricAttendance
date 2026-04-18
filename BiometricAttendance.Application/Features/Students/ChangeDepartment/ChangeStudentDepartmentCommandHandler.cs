namespace BiometricAttendance.Application.Features.Students.ChangeDepartment;

public class ChangeStudentDepartmentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeStudentDepartmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeStudentDepartmentCommand request, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students
            .FindAsync
            (
                x => x.Id == request.StudentId,
                [nameof(Student.Attendances), nameof(Student.Courses)],
                cancellationToken
            );

        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        if (student.DepartmentId == request.DepartmentId)
            return Result.Success();

        if (!await _unitOfWork.Departments.AnyAsync(x => x.Id == request.DepartmentId, cancellationToken))
            return Result.Failure(DepartmentErrors.NotFound);

        student.ChangeDepartment(request.DepartmentId);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
