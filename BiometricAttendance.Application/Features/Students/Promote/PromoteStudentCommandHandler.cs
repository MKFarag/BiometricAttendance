namespace BiometricAttendance.Application.Features.Students.Promote;

public class PromoteStudentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<PromoteStudentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(PromoteStudentCommand request, CancellationToken cancellationToken = default)
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

        if (!student.CanPromote)
            return Result.Failure(StudentErrors.PromoteFailed);

        student.Promote();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
