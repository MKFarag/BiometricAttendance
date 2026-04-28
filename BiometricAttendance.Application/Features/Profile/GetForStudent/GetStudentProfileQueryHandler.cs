namespace BiometricAttendance.Application.Features.Profile.GetForStudent;

public class GetStudentProfileQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStudentProfileQuery, Result<StudentProfileResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<StudentProfileResponse>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure<StudentProfileResponse>(UserErrors.NotFound);

        var student = await _unitOfWork.Students
            .FindAsync
            (
                x => x.UserId == user.Id,
                [nameof(Student.Department), nameof(Student.Fingerprint)],
                cancellationToken
            );

        var response = (student, user).Adapt<StudentProfileResponse>();

        return Result.Success(response);
    }
}
