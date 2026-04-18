using BiometricAttendance.Application.Features.Students.ChangeDepartment;
using BiometricAttendance.Application.Features.Students.ChangeLevel;

namespace BiometricAttendance.Application.Test.Students;

public class ChangeStudentLevelCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentsRepo = A.Fake<IStudentRepository>();
    private readonly ChangeStudentLevelCommandHandler _handler;

    public ChangeStudentLevelCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentsRepo);
        _handler = new ChangeStudentLevelCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentsRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var command = new ChangeStudentLevelCommand(1, 2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenPassTheSameLevel_ReturnsSuccessWithNoChanges()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 3, null);

        A.CallTo(() => _studentsRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        var command = new ChangeStudentLevelCommand(1, student.Level);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ReturnsSuccess()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 3);

        A.CallTo(() => _studentsRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new ChangeStudentLevelCommand(student.Id, 4);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Level, student.Level);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
