using BiometricAttendance.Application.Features.Students.ChangeDepartment;

namespace BiometricAttendance.Application.Test.Students;

public class ChangeStudentDepartmentCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentsRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<Department> _departmentsRepo = A.Fake<IGenericRepository<Department>>();
    private readonly ChangeStudentDepartmentCommandHandler _handler;

    public ChangeStudentDepartmentCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentsRepo);
        A.CallTo(() => _unitOfWork.Departments).Returns(_departmentsRepo);
        _handler = new ChangeStudentDepartmentCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var command = new ChangeStudentDepartmentCommand(1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenDepartmentIsSameAsCurrent_ReturnsSuccessWithoutSaving()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 3);

        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        var command = new ChangeStudentDepartmentCommand(1, student.DepartmentId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenDepartmentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 3);

        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(false);

        var command = new ChangeStudentDepartmentCommand(1, 7);

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
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 3);

        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _departmentsRepo.AnyAsync(A<Expression<Func<Department, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(true);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new ChangeStudentDepartmentCommand(1, 7);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.DepartmentId, student.DepartmentId);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
