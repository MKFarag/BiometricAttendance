using BiometricAttendance.Application.Contracts.Students;
using BiometricAttendance.Application.Features.Students.CompleteRegistration;

namespace BiometricAttendance.Application.Test.Students;

public class CompleteStudentRegistrationCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IUserRepository _usersRepo = A.Fake<IUserRepository>();
    private readonly IGenericRepository<Department> _departmentsRepo = A.Fake<IGenericRepository<Department>>();
    private readonly IGenericRepository<Student> _studentsRepo = A.Fake<IGenericRepository<Student>>();
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
        var command = new CompleteStudentRegistrationCommand("user-id", new CompleteStudentRegistrationRequest(2, 1));

        A.CallTo(() => _usersRepo.FindByIdAsync(command.UserId, A<CancellationToken>.Ignored))
            .Returns((AppUser?)null);

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
        var user = new AppUser();
        var command = new CompleteStudentRegistrationCommand("user-id", new CompleteStudentRegistrationRequest(2, 999));

        A.CallTo(() => _usersRepo.FindByIdAsync(command.UserId, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

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
        var user = new AppUser();
        var command = new CompleteStudentRegistrationCommand("user-id", new CompleteStudentRegistrationRequest(4, 2));

        A.CallTo(() => _usersRepo.FindByIdAsync(command.UserId, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentsRepo.AddAsync(A<Student>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(user))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _usersRepo.AddToRoleAsync(user, DefaultRoles.Student.Name))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _studentsRepo.AddAsync(
                A<Student>.That.Matches(x => x.Level == command.Request.Level && x.DepartmentId == command.Request.DepartmentId),
                A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(user))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.AddToRoleAsync(user, DefaultRoles.Student.Name))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
