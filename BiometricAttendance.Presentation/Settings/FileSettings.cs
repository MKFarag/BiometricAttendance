namespace BiometricAttendance.Presentation.Settings;

public static class FileSettings
{
    public const int MaxFileSizeInMB = 2;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    public static readonly string[] BlockedSignatures = ["4D-5A", "2F-2A", "D0-CF"];
    public static readonly string[] AllowedImagesExtensions = [".jpg", ".jpeg", ".png"];
}
