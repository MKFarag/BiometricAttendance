using BiometricAttendance.Application.Features.Students.Get;

namespace BiometricAttendance.Application.Test.Students;

public class GetStudentQueryHandlerTest : IClassFixture<MapsterTestFixture>
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IUserRepository _userRepo = A.Fake<IUserRepository>();
    private readonly GetStudentQueryHandler _handler;

    public GetStudentQueryHandlerTest(MapsterTestFixture _)
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.Users).Returns(_userRepo);
        _handler = new GetStudentQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var studentId = 1;

        A.CallTo(() => _studentRepo.FindAsync(A<Expression<Func<Student, bool>>>.Ignored, A<string[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var query = new GetStudentQuery(studentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error, StudentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenStudentFound_ReturnsMappedStudentDetails()
    {
        // Arrange
        var userId = Guid.CreateVersion7().ToString();
        var user = User.Create("Mohamed@example.com", "MohamedKhaled", "Mohamed", "Khaled"); 
        var student = Student.Create(userId, 3, 2);

        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        var query = new GetStudentQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
