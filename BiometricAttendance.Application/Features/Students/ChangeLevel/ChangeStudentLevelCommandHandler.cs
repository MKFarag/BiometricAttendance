namespace BiometricAttendance.Application.Features.Students.ChangeLevel;

public class ChangeStudentLevelCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeStudentLevelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeStudentLevelCommand request, CancellationToken cancellationToken = default)
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

        if (request.Level == student.Level)
            return Result.Success();

        student.ChangeLevel(request.Level);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
