//using BiometricAttendance.Application.Settings;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace BiometricAttendance.Infrastructure.Services;

///// <summary>
///// Background jobs called by Hangfire. Kept in Infrastructure because they rely
///// directly on <see cref="ISerialPortService"/> and must be a concrete, DI-resolvable class.
///// </summary>
//public class FingerprintJobService(
//    ISerialPortService serialPortService,
//    IStudentService studentService,
//    FingerprintState state,
//    IOptions<EnrollmentCommands> options,
//    ILogger<FingerprintJobService> logger) : IFingerprintJobService
//{
//    private readonly EnrollmentCommands _options = options.Value;
//    private const int EnrollmentJobTimeoutSeconds = 12;

//    public async Task ExecuteEnrollmentJob(int studentId)
//    {
//        var endTime = DateTime.Now.AddSeconds(EnrollmentJobTimeoutSeconds);

//        while (DateTime.Now < endTime)
//        {
//            await Task.Delay(1000);

//            var raw = serialPortService.LatestProcessedFingerprintId;

//            if (string.IsNullOrEmpty(raw) || !int.TryParse(raw, out int fingerId))
//                continue;

//            // TODO: Implement IStudentService.RegisterFingerprintAsync — persist hardware finger ID on the student row
//            var result = await studentService.RegisterFingerprintAsync(studentId, fingerId);

//            await Task.Delay(500);

//            if (result.IsSuccess)
//            {
//                serialPortService.DeleteLastValue();
//                break;
//            }

//            logger.LogCritical(
//                "Failed to register fingerprint #{fid} for student #{id}: {error}",
//                fingerId, studentId, result.Error.Description);
//            break;
//        }

//        serialPortService.SendCommand(_options.Deny);
//        logger.LogWarning("Enrollment job finished for student #{id}", studentId);

//        if (state.FpStatus)
//        {
//            serialPortService.Stop();
//            state.FpStatus = false;
//        }
//    }

//    public async Task ExecuteStartAttendanceJob()
//    {
//        List<int> fingerIds = [];
//        logger.LogInformation("Attendance reading started");

//        while (state.ActionButtonStatus)
//        {
//            await Task.Delay(1000);

//            if (!state.FpStatus)
//            {
//                state.ActionButtonStatus = false;
//                logger.LogCritical("Fingerprint service is down — press End to finalise");
//                break;
//            }

//            var raw = serialPortService.LatestProcessedFingerprintId;

//            if (string.IsNullOrEmpty(raw) || !int.TryParse(raw, out int fingerId) || fingerIds.Contains(fingerId))
//                continue;

//            fingerIds.Add(fingerId);
//            logger.LogInformation("Fingerprint #{fid} collected", fingerId);
//        }

//        if (fingerIds.Count > 0)
//        {
//            state.ActionButtonData = fingerIds;
//            logger.LogInformation("{count} fingerprint(s) stored in session state", fingerIds.Count);
//        }
//        else
//        {
//            logger.LogWarning("No fingerprints were collected during this session");
//        }

//        logger.LogWarning("Attendance reading stopped");
//    }
//}
