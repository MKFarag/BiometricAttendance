using BiometricAttendance.Application.Features.Students.Promote;

namespace BiometricAttendance.Application.Test.Students;

public class PromoteStudentCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentsRepo = A.Fake<IStudentRepository>();
    private readonly PromoteStudentCommandHandler _handler;

    public PromoteStudentCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentsRepo);
        _handler = new PromoteStudentCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var command = new PromoteStudentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenStudentCanNotPromote_ReturnsPromoteFailedError()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 5, 1);

        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        var command = new PromoteStudentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.PromoteFailed, result.Error);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenStudentCanPromote_ReturnsSuccess()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 3, 1);

        A.CallTo(() => _studentsRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new PromoteStudentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, student.Level);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
