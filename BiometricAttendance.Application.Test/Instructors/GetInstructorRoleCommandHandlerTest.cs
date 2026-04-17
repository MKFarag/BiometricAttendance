using BiometricAttendance.Application.Features.Incstructors.GetRole;

namespace BiometricAttendance.Application.Test.Instructors;

public class GetInstructorRoleCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IUserRepository _usersRepo = A.Fake<IUserRepository>();
    private readonly IInstructorPassService _instructorPassService = A.Fake<IInstructorPassService>();
    private readonly GetInstructorRoleCommandHandler _handler;

    public GetInstructorRoleCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Users).Returns(_usersRepo);
        _handler = new GetInstructorRoleCommandHandler(_unitOfWork, _instructorPassService);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var pass = "pass";

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns((User?)null);

        var command = new GetInstructorRoleCommand(user.Id, pass);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _instructorPassService.TryUseAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenPassIsInvalid_ReturnsInvalidPasswordError()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var pass = "pass";

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _instructorPassService.TryUseAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new GetInstructorRoleCommand(user.Id, pass);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InstructorErrors.InvalidPassword, result.Error);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _usersRepo.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CommitTransactionAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.RollbackTransactionAsync())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenPassIsValid_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var pass = "pass";

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _instructorPassService.TryUseAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(user))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _usersRepo.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _unitOfWork.CommitTransactionAsync(A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new GetInstructorRoleCommand(user.Id, pass);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(user))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.AddToRoleAsync(user, DefaultRoles.Instructor.Name))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CommitTransactionAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.RollbackTransactionAsync())
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenOperationCanceledExceptionOccurs_ReturnsSetRoleFailedError()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var pass = "pass";

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _instructorPassService.TryUseAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Throws(new OperationCanceledException());

        A.CallTo(() => _unitOfWork.RollbackTransactionAsync())
            .Returns(Task.CompletedTask);

        var command = new GetInstructorRoleCommand(user.Id, pass);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(InstructorErrors.SetRoleFailed, result.Error);

        A.CallTo(() => _unitOfWork.CommitTransactionAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.RollbackTransactionAsync())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_RollsBackTransactionAndThrows()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var pass = "pass";

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _unitOfWork.BeginTransactionAsync(A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _instructorPassService.TryUseAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Throws(new InvalidOperationException("boom"));

        A.CallTo(() => _unitOfWork.RollbackTransactionAsync())
            .Returns(Task.CompletedTask);

        var command = new GetInstructorRoleCommand(user.Id, pass);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));

        A.CallTo(() => _unitOfWork.CommitTransactionAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.RollbackTransactionAsync())
            .MustHaveHappenedOnceExactly();
    }
}
