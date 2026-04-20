namespace BiometricAttendance.Application.Features.Attendances.MarkAttendance;

public class MarkAttendanceCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<MarkAttendanceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Students.AnyAsync(x => x.Id == request.StudentId, cancellationToken))
            return Result.Failure(StudentErrors.NotFound);

        if (!await _unitOfWork.Courses.AnyAsync(x => x.Id == request.CourseId, cancellationToken))
            return Result.Failure(CourseErrors.NotFound);

        if (!await _unitOfWork.StudentCourses.AnyAsync(x => x.StudentId == request.StudentId && x.CourseId == request.CourseId, cancellationToken))
            return Result.Failure(AttendanceErrors.StudentNotEnrolled);

        if (await _unitOfWork.Attendances.AnyAsync(x => x.StudentId == request.StudentId && x.CourseId == request.CourseId && x.WeekNumber == request.WeekNumber, cancellationToken))
            return Result.Failure(AttendanceErrors.AlreadyMarked);

        var attendance = Attendance.Create(request.StudentId, request.CourseId, request.WeekNumber);

        await _unitOfWork.Attendances.AddAsync(attendance, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
