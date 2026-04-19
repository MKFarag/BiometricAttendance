namespace BiometricAttendance.Application.Features.Students.ForceRemove;

public class ForceRemoveStudentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ForceRemoveStudentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ForceRemoveStudentCommand request, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students
            .FindAsync
            (
                x => x.Id == request.StudentId,
                [nameof(Student.Attendances), nameof(Student.Courses), nameof(Student.Fingerprint)],
                cancellationToken
            );

        if (student is null)
            return Result.Failure(StudentErrors.NotFound);

        if (student.Attendances.Count != 0 || student.Courses.Count != 0)
            student.ResetData();

        if (student.Fingerprint is not null)
        {
            student.RemoveFingerprint();
            _unitOfWork.Fingerprints.Delete(student.Fingerprint);
        }

        _unitOfWork.Students.Delete(student);

        if (await _unitOfWork.Users.FindByIdAsync(student.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        await _unitOfWork.Users.DeleteAllRolesAsync(user);

        var result = await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Pending.Name);

        if (result.IsSuccess)
            await _unitOfWork.CompleteAsync(CancellationToken.None);

        return result;
    }
}
