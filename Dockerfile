# ================================
# Stage 1: Build
# ================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ClinicManagementSystem.slnx .
COPY ClinicManagementSystem.API/ClinicManagementSystem.API.csproj                             ClinicManagementSystem.API/
COPY ClinicManagementSystem.Application/ClinicManagementSystem.Application.csproj             ClinicManagementSystem.Application/
COPY ClinicManagementSystem.Infrastructure/ClinicManagementSystem.Infrastructure.csproj       ClinicManagementSystem.Infrastructure/
COPY ClinicManagementSystem.Domain/ClinicManagementSystem.Domain.csproj                       ClinicManagementSystem.Domain/
COPY ClinicManagementSystem.UnitTests/ClinicManagementSystem.UnitTests.csproj                 ClinicManagementSystem.UnitTests/

# Restore dependencies
RUN dotnet restore

# Copy all source files
COPY . .

# Build and publish
WORKDIR /src/ClinicManagementSystem.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# ================================
# Stage 2: Runtime
# ================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/Logs

# Copy published output
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "ClinicManagementSystem.API.dll"]