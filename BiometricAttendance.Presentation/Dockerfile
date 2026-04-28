# ── Build Stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution & project files first (layer caching)
COPY *.slnx ./
COPY BiometricAttendance.Domain/*.csproj                 BiometricAttendance.Domain/
COPY BiometricAttendance.Application/*.csproj            BiometricAttendance.Application/
COPY BiometricAttendance.Infrastructure/*.csproj         BiometricAttendance.Infrastructure/
COPY BiometricAttendance.Presentation/*.csproj           BiometricAttendance.Presentation/

RUN dotnet restore

# Copy everything and publish
COPY . .
WORKDIR /src/BiometricAttendance.Presentation
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Runtime Stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BiometricAttendance.Presentation.dll"]
