using System;
using System.Collections.Generic;
using System.Text;

namespace BiometricAttendance.Application.Contracts.Fingerprint;

public record FingerprintResponse(
    int Id,
    DateTime RegisteredAt
);
