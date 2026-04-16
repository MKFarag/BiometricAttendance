namespace BiometricAttendance.Application.Contracts.Fingerprint;

/// <summary>
/// Returned by the Match endpoint — the student whose fingerprint was recognized.
/// </summary>
public record StudentAttendanceResponse(int Id, string Name);
