namespace BiometricAttendance.Domain.Constants;

/// <summary>
/// Holds transient runtime state for the fingerprint reader session.
/// Registered as a singleton so the state is shared across the request pipeline.
/// </summary>
public sealed class FingerprintStatus
{
    /// <summary>Whether the serial port / fingerprint reader is currently open and running.</summary>
    public bool IsEnabled { get; private set; } = false;

    /// <summary>Whether an attendance recording session is currently active.</summary>
    public bool IsActionButtonEnabled { get; set; } = false;

    /// <summary>Fingerprint IDs collected during the active attendance session.</summary>
    public List<int> StudentsId { get; set; } = [];

    public void Enable()
        => IsEnabled = true;

    public void Disable()
        => IsEnabled = false;
}
