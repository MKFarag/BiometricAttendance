namespace BiometricAttendance.Application.Features.Incstructors.GetRole;

public class GetInstructorRoleCommandHandler(IUnitOfWork unitOfWork, IInstructorPassService instructorPassService) : IRequestHandler<GetInstructorRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IInstructorPassService _instructorPassService = instructorPassService;

    public async Task<Result> Handle(GetInstructorRoleCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            if (!await _instructorPassService.TryUseAsync(request.UserId, request.Pass, cancellationToken))
                return Result.Failure(InstructorErrors.InvalidPassword);

            await _unitOfWork.Users.DeleteAllRolesAsync(user);

            await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Instructor.Name);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure(InstructorErrors.SetRoleFailed);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return Result.Success();
    }
}
