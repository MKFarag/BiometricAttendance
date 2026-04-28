using BiometricAttendance.Domain.Abstraction;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Text.Json;

namespace BiometricAttendance.Presentation;

internal static class GlobalExtensions
{
    extension(Result result)
    {
        internal ObjectResult ToProblem()
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot create a problem details response for a successful result.");

            var problem = Results.Problem(statusCode: result.Error.StatusCode);

            var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            problemDetails!.Extensions = new Dictionary<string, object?>
            {
                {
                    "error", new Dictionary<string, string>
                    {
                        { "code", result.Error.Code },
                        { "description", result.Error.Description }
                    }
                }
            };

            return new ObjectResult(problemDetails);
        }
    }

    extension(ControllerBase controller)
    {
        internal IActionResult ToProblem(ValidationResult result)
        {
            var modelState = new ModelStateDictionary();

            foreach (var error in result.Errors)
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);

            return controller.ValidationProblem(modelState);
        }
    }

    extension(ClaimsPrincipal user)
    {
        internal string? GetId() 
            => user.FindFirstValue(ClaimTypes.NameIdentifier);

        internal List<string> GetRoles()
            => [.. user.FindAll("roles")
                   .Select(c => JsonSerializer.Deserialize<List<string>>(c.Value))
                   .Where(r => r is not null)
                   .SelectMany(r => r!)];
    }
}
