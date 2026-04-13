namespace BiometricAttendance.Application.Features.Auth.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result>;

