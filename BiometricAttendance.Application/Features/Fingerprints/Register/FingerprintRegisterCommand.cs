namespace BiometricAttendance.Application.Features.Fingerprints.Register;

public record FingerprintRegisterCommand(int StudentId) : IRequest<Result>;
