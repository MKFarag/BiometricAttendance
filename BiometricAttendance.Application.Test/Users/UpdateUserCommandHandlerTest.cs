using BiometricAttendance.Application.Contracts.Users;
using BiometricAttendance.Application.Features.Users.Update;

namespace BiometricAttendance.Application.Test.Users;

public class UpdateUserCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IUserRepository _userRepo = A.Fake<IUserRepository>();
    private readonly IRoleRepository _roleRepo = A.Fake<IRoleRepository>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Users).Returns(_userRepo);
        A.CallTo(() => _unitOfWork.Roles).Returns(_roleRepo);
        _handler = new UpdateUserCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var updateUserRequest = new UpdateUserRequest("First", "Last", "Email@test.com", "UserName", ["Role"]);

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns((User?)null);

        var command = new UpdateUserCommand(userId, updateUserRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenEmailExist_ReturnsDuplicatedEmailError()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var updateUserRequest = new UpdateUserRequest("First", "Last", "ExistEmail@test.com", "UserName", ["Role"]);
        var user = new User { Id = userId, FirstName = "First", LastName = "Last", Email = "Email@test.com", UserName = "UserName" };

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new UpdateUserCommand(userId, updateUserRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicatedEmail, result.Error);
    }

    [Fact]
    public async Task Handle_WhenUserNameExist_ReturnsDuplicatedUserNameError()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var updateUserRequest = new UpdateUserRequest("First", "Last", "Email@test.com", "ExistUserName", ["Role"]);
        var user = new User { Id = userId, FirstName = "First", LastName = "Last", Email = "Email@test.com", UserName = "UserName" };

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new UpdateUserCommand(userId, updateUserRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicatedUserName, result.Error);
    }

    [Fact]
    public async Task Handle_WhenPassInvalidRole_ReturnsInvalidRolesError()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var updateUserRequest = new UpdateUserRequest("First", "Last", "Email@test.com", "UserName", ["InvalidRole"]);
        var user = updateUserRequest.Adapt<User>();

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _roleRepo.GetAllNamesAsync(A<bool>.Ignored, A<bool>.Ignored, A<CancellationToken>.Ignored))
            .Returns(["Role"]);

        var command = new UpdateUserCommand(userId, updateUserRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.InvalidRoles, result.Error);
    }

    [Fact]
    public async Task Handle_ValidDataWithNoChanges_ReturnsSuccessWithNoChangesInRoles()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var roles = new List<string> { "Role" };
        var updateUserRequest = new UpdateUserRequest("First", "Last", "Email@test.com", "UserName", roles);
        var user = updateUserRequest.Adapt<User>();

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _roleRepo.GetAllNamesAsync(A<bool>.Ignored, A<bool>.Ignored, A<CancellationToken>.Ignored))
            .Returns(roles);

        A.CallTo(() => _userRepo.UpdateAsync(A<User>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _userRepo.GetRolesAsync(A<User>.Ignored))
            .Returns(roles);

        A.CallTo(() => _userRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _userRepo.AddToRolesAsync(A<User>.Ignored, A<IEnumerable<string>>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new UpdateUserCommand(userId, updateUserRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _userRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _userRepo.AddToRolesAsync(A<User>.Ignored, A<IEnumerable<string>>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_ValidDataWithChanges_ReturnsSuccessWithChanges()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var roles = new List<string> { "Role_1", "Role_2" };
        var updateUserRequest = new UpdateUserRequest("First", "Last", "NewEmail@test.com", "NewUserName", [roles.First()]);
        var user = new User { Id = userId, FirstName = "First", LastName = "Last", Email = "Email@test.com", UserName = "UserName" };

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _roleRepo.GetAllNamesAsync(A<bool>.Ignored, A<bool>.Ignored, A<CancellationToken>.Ignored))
            .Returns(roles);

        A.CallTo(() => _userRepo.UpdateAsync(A<User>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _userRepo.GetRolesAsync(A<User>.Ignored))
            .Returns(roles);

        A.CallTo(() => _userRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _userRepo.AddToRolesAsync(A<User>.Ignored, A<IEnumerable<string>>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new UpdateUserCommand(userId, updateUserRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _userRepo.EmailExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _userRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _userRepo.AddToRolesAsync(A<User>.Ignored, A<IEnumerable<string>>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
