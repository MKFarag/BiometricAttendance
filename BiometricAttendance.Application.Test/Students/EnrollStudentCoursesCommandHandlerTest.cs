using BiometricAttendance.Application.Features.Students.EnrollCourses;

namespace BiometricAttendance.Application.Test.Students;

public class EnrollStudentCoursesCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentRepo = A.Fake<IStudentRepository>();
    private readonly IGenericRepository<Course> _coursesRepo = A.Fake<IGenericRepository<Course>>();
    private readonly IGenericRepository<StudentCourse> _studentCoursesRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly EnrollStudentCoursesCommandHandler _handler;

    public EnrollStudentCoursesCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentRepo);
        A.CallTo(() => _unitOfWork.Courses).Returns(_coursesRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCoursesRepo);
        _handler = new EnrollStudentCoursesCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var command = new EnrollStudentCoursesCommand(Guid.CreateVersion7().ToString(), [1, 2]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenRequestContainsInvalidCourses_ReturnsInvalidCoursesError()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);

        A.CallTo(() => _studentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _coursesRepo.FindAllProjectionAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<Expression<Func<Course, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([1, 2]);

        var command = new EnrollStudentCoursesCommand(student.UserId, [1, 2, 3]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.InvalidCourses, result.Error);

        A.CallTo(() => _studentCoursesRepo.AddRangeAsync(A<IEnumerable<StudentCourse>>.Ignored, A<CancellationToken>.Ignored))
            .MustNotHaveHappened();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ReturnsSuccess()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);
        IEnumerable<StudentCourse> studentCourses = 
            [StudentCourse.Create(1, student.Id), StudentCourse.Create(2, student.Id)];

        A.CallTo(() => _studentRepo.GetAsync(A<object[]>.Ignored, A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _coursesRepo.FindAllProjectionAsync(
                A<Expression<Func<Course, bool>>>.Ignored,
                A<Expression<Func<Course, int>>>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns([1, 2, 3]);

        A.CallTo(() => _studentCoursesRepo.AddRangeAsync(A<IEnumerable<StudentCourse>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(studentCourses);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new EnrollStudentCoursesCommand(student.UserId, [1, 2]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        A.CallTo(() => _studentCoursesRepo.AddRangeAsync(A<IEnumerable<StudentCourse>>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
