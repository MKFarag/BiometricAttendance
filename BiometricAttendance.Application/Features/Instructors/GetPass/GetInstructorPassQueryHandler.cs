namespace BiometricAttendance.Application.Features.Instructors.GetPass;

public class GetInstructorPassQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetInstructorPassQuery, Result<InstructorPassResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<InstructorPassResponse>> Handle(GetInstructorPassQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.InstructorPasses.FindAsync(x => !x.IsDisabled && x.UsedBy.Count < x.MaxUsedCount, cancellationToken) is not { } currentPass)
            return Result.Failure<InstructorPassResponse>(InstructorErrors.NoPassAvailable);

        var remaining = currentPass.MaxUsedCount - currentPass.UsedBy.Count;

        var response = new InstructorPassResponse(currentPass.Id, currentPass.PassCode, remaining);

        return Result.Success(response);
    }
}
