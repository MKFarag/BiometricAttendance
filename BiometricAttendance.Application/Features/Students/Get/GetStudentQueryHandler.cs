namespace BiometricAttendance.Application.Features.Students.Get;

public class GetStudentQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetStudentQuery, Result<StudentDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<StudentDetailResponse>> Handle(GetStudentQuery request, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students
            .FindAsync
            (
                x => x.Id == request.Id,
                [nameof(Student.Department), nameof(Student.Courses), nameof(StudentCourse.Course)],
                cancellationToken
            );

        if (student is null)
            return Result.Failure<StudentDetailResponse>(StudentErrors.NotFound);

        var user = await _unitOfWork.Users.FindByIdAsync(student.UserId, cancellationToken);

        var response = (student, user).Adapt<StudentDetailResponse>();

        return Result.Success(response);
    }
}
