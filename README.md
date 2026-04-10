# 🏥 Clinic Management System API

A production-ready RESTful API for managing clinic operations built with **ASP.NET Core (.NET 10)** following **Clean Architecture** and **CQRS** patterns.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Running Tests](#running-tests)
- [Improvements Roadmap](#improvements-roadmap)

---

## Overview

The Clinic Management System API provides a comprehensive backend solution for managing:

- **Doctors** and their specializations
- **Patients** and their medical records
- **Appointments** scheduling and state management
- **Sessions** (consultations) with notes and prescriptions
- **Billing** records per session
- **Authentication** with role-based access control

---

## Architecture

The project follows **Clean Architecture** with clear separation of concerns across four layers:

```
ClinicManagementSystem/
├── Domain                  → Entities, base classes
├── Application             → CQRS Handlers, DTOs, Validators, Profiles, Interfaces
├── Infrastructure          → EF Core, Repositories, UnitOfWork, Identity
└── API                     → Controllers, Middlewares, BaseController
```

### Patterns Used

- **CQRS** with MediatR — Commands and Queries are separated
- **Repository + Unit of Work** — abstracts data access
- **Pipeline Behavior** — FluentValidation runs before every handler
- **Generic Repository** — reusable data access with projection support

---

## Features

| Feature                     | Status |
| --------------------------- | ------ |
| CRUD for all entities       | ✅      |
| JWT Authentication          | ✅      |
| Role-based Authorization    | ✅      |
| Refresh Token               | ✅      |
| Global Exception Handling   | ✅      |
| AutoMapper                  | ✅      |
| FluentValidation            | ✅      |
| MediatR + CQRS              | ✅      |
| Pagination                  | ✅      |
| Rate Limiting               | ✅      |
| API Versioning              | ✅      |
| Swagger UI with JWT         | ✅      |
| Unit Testing                | ✅      |
| Separate Create/Update DTOs | ✅      |
| Logging (Serilog)           | ✅      |
| Caching                     | ✅      |
| Localization                | ⏳      |
| Multi-Device Refresh Token  | ⏳      |

---

## Tech Stack

| Technology            | Purpose          |
| --------------------- | ---------------- |
| ASP.NET Core 10       | Web framework    |
| Entity Framework Core | ORM              |
| SQL Server            | Database         |
| ASP.NET Core Identity | User management  |
| JWT Bearer            | Authentication   |
| MediatR               | CQRS mediator    |
| FluentValidation      | Input validation |
| AutoMapper            | Object mapping   |
| Swashbuckle 10.x      | Swagger UI       |
| xUnit v3              | Unit testing     |
| Moq                   | Mocking          |
| FluentAssertions      | Test assertions  |

---

## Project Structure

```
Application/
├── Common/
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs
│   └── Pagination/
│       ├── PaginationRequest.cs
│       └── PaginatedResponse.cs
├── DTOs/
│   ├── CreateDTOs/
│   └── ResponseDTOs/
├── Exceptions/
│   ├── NotFoundException.cs
│   ├── BadRequestException.cs
│   ├── DuplicateException.cs
│   └── ValidationException.cs
├── Features/
│   ├── DoctorSpecialization/
│   │   ├── Commands/
│   │   │   ├── Create/
│   │   │   ├── Update/
│   │   │   └── Delete/
│   │   └── Queries/
│   │       ├── GetAll/
│   │       └── GetById/
│   ├── AppointmentState/
│   ├── Doctor/
│   ├── Patient/
│   ├── Appointment/
│   ├── Session/
│   └── Billing/
├── Helpers/
│   └── GenderHelper.cs
├── MappingProfiles/
├── Services/
│   └── Auth/
│       ├── IAuthService.cs
│       └── AuthService.cs
└── Settings/
    ├── JwtSettings.cs
    └── RateLimitSettings.cs
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   
   ```bash
   git clone https://github.com/AhmedEmadHassan/ClinicManagementSystem.git
   cd ClinicManagementSystem
   ```

2. **Restore dependencies**
   
   ```bash
   dotnet restore
   ```

3. **Update connection string** in `appsettings.json`:
   
   ```json
   "ConnectionStrings": {
    "SqlServerConnection": "Server=YOUR_SERVER;Database=ClinicManagementDB;Trusted_Connection=True;"
   }
   ```

4. **Apply migrations**
   
   ```bash
   dotnet ef database update --project ClinicManagementSystem.Infrastructure --startup-project ClinicManagementSystem.API
   ```

5. **Run the application**
   
   ```bash
   dotnet run --project ClinicManagementSystem.API
   ```

6. **Open Swagger UI**
   
   ```
   https://localhost:{port}/swagger
   ```

---

## Configuration

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "your-connection-string"
  },
  "JwtSettings": {
    "Key": "YourSuperSecretKeyHereMustBe32CharsMin!",
    "Issuer": "ClinicManagementSystem",
    "Audience": "ClinicManagementSystemUsers",
    "DurationInMinutes": 60,
    "RefreshTokenDurationInDays": 7
  },
  "RateLimitSettings": {
    "GlobalFixedWindow": {
      "PermitLimit": 100,
      "WindowInSeconds": 60
    },
    "AuthSlidingWindow": {
      "PermitLimit": 10,
      "WindowInSeconds": 60,
      "SegmentsPerWindow": 6
    }
  }
}
```

---

## API Endpoints

All endpoints are versioned under `api/v1/`.

### Auth

| Method | Endpoint                      | Access     |
| ------ | ----------------------------- | ---------- |
| POST   | `api/v1/Auth/register`        | Anonymous  |
| POST   | `api/v1/Auth/login`           | Anonymous  |
| POST   | `api/v1/Auth/refresh-token`   | Anonymous  |
| POST   | `api/v1/Auth/logout`          | Authorized |
| POST   | `api/v1/Auth/change-password` | Authorized |

### Doctor Specialization

| Method | Endpoint                           | Roles |
| ------ | ---------------------------------- | ----- |
| GET    | `api/v1/DoctorSpecialization`      | Admin |
| GET    | `api/v1/DoctorSpecialization/{id}` | Admin |
| POST   | `api/v1/DoctorSpecialization`      | Admin |
| PUT    | `api/v1/DoctorSpecialization/{id}` | Admin |
| DELETE | `api/v1/DoctorSpecialization/{id}` | Admin |

### Doctor

| Method | Endpoint             | Roles |
| ------ | -------------------- | ----- |
| GET    | `api/v1/Doctor`      | Admin |
| GET    | `api/v1/Doctor/{id}` | Admin |
| POST   | `api/v1/Doctor`      | Admin |
| PUT    | `api/v1/Doctor/{id}` | Admin |
| DELETE | `api/v1/Doctor/{id}` | Admin |

### Patient

| Method | Endpoint              | Roles               |
| ------ | --------------------- | ------------------- |
| GET    | `api/v1/Patient`      | Admin, Receptionist |
| GET    | `api/v1/Patient/{id}` | Admin, Receptionist |
| POST   | `api/v1/Patient`      | Admin, Receptionist |
| PUT    | `api/v1/Patient/{id}` | Admin, Receptionist |
| DELETE | `api/v1/Patient/{id}` | Admin, Receptionist |

### Appointment

| Method | Endpoint                  | Roles               |
| ------ | ------------------------- | ------------------- |
| GET    | `api/v1/Appointment`      | All Roles           |
| GET    | `api/v1/Appointment/{id}` | All Roles           |
| POST   | `api/v1/Appointment`      | Admin, Receptionist |
| PUT    | `api/v1/Appointment/{id}` | Admin, Receptionist |
| DELETE | `api/v1/Appointment/{id}` | Admin               |

### Session

| Method | Endpoint              | Roles         |
| ------ | --------------------- | ------------- |
| GET    | `api/v1/Session`      | Admin, Doctor |
| GET    | `api/v1/Session/{id}` | Admin, Doctor |
| POST   | `api/v1/Session`      | Admin, Doctor |
| PUT    | `api/v1/Session/{id}` | Admin, Doctor |
| DELETE | `api/v1/Session/{id}` | Admin         |

### Billing

| Method | Endpoint              | Roles               |
| ------ | --------------------- | ------------------- |
| GET    | `api/v1/Billing`      | Admin, Receptionist |
| GET    | `api/v1/Billing/{id}` | Admin, Receptionist |
| POST   | `api/v1/Billing`      | Admin, Receptionist |
| PUT    | `api/v1/Billing/{id}` | Admin, Receptionist |
| DELETE | `api/v1/Billing/{id}` | Admin               |

### Pagination

All `GET` (list) endpoints support pagination via query parameters:

```
GET api/v1/Doctor?pageNumber=1&pageSize=10
```

---

## Authentication

The API uses **JWT Bearer** authentication with **ASP.NET Core Identity**.

### Roles

| Role           | Description                             |
| -------------- | --------------------------------------- |
| `Admin`        | Full access to all endpoints            |
| `Doctor`       | Access to sessions and appointments     |
| `Patient`      | Read access to appointments             |
| `Receptionist` | Manages patients, appointments, billing |

### How to authenticate in Swagger

1. Call `POST api/v1/Auth/login` with valid credentials
2. Copy the `accessToken` from the response
3. Click the 🔒 **Authorize** button in Swagger UI
4. Paste the token (**without** the `Bearer ` prefix)
5. All subsequent requests will include the token automatically

### Register example

```json
{
  "userName": "AdminUser",
  "email": "admin@clinic.com",
  "password": "P@$$w0rd",
  "role": "Admin"
}
```

---

## Running Tests

The solution includes unit tests covering Handlers, Validators, and AuthService.

```bash
dotnet test
```

### Test Coverage

| Area        | Tests                                                 |
| ----------- | ----------------------------------------------------- |
| Handlers    | GetById, Create, Update, Delete for all entities      |
| Validators  | Valid and invalid inputs for all commands             |
| AuthService | Register, Login, Logout, ChangePassword, RefreshToken |

---

## Improvements Roadmap

- [x] Logging (Serilog)
- [x] Caching
- [ ] Localization
- [ ] Multi-Device Refresh Token Support
- [ ] Use Cases (beyond CRUD)
- [ ] Integration Tests
- [x] Docker Support
- [ ] CI/CD Pipeline
