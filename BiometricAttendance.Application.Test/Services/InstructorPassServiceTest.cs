using BiometricAttendance.Application.Services;

namespace BiometricAttendance.Application.Test.Services;

public class InstructorPassServiceTest
{
    private readonly IUnitOfWork _unitOfWork = A.Fake<IUnitOfWork>();
    private readonly IGenericRepository<InstructorPass> _instructorPassRepo = A.Fake<IGenericRepository<InstructorPass>>();
    private readonly InstructorPassService _service;

    public InstructorPassServiceTest()
    {
        A.CallTo(() => _unitOfWork.InstructorPasses).Returns(_instructorPassRepo);
        _service = new InstructorPassService(_unitOfWork);
    }

    [Fact]
    public async Task TryUseAsync_WhenNoValidPass_ThrowsException()
    {
        // Arrange
        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns<InstructorPass?>(null);

        async Task<bool> act() => await _service.TryUseAsync("user-1", "PASS", CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>((Func<Task<bool>>)act);
    }

    [Fact]
    public async Task TryUseAsync_WhenPassCodeDoesNotMatch_ReturnsFalseAndDoesNotPersist()
    {
        // Arrange
        var currentPass = new InstructorPass(10);

        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(currentPass);

        // Act
        var result = await _service.TryUseAsync("user-1", "WRONG-PASS", CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Empty(currentPass.UsedBy);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task TryUseAsync_WhenPassCodeMatchesAndNotExhausted_ReturnsTrueAndSaves()
    {
        // Arrange
        var currentPass = new InstructorPass(2);
        var submittedCode = currentPass.PassCode;

        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(currentPass);

        // Act
        var result = await _service.TryUseAsync("user-1", submittedCode, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Single(currentPass.UsedBy);
        Assert.Equal("user-1", currentPass.UsedBy[0]);

        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task TryUseAsync_WhenPassBecomesExhausted_AddsNewPassAndSaves()
    {
        // Arrange
        var currentPass = new InstructorPass(1);
        var submittedCode = currentPass.PassCode;

        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(currentPass);

        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsLazily((InstructorPass entity, CancellationToken _) => Task.FromResult(entity));

        // Act
        var result = await _service.TryUseAsync("user-1", submittedCode, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.True(currentPass.IsExhausted);

        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CreateNewPassAsync_WhenNoValidPass_ThrowsException()
    {
        // Arrange
        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns<InstructorPass?>(null);

        var act = async () => await _service.CreateNewPassAsync(CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(act);
    }

    [Fact]
    public async Task CreateNewPassAsync_WhenCurrentPassStillFresh_DoesNothing()
    {
        // Arrange
        var currentPass = new InstructorPass(10);

        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(currentPass);

        // Act
        await _service.CreateNewPassAsync(CancellationToken.None);

        // Assert
        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public async Task CreateNewPassAsync_WhenPassIsOldAndUnused_RenewsAndSaves()
    {
        // Arrange
        var currentPass = new InstructorPass(10);
        var oldCode = currentPass.PassCode;
        SetGeneratedAt(currentPass, DateTime.UtcNow.AddHours(-13));

        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(currentPass);

        // Act
        await _service.CreateNewPassAsync(CancellationToken.None);

        // Assert
        Assert.NotEqual(oldCode, currentPass.PassCode);
        Assert.False(currentPass.IsDisabled);
        Assert.Empty(currentPass.UsedBy);

        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CreateNewPassAsync_WhenPassIsOldAndUsed_DisablesCurrentAndCreatesNewPass()
    {
        // Arrange
        var currentPass = new InstructorPass(10);
        currentPass.Use("user-1");
        SetGeneratedAt(currentPass, DateTime.UtcNow.AddHours(-13));

        A.CallTo(() => _instructorPassRepo.TrackedFindAsync(A<Expression<Func<InstructorPass, bool>>>.Ignored, A<CancellationToken>.Ignored))
            .Returns(currentPass);

        A.CallTo(() => _instructorPassRepo.AddAsync(A<InstructorPass>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsLazily((InstructorPass entity, CancellationToken _) => Task.FromResult(entity));

        // Act
        await _service.CreateNewPassAsync(CancellationToken.None);

        // Assert
        Assert.True(currentPass.IsDisabled);

        A.CallTo(() => _instructorPassRepo.AddAsync(
            A<InstructorPass>.That.Matches(x => x.MaxUsedCount == 10 && !x.IsDisabled && x.UsedBy.Count == 0),
            A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _unitOfWork.CompleteAsync(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
    }

    private static void SetGeneratedAt(InstructorPass instructorPass, DateTime value)
    {
        var generatedAtProperty = typeof(InstructorPass).GetProperty(nameof(InstructorPass.GeneratedAt))!;
        generatedAtProperty.SetValue(instructorPass, value);
    }
}
