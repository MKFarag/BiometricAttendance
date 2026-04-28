namespace BiometricAttendance.Application.Contracts.Fingerprint;

public record FingerprintResponse(
    int Id,
    DateTime RegisteredAt
);
