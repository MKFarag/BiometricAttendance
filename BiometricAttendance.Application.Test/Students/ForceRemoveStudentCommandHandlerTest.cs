using BiometricAttendance.Application.Features.Students.ForceRemove;

namespace BiometricAttendance.Application.Test.Students;

public class ForceRemoveStudentCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IStudentRepository _studentsRepo = A.Fake<IStudentRepository>();
    private readonly IUserRepository _usersRepo = A.Fake<IUserRepository>();
    private readonly IGenericRepository<Attendance> _attendancesRepo = A.Fake<IGenericRepository<Attendance>>();
    private readonly IGenericRepository<StudentCourse> _studentCoursesRepo = A.Fake<IGenericRepository<StudentCourse>>();
    private readonly IGenericRepository<Fingerprint> _fingerprintsRepo = A.Fake<IGenericRepository<Fingerprint>>();
    private readonly ForceRemoveStudentCommandHandler _handler;

    public ForceRemoveStudentCommandHandlerTest()
    {
        A.CallTo(() => _unitOfWork.Students).Returns(_studentsRepo);
        A.CallTo(() => _unitOfWork.Users).Returns(_usersRepo);
        A.CallTo(() => _unitOfWork.Attendances).Returns(_attendancesRepo);
        A.CallTo(() => _unitOfWork.StudentCourses).Returns(_studentCoursesRepo);
        A.CallTo(() => _unitOfWork.Fingerprints).Returns(_fingerprintsRepo);
        _handler = new ForceRemoveStudentCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenStudentNotFound_ReturnsNotFoundError()
    {
        // Arrange
        A.CallTo(() => _studentsRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns((Student?)null);

        var command = new ForceRemoveStudentCommand(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(StudentErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 2, 1);

        A.CallTo(() => _studentsRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns((User?)null);

        var command = new ForceRemoveStudentCommand(10);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.NotFound, result.Error);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_RemovesAllRelatedDataAndReturnsSuccess()
    {
        // Arrange
        var student = Student.Create(Guid.CreateVersion7().ToString(), 4, 2, 7);
        student.Attendances.Add(new Attendance());
        student.Courses.Add(new StudentCourse());

        var fingerprint = new Fingerprint();
        typeof(Student).GetProperty(nameof(Student.Fingerprint))!.SetValue(student, fingerprint);

        var user = new User { Id = student.UserId };

        A.CallTo(() => _studentsRepo.FindAsync(
                A<Expression<Func<Student, bool>>>.Ignored,
                A<string[]>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(student);

        A.CallTo(() => _studentsRepo.Delete(A<Student>.Ignored));

        A.CallTo(() => _usersRepo.FindByIdAsync(A<string>.Ignored, A<CancellationToken>.Ignored))
            .Returns(user);

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(user))
            .Returns(Task.CompletedTask);

        A.CallTo(() => _usersRepo.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored))
            .Returns(Result.Success());

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .Returns(1);

        var command = new ForceRemoveStudentCommand(10);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(student.Fingerprint);
        Assert.Null(student.FingerprintId);
        Assert.Empty(student.Attendances);
        Assert.Empty(student.Courses);

        A.CallTo(() => _studentsRepo.Delete(student))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.DeleteAllRolesAsync(user))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _usersRepo.AddToRoleAsync(A<User>.Ignored, A<string>.Ignored))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}
