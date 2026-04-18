namespace BiometricAttendance.Application.Features.Students.ChangeLevel;

public class ChangeStudentLevelCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeStudentLevelCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeStudentLevelCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Students.GetAsync([request.StudentId], cancellationToken) is not { } student)
            return Result.Failure(StudentErrors.NotFound);

        if (request.Level == student.Level)
            return Result.Success();

        student.ChangeLevel(request.Level);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
