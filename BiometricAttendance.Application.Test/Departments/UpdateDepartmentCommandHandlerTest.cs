using BiometricAttendance.Application.Features.Departments.Update;

namespace BiometricAttendance.Application.Test.Departments;

public class UpdateDepartmentCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Department> _departmentRepo = A.Fake<IGenericRepository<Department>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly UpdateDepartmentCommandHandler _handler;

    public UpdateDepartmentCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentRepo);
        _handler = new UpdateDepartmentCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenDepartmentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _departmentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Department?)null);

        var command = new UpdateDepartmentCommand(1, "IT");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenNameEqualsCurrentName_ReturnsSuccessWithoutChanges()
    {
        // Arrange
        var department = Department.Create("IT");

        A.CallTo(() => _departmentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(department);

        var command = new UpdateDepartmentCommand(1, "it");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExists_ReturnsNameAlreadyExistsError()
    {
        // Arrange
        var department = Department.Create("IT");

        A.CallTo(() => _departmentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(department);

        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new UpdateDepartmentCommand(1, "CS");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DepartmentErrors.NameAlreadyExists, result.Error);
    }

    [Fact]
    public async Task Handle_WhenPassValidData_ReturnsSuccess()
    {
        // Arrange
        var department = Department.Create("IT");

        A.CallTo(() => _departmentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(department);

        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new UpdateDepartmentCommand(1, "CS");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
