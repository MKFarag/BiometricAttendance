using BiometricAttendance.Application.Features.Profile.ChangeUserName;

namespace BiometricAttendance.Application.Test.Profile;

public class ChangeMyUserNameCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IUserRepository _userRepo = A.Fake<IUserRepository>();
    private readonly ChangeMyUserNameCommandHandler _handler;

    public ChangeMyUserNameCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Users).Returns(_userRepo);
        _handler = new ChangeMyUserNameCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns((User?)null);

        var command = new ChangeMyUserNameCommand(Guid.CreateVersion7().ToString(), "new_username");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenNewUserNameSameAsCurrent_ReturnsSameUserNameError()
    {
        // Arrange
        var user = User.Create("user@example.com", "same_username", "John", "Doe");

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        var command = new ChangeMyUserNameCommand(Guid.CreateVersion7().ToString(), "same_username");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.SameUserName, result.Error);
    }

    [Fact]
    public async Task Handle_WhenUserNameAlreadyTaken_ReturnsDuplicatedUserNameError()
    {
        // Arrange
        var user = User.Create("user@example.com", "current_username", "John", "Doe");

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new ChangeMyUserNameCommand(Guid.CreateVersion7().ToString(), "taken_username");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.DuplicatedUserName, result.Error);
    }

    [Fact]
    public async Task Handle_WhenUserNameChangeCooldownNotExpired_ReturnsChangeNotAllowedError()
    {
        // Arrange
        var user = User.Create("user@example.com", "current_username", "John", "Doe");

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _userRepo.IsChangeUserNameAvailable(A<User>.Ignored))
            .Returns(false);

        var command = new ChangeMyUserNameCommand(Guid.CreateVersion7().ToString(), "new_username");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNameChangeNotAllowed, result.Error);
    }

    [Fact]
    public async Task Handle_ValidNewUserName_ReturnsSuccess()
    {
        // Arrange
        var user = User.Create("user@example.com", "current_username", "John", "Doe");

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.UserNameExistsAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _userRepo.IsChangeUserNameAvailable(A<User>.Ignored))
            .Returns(true);

        A.CallTo(() => _userRepo.ChangeUserNameAsync(A<User>.Ignored, A<string>.Ignored, A<int>.Ignored))
            .Returns(Result.Success());

        var command = new ChangeMyUserNameCommand(Guid.CreateVersion7().ToString(), "new_username");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _userRepo.ChangeUserNameAsync(A<User>.Ignored, A<string>.Ignored, User.MinDaysBetweenUserNameChanges))
            .MustHaveHappenedOnceExactly();
    }
}
