namespace BiometricAttendance.Infrastructure.Settings;

/// <summary>
/// Serial-port command strings sent to the fingerprint reader to control enrollment mode.
/// Bind from appsettings.json under the "EnrollmentCommands" section.
/// </summary>
public class EnrollmentCommands
{
    public const string SectionName = nameof(EnrollmentCommands);

    /// <summary>Command that tells the reader to start an enrollment sequence.</summary>
    public string Start { get; init; } = string.Empty;

    /// <summary>Command that enables enrollment (allows new fingerprints to be registered).</summary>
    public string Allow { get; init; } = string.Empty;

    /// <summary>Command that disables enrollment.</summary>
    public string Deny { get; init; } = string.Empty;

    /// <summary>Command that queries the reader for the current enrollment state.</summary>
    public string Check { get; init; } = string.Empty;

    /// <summary>Password-protected command that wipes all stored fingerprint data from the reader.</summary>
    public string Delete { get; init; } = string.Empty;
}
