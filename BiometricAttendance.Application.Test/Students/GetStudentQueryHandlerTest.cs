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

        var department = new Department
        {
            Id = 10,
            Name = "IT"
        };

        var course1 = new Course
        {
            Id = 100,
            Name = "Algorithms",
            Code = "CS301",
            Level = 3
        };

        var course2 = new Course
        {
            Id = 101,
            Name = "Databases",
            Code = "CS302",
            Level = 3
        };

        var student = new Student
        {
            Id = studentId,
            UserId = userId,
            Level = 3,
            DepartmentId = department.Id,
            Department = department,
            Courses =
            [
                new StudentCourse { StudentId = studentId, CourseId = course1.Id, Course = course1 },
                new StudentCourse { StudentId = studentId, CourseId = course2.Id, Course = course2 }
            ]
        };

        var user = new User
        {
            Id = userId,
            FirstName = "Mohamed",
            LastName = "Khaled",
            Email = "Mohamed@example.com"
        };

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
