namespace BiometricAttendance.Domain.Errors;

public static class GlobalErrors
{
    public static Error InvalidInput(string field) =>
        new($"{field}.InvalidInput", $"The provided value for '{field}' is invalid.", StatusCodes.BadRequest);

    public static Error IdNotFound(string entity) =>
        new($"{entity}.NotFound", $"No {entity} record was found with the provided ID.", StatusCodes.NotFound);
}
