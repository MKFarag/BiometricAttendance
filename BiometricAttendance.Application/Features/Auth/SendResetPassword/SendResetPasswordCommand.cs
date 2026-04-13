namespace BiometricAttendance.Application.Features.Auth.SendResetPassword;

public record SendResetPasswordCommand(string Email) : IRequest<Result>;
