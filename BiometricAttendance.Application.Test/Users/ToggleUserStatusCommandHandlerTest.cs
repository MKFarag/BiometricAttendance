using BiometricAttendance.Application.Features.Users.ToggleStatus;

namespace BiometricAttendance.Application.Test.Users;

public class ToggleUserStatusCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IUserRepository _userRepo = A.Fake<IUserRepository>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly ToggleUserStatusCommandHandler _handler;

    public ToggleUserStatusCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Users).Returns(_userRepo);
        _handler = new ToggleUserStatusCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns((User?)null);

        var command = new ToggleUserStatusCommand(user.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _userRepo.UpdateAsync(A<User>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new ToggleUserStatusCommand(user.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user?.IsDisabled, true);

        A.CallTo(() => _userRepo.UpdateAsync(A<User>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
