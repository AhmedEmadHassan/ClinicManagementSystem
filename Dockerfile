#Prepare Stage

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app
COPY *.slnx ./
COPY ClinicManagementSystem.API/*.csproj ./ClinicManagementSystem.API/
COPY ClinicManagementSystem.Application/*.csproj ./ClinicManagementSystem.Application/
COPY ClinicManagementSystem.Domain/*.csproj ./ClinicManagementSystem.Domain/
COPY ClinicManagementSystem.Infrastructure/*.csproj ./ClinicManagementSystem.Infrastructure/
COPY ClinicManagementSystem.UnitTests/*.csproj ./ClinicManagementSystem.Infrastructure/
# Restore libraries
RUN dotnet restore
# Copy Everything
COPY . .
# Build and Publish
RUN dotnet publish -c release -o out

# Build Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ClinicManagementSystem.dll"]
