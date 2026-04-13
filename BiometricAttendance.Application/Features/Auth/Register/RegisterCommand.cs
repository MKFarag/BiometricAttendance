namespace BiometricAttendance.Application.Features.Auth.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<Result>;
