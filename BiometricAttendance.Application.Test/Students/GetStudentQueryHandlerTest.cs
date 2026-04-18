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
        var studentId = 1;
        var userId = Guid.CreateVersion7().ToString();

        var department = Department.Create("IT"); 
        department.Id = 10;

        var course1 = Course.Create("Algorithms", "CS301", 3);
        course1.Id = 100;


        var course2 = Course.Create("Databases", "CS302", 3);
        course2.Id = 101;

        var student = Student.Create(userId, 3, department.Id);
        student.Id = studentId;
        student.EnrollInCourses([course1.Id, course2.Id]);

        var user = User.Create("Mohamed@example.com", "MohamedKhaled", "Mohamed", "Khaled");
        user.Id = userId;

        A.CallTo(() => _studentRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _userRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        var query = new GetStudentQuery(studentId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(studentId, result.Value.Id);
        Assert.Equal(user.FirstName, result.Value.FirstName);
        Assert.Equal(user.LastName, result.Value.LastName);
        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(student.Level, result.Value.Level);
        Assert.Equal(department.Id, result.Value.Department.Id);
        Assert.Equal(department.Name, result.Value.Department.Name);
        Assert.Equal(2, result.Value.Courses.Count);
        Assert.Contains(result.Value.Courses, x => x.Id == course1.Id && x.Name == course1.Name && x.Code == course1.Code);
        Assert.Contains(result.Value.Courses, x => x.Id == course2.Id && x.Name == course2.Name && x.Code == course2.Code);
    }
}
