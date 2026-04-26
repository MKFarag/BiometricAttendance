namespace BiometricAttendance.Application.Features.Fingerprints.EndTakingAttendance;

public class EndTakingAttendanceCommandHandler(IUnitOfWork unitOfWork, FingerprintStatus fingerprintStatus) : IRequestHandler<EndTakingAttendanceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly FingerprintStatus _fingerprintStatus = fingerprintStatus;

    public async Task<Result> Handle(EndTakingAttendanceCommand request, CancellationToken cancellationToken = default)
    {
        if (!_fingerprintStatus.IsAttendanceActionWorking)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        if (!await _unitOfWork.Courses.AnyAsync(x => x.Id == request.CourseId, cancellationToken))
            return Result.Failure(CourseErrors.NotFound);

        if (await _unitOfWork.Attendances.AnyAsync(x => x.CourseId == request.CourseId && x.WeekNumber == request.WeekNum, cancellationToken))
            return Result.Failure(AttendanceErrors.WeekAlreadyRecorded);

        _fingerprintStatus.EndAttendanceAction();

        if (_fingerprintStatus.FingerprintIds.Count == 0)
            await Task.Delay(2000, cancellationToken);

        var fingerprintsId = _fingerprintStatus.FingerprintIds;

        _fingerprintStatus.ClearFingerprintIds();

        if (fingerprintsId.Count == 0)
            return Result.Success();

        var studentsId = await _unitOfWork.Students
            .FindAllProjectionAsync
            (
                x => x.FingerprintId.HasValue && fingerprintsId.Contains(x.FingerprintId.Value) && x.Courses.Any(x => x.CourseId == request.CourseId),
                [nameof(Student.Courses)],
                x => x.Id, true,
                cancellationToken
            );

        var attendances = Attendance.CreateRange([.. studentsId], request.CourseId, request.WeekNum);

        await _unitOfWork.Attendances.AddRangeAsync(attendances, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
