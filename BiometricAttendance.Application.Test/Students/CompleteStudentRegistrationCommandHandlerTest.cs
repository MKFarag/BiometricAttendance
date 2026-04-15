using BiometricAttendance.Application.Contracts.Students;
using BiometricAttendance.Application.Features.Students.CompleteRegistration;

namespace BiometricAttendance.Application.Test.Students;

public class CompleteStudentRegistrationCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IUserRepository _usersRepo = A.Fake<IUserRepository>();
    private readonly IGenericRepository<Department> _departmentsRepo = A.Fake<IGenericRepository<Department>>();
    private readonly IStudentRepository _studentsRepo = A.Fake<IStudentRepository>();
    private readonly CompleteStudentRegistrationCommandHandler _handler;

    public CompleteStudentRegistrationCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Users).Returns(_usersRepo);
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentsRepo);
        A.CallTo(() => _unitOfWork.Students).Returns(_studentsRepo);
        _handler = new CompleteStudentRegistrationCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var request = new CompleteStudentRegistrationRequest(2, 1);

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns((User?)null);

        var command = new CompleteStudentRegistrationCommand(userId, request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenDepartmentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var request = new CompleteStudentRegistrationRequest(2, 1);

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new CompleteStudentRegistrationCommand(user.Id, request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ReturnsSuccess()
    {
        // Arrange
        var user = new User { Id = Guid.CreateVersion7().ToString() };
        var request = new CompleteStudentRegistrationRequest(2, 1);

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentsRepo.AddAsync(A<Student>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new Student());

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _usersRepo.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new CompleteStudentRegistrationCommand(user.Id, request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(A<User>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
