using BiometricAttendance.Application.Features.Departments.Remove;

namespace BiometricAttendance.Application.Test.Departments;

public class RemoveDepartmentCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Department> _departmentRepo = A.Fake<IGenericRepository<Department>>();
    private readonly IGenericRepository<DepartmentCourse> _departmentCourseRepo = A.Fake<IGenericRepository<DepartmentCourse>>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly RemoveDepartmentCommandHandler _handler;

    public RemoveDepartmentCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentRepo);
        A.CallTo(() => _unitOfWork.DepartmentCourses).Returns(_departmentCourseRepo);
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        _handler = new RemoveDepartmentCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenDepartmentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new RemoveDepartmentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenDepartmentInUseInOnlyOne_ReturnsInUseError()
    {
        // Arrange
        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _departmentCourseRepo.AnyAsync(A<Expression<Func<DepartmentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new RemoveDepartmentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.InUse, result.Error);

        A.CallTo(() => _departmentRepo.ExecuteDeleteAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenDepartmentInUseInBoth_ReturnsInUseError()
    {
        // Arrange
        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _departmentCourseRepo.AnyAsync(A<Expression<Func<DepartmentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _studentRepo.AnyAsync(A<Expression<Func<Student, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new RemoveDepartmentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.InUse, result.Error);

        A.CallTo(() => _departmentRepo.ExecuteDeleteAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenPassValidData_ReturnsSuccess()
    {
        // Arrange
        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _departmentCourseRepo.AnyAsync(A<Expression<Func<DepartmentCourse, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _departmentRepo.ExecuteDeleteAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new RemoveDepartmentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _departmentRepo.ExecuteDeleteAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
