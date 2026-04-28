using BiometricAttendance.Application.Contracts.Departments;
using BiometricAttendance.Application.Features.Departments.Add;

namespace BiometricAttendance.Application.Test.Departments;

public class AddDepartmentCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<Department> _departmentRepo = A.Fake<IGenericRepository<Department>>();
    private readonly ICacheService _cacheService = A.Fake<ICacheService>();
    private readonly AddDepartmentCommandHandler _handler;

    public AddDepartmentCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentRepo);
        _handler = new AddDepartmentCommandHandler(_unitOfWork, _cacheService);
    }

    [Fact]
    public async Task Handle_WhenNameIsExist_ReturnsNameAlreadyExistsError()
    {
        // Arrange
        var request = new DepartmentRequest("ExistName");

        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        var command = new AddDepartmentCommand(request.Name);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, DepartmentErrors.NameAlreadyExists);
    }

    [Fact]
    public async Task Handle_PassValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new DepartmentRequest("Name");
        var department = Department.Create(request.Name);

        A.CallTo(() => _departmentRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        A.CallTo(() => _departmentRepo.AddAsync(A<Department>.Ignored, A<CancellationToken>.Ignored))
            .Returns(department);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(Task.CompletedTask);

        var command = new AddDepartmentCommand(request.Name);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, department.Name);

        A.CallTo(() => _departmentRepo.AddAsync(A<Department>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _cacheService.RemoveByTagAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
