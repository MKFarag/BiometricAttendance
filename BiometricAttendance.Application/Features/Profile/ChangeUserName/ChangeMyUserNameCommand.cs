namespace BiometricAttendance.Application.Features.Profile.ChangeUserName;

public record ChangeMyUserNameCommand(string UserId, string NewUserName) : IRequest<Result>;