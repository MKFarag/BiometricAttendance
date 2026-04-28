namespace BiometricAttendance.Application.Contracts.Profile;

public record StudentProfileResponse(
    string UserId,
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    int StudentId,
    int Level,
    DepartmentResponse Department,
    FingerprintResponse? Fingerprint
);
