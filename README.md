# BiometricAttendance

BiometricAttendance is a fingerprint-based attendance management API built with ASP.NET Core 10 and Clean Architecture. The project focuses on real attendance workflows for educational institutions, combining identity management, permission-based authorization, fingerprint device integration, attendance analytics, background jobs, and distributed caching.

## Overview

This project is not just a CRUD API. It covers the full operational flow around attendance:

- User registration, login, refresh token rotation, email confirmation, and password reset
- Pending-user onboarding into `Student` or `Instructor`
- Role and permission management using claims
- Fingerprint registration through a real serial-port device
- Attendance marking manually or through a fingerprint attendance session
- Student course enrollment and academic progression
- Background jobs for refresh-token cleanup and instructor-pass rotation
- Distributed idempotency with `HybridCache` backed by `Redis`

## Why This Project Is Strong

- Real hardware integration: fingerprint workflows are connected to a serial-port service, not mocked HTTP-only logic
- Clean Architecture: clear separation between `Domain`, `Application`, `Infrastructure`, and `Presentation`
- Custom CQRS pipeline: requests and handlers are implemented inside the solution without adding MediatR
- Granular permissions: authorization is claim-based, not only role-based
- Hybrid caching: the project uses `HybridCache` with `StackExchangeRedis` for distributed cache-backed operations
- Idempotent fingerprint actions: duplicate client requests can be safely rejected or replayed
- Background reliability: Hangfire is used for recurring operational tasks
- Production-oriented defaults: rate limiting, health checks, structured logging, OpenAPI docs, and problem-details error handling are already wired in
- Good test coverage for the application layer: currently `132` unit tests in `BiometricAttendance.Application.Test`

## Implemented Features

### Authentication and account lifecycle

- JWT access tokens
- Refresh tokens with revoke and cleanup flow
- Email confirmation
- Forgot password and reset password
- Profile endpoints for:
  - viewing current profile
  - changing password
  - changing email with confirmation
  - changing username

### Roles, permissions, and onboarding

- Default roles seeded through migrations:
  - `Admin`
  - `Pending`
  - `Student`
  - `Instructor`
  - `SuperInstructor`
- Permission-based authorization via role claims such as:
  - `attendance:read`
  - `attendance:mark`
  - `fingerprint:register`
  - `fingerprint:action`
  - `student:promote`
  - `role:add`
  - `user:modify`
- Pending users can complete registration as students
- Pending users can become instructors using a rotating instructor pass

### Student, course, and department workflows

- Department CRUD
- Course CRUD
- User CRUD
- Role CRUD with permission assignment
- Student registration completion
- Student self-enrollment in courses
- Student course removal
- Student department change
- Student level change
- Student promotion
- Force-removing a student while preserving the identity account

### Attendance workflows

- Manual attendance marking per student, course, and week
- Fingerprint attendance session:
  - start session
  - collect fingerprint identifiers from the device
  - end session and persist attendance for a specific course/week
- Attendance queries for:
  - current student attendance summary
  - weekly course attendance
  - total attendance percentage per course
  - detailed student attendance in a course

### Fingerprint workflows

- Fingerprint registration for a specific student
- Attendance session start/stop through the fingerprint service
- Serial port configuration through app settings
- Runtime fingerprint state managed by infrastructure services

### Reliability and observability

- Global exception handling with problem-details responses
- Serilog request logging
- Health checks for:
  - SQL Server
  - Hangfire
  - mail provider
- Hangfire dashboard protected with basic authentication
- API versioning using the `x-api-version` header
- Swagger UI and ReDoc in development
- Rate limiting policies for IP, user, sliding window, and concurrency

## Redis, Hybrid Cache, and Idempotency

One of the strongest technical parts in this project is the caching and request-safety setup.

### Hybrid Cache

The project registers:

- `services.AddHybridCache()`
- `services.AddStackExchangeRedisCache(...)`

This gives a two-layer cache model:

- local in-memory cache for fast hot reads
- distributed Redis cache for cross-instance consistency

The `CacheService` wraps `HybridCache` and supports:

- `GetOrCreateAsync`
- explicit set/remove
- removing multiple keys
- tag-based invalidation

### Idempotent fingerprint endpoints

The custom `IdempotentAttribute` uses `HybridCache` to protect sensitive POST operations from duplicate execution.

Currently it is applied to:

- `POST /api/Fingerprints/register/{studentId}`
- `POST /api/Fingerprints/attendance/start`

Behavior:

- client sends `X-Idempotency-Key`
- first request acquires a short distributed lock
- concurrent duplicate request gets `409 Conflict`
- successful response is cached briefly and can be replayed safely
- failed responses are not permanently cached

This is especially useful for hardware-triggered or retried client calls where duplicate fingerprint actions would be harmful.

## Instructor Pass Flow

The instructor onboarding flow is another notable feature.

- A pending user can request instructor role assignment using an instructor pass
- The pass is single-code based and usage-tracked
- A pass has a maximum number of uses
- Hangfire renews or rotates the pass daily depending on whether it has been used
- If a pass is exhausted, a new one is generated automatically

## Architecture

The solution follows Clean Architecture:

```text
BiometricAttendance.Domain
BiometricAttendance.Application
BiometricAttendance.Infrastructure
BiometricAttendance.Presentation
BiometricAttendance.Application.Test
```

### Layer responsibilities

- `Domain`
  - entities
  - constants
  - errors
  - repository contracts
  - result/error abstractions
- `Application`
  - use cases
  - commands and queries
  - handlers
  - contracts/DTOs
  - application services
- `Infrastructure`
  - EF Core persistence
  - repositories
  - identity integration
  - JWT provider
  - mail service
  - cache service
  - fingerprint device integration
  - health checks
- `Presentation`
  - controllers
  - dependency injection
  - rate limiting
  - OpenAPI configuration
  - exception handling
  - idempotency filter

## Tech Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| Language | C# / .NET 10 |
| Architecture | Clean Architecture |
| Data access | Entity Framework Core 10 |
| Database | SQL Server |
| Authentication | ASP.NET Core Identity + JWT |
| Authorization | Role claims + custom permission policies |
| Caching | `HybridCache` + `Redis` |
| Background jobs | Hangfire |
| Validation | FluentValidation |
| Mapping | Mapster |
| Logging | Serilog |
| API docs | NSwag, Swagger UI, ReDoc |
| Health checks | AspNetCore HealthChecks |
| Testing | xUnit + FakeItEasy |
| Hardware integration | `System.IO.Ports` serial communication |

## Main API Areas

- `/Auth`
  - login
  - refresh token
  - revoke refresh token
  - register
  - confirm email
  - resend confirmation
  - forgot/reset password
- `/api/Users`
- `/api/Roles`
- `/api/Departments`
- `/api/Courses`
- `/api/Students`
- `/api/Instructors`
- `/api/Fingerprints`
- `/api/Attendances`
- `/me`
- `/health`
- `/jobs`

## Data Model Summary

Main business entities:

- `Student`
- `Department`
- `Course`
- `StudentCourse`
- `Attendance`
- `Fingerprint`
- `InstructorPass`

Identity/persistence-related entities:

- `ApplicationUser`
- `ApplicationRole`
- `RefreshToken`

Important notes from the current model:

- a course belongs to one department in the current implementation
- students are linked to departments and can enroll in multiple courses
- fingerprints are stored as registered IDs tied to students
- attendance is tracked by `studentId`, `courseId`, and `weekNumber`

## Migrations

The project already contains EF Core migrations, including:

1. `AddIdentities`
2. `AddDefaultData`
3. `AddEntities`
4. `AddPendingRole`
5. `AddIndexesAndFixTheStudentWithUserFK`
6. `ModifyInstructorPass`
7. `AddStudentCoursesTable`
8. `RemoveDepartmentCourseTable`
9. `DeleteUpdatedAtInFingerprint`
10. `AddPermissionsAndSuperInstructorRole`

These migrations do more than create tables. They also seed important data such as:

- default roles
- default role claims/permissions
- a default admin record

### Apply migrations

Use the presentation project as the startup project:

```bash
dotnet ef database update --project BiometricAttendance.Infrastructure --startup-project BiometricAttendance.Presentation
```

### Create a new migration

```bash
dotnet ef migrations add <MigrationName> --project BiometricAttendance.Infrastructure --startup-project BiometricAttendance.Presentation --output-dir Persistence/Migrations
```

## Configuration

Important configuration sections used by the project:

- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:HangfireConnection`
- `ConnectionStrings:Redis`
- `Jwt`
- `MailSettings`
- `AppUrlSettings`
- `RateLimitingOptions`
- `EnrollmentCommands`
- `SerialPort`
- `AllowedOrigins`
- `Hangfire`

### Example appsettings values

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Database=BiometricAttendance;Integrated Security=True;Encrypt=false;Trust Server Certificate=True;",
    "HangfireConnection": "Data Source=.;Database=BiometricAttendanceJobs;Integrated Security=True;Encrypt=false;Trust Server Certificate=True;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "BiometricAttendance",
    "Audience": "BiometricAttendance users",
    "ExpiryMinutes": 30
  },
  "SerialPort": {
    "PortName": "COM4",
    "BaudRate": 9600
  }
}
```

## Running the Project

### Local development

1. Install .NET 10 SDK
2. Prepare SQL Server
3. Prepare Redis
4. Update configuration values in `BiometricAttendance.Presentation/appsettings.Development.json` or user secrets
5. Apply migrations
6. Run the API

```bash
dotnet restore
dotnet ef database update --project BiometricAttendance.Infrastructure --startup-project BiometricAttendance.Presentation
dotnet run --project BiometricAttendance.Presentation
```

### Development endpoints

- Swagger UI: `/swagger`
- ReDoc: `/redoc`
- Health checks: `/health`
- Hangfire dashboard: `/jobs`

## Docker

There is a Docker setup in [BiometricAttendance.Presentation/docker-compose.yml](BiometricAttendance.Presentation/docker-compose.yml).

Current compose file provisions:

- API container
- SQL Server container

Important note:

- Redis is configured in the application, but the current compose file does not provision a Redis container yet
- if you want full distributed cache behavior for idempotency and `HybridCache`, run Redis separately or extend the compose file

## Security and Runtime Protections

- JWT bearer authentication
- refresh-token revoke flow
- lockout settings for failed logins
- confirmed-email requirement
- role + permission authorization
- rate limiting on sensitive endpoints
- idempotency protection on critical fingerprint operations
- centralized exception handling
- restricted Hangfire dashboard access

## Testing

The solution contains an application test project:

- `BiometricAttendance.Application.Test`

It currently includes `132` unit tests covering handlers and services across:

- users
- roles
- departments
- courses
- students
- attendances
- profile flows
- instructor-pass logic

## Current Notes

- The previous README mentioned features like PDF/Excel reports, Scalar, and some roadmap items that do not match the current codebase; this document reflects the implemented state instead
- Docker support exists, but Redis is still expected as an external dependency for full cache-backed behavior
- The fingerprint flow depends on the configured serial port and the connected device behavior

## License

This repository currently does not include a dedicated `LICENSE` file in the project root. Add one if you plan to publish the project publicly.
