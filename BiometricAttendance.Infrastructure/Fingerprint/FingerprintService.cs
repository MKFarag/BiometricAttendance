namespace BiometricAttendance.Infrastructure.Fingerprint;

public sealed class FingerprintService(
    ISerialPortService serialPortService, IOptions<EnrollmentCommands> enrollmentOptions, FingerprintStatus fingerprintStatus,
    IStudentService studentService, ILogger<FingerprintService> logger) : IFingerprintService
{
    private readonly EnrollmentCommands _commands = enrollmentOptions.Value;
    private readonly ISerialPortService _serialPortService = serialPortService;
    private readonly FingerprintStatus _fingerprintStatus = fingerprintStatus;
    private readonly IStudentService _studentService = studentService;
    private readonly ILogger<FingerprintService> _logger = logger;

    private const int _enrollmentCheckTimeout = 6;

    public async Task ExecuteEnrollmentJob(int studentId)
    {
        var startResult = await StartAsync();

        _fingerprintStatus.StartEnrollment();

        if (startResult.IsFailure)
        {
            _logger.LogError("Error while trying to start the fingerprint: {error}", startResult.Error.Description);
            return;
        }

        await Task.Delay(1000);

        SendEnrollmentStart();

        var endTime = DateTime.Now.AddSeconds(_enrollmentCheckTimeout);
        while (DateTime.Now < endTime)
        {
            await Task.Delay(1000);

            var fingerprintId = GetFingerId();

            if (fingerprintId.IsFailure)
                continue;

            await _studentService.SetFingerprintAsync(studentId, fingerprintId.Value, CancellationToken.None);

            _logger.LogInformation("The enrollment is succeed.");

            break;
        }

        SendEnrollmentDeny();

        _logger.LogWarning("Enrollment is over");

        _fingerprintStatus.EndEnrollment();

        Stop();
    }

    public async Task ExecuteStartAttendanceJob()
    {
        var startResult = await StartAsync();

        if (startResult.IsFailure)
        {
            _logger.LogError("Error while trying to start the fingerprint: {error}", startResult.Error.Description);
            return;
        }

        List<int> fingerprintIds = [];

        _logger.LogInformation("Start reading from fingerprint...");

        while (_fingerprintStatus.IsAttendanceActionWorking)
        {
            await Task.Delay(1000);

            var fingerprintId = GetFingerId();

            if (fingerprintId.IsFailure && fingerprintId.Error.StatusCode == 503)
            {
                _fingerprintStatus.EndAttendanceAction();
                _logger.LogCritical("Fingerprint service has a critical error please press End button");
            }

            if (fingerprintId.IsFailure || fingerprintIds.Contains(fingerprintId.Value))
                continue;

            fingerprintIds.Add(fingerprintId.Value);

            _logger.LogInformation("Fingerprint id #{fid} has been added", fingerprintId.Value);
        }

        if (fingerprintIds.Count > 0)
        {
            _logger.LogInformation("Sending data...");
            _fingerprintStatus.SetFingerprintIds(fingerprintIds);
            _logger.LogInformation("Data sent successfully");
        }
        else if (fingerprintIds.Count == 0)
            _logger.LogWarning("No data has been read");

        _logger.LogWarning("Reading service has been stopped");

        Stop();
    }

    #region Private

    private async Task<Result> StartAsync()
    {
        if (_fingerprintStatus.IsEnabled)
            return Result.Failure(FingerprintErrors.AlreadyWorking);

        try
        {
            _serialPortService.Start();
            _logger.LogInformation("Fingerprint service started successfully");

            _fingerprintStatus.Enable();
            await Task.Delay(1000);

            return Result.Success();
        }
        catch
        {
            return Result.Failure(FingerprintErrors.StartFailed);
        }
    }

    private Result Stop()
    {
        if (!_fingerprintStatus.IsEnabled)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _logger.LogWarning("Stopping fingerprint service...");
        _serialPortService.Stop();

        _fingerprintStatus.Disable();
        _logger.LogInformation("Fingerprint service stopped successfully");

        return Result.Success();
    }

    private Result<int> GetFingerId()
    {
        if (!_fingerprintStatus.IsEnabled)
            return Result.Failure<int>(FingerprintErrors.ServiceUnavailable);

        var latestId = _serialPortService.LatestProcessedFingerprintId;

        if (string.IsNullOrEmpty(latestId))
            return Result.Failure<int>(FingerprintErrors.NoData);

        if (!int.TryParse(latestId, out int fingerId))
            return Result.Failure<int>(FingerprintErrors.InvalidData);

        return Result.Success(fingerId);
    }

    private Result SendEnrollmentStart()
    {
        if (!_fingerprintStatus.IsEnabled)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _serialPortService.SendCommand(_commands.Start);
        return Result.Success();
    }

    private Result SendEnrollmentDeny()
    {
        if (!_fingerprintStatus.IsEnabled)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _serialPortService.SendCommand(_commands.Deny);
        return Result.Success();
    }

    private Result SendEnrollmentAllow()
    {
        if (!_fingerprintStatus.IsEnabled)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _serialPortService.SendCommand(_commands.Allow);
        return Result.Success();
    }

    private Result SendDeleteAll()
    {
        if (!_fingerprintStatus.IsEnabled)
            return Result.Failure(FingerprintErrors.ServiceUnavailable);

        _serialPortService.SendCommand(_commands.Delete);
        return Result.Success();
    }

    #endregion
}
