# ClinicManagementSystem

A **Clinic Management System** — a multi‑project .NET solution providing core backend logic and API layers to support clinic workflows like patient & doctor management, appointments, and related business operations.

---

## 🧩 Overview

ClinicManagementSystem is built in **C# (.NET)** and follows a **layered architecture** (Domain‑Driven Design & Clean Architecture style). It’s designed to be a backend platform powering clinic workflows such as:

- 💉 Managing **patients**, **doctors**, and **clinics**
- 📅 Handling **appointment scheduling**
- 🩺 Supporting business logic for healthcare operations
- 🧪 Including **unit tests** to validate core features

This structure makes the project suitable as a foundation for building REST APIs, admin UIs, or mobile clients for a medical clinic management platform.

---

## 📁 Repository Structure

ClinicManagementSystem/  
├── ClinicManagementSystem.API # RESTful API entrypoint  
├── ClinicManagementSystem.Application # Application services, DTOs, workflows  
├── ClinicManagementSystem.Domain # Core business models & interfaces  
├── ClinicManagementSystem.Infrastructure# Data access, EF Core or external services 
├── ClinicManagementSystem.UnitTests # Unit tests for business logic  
├── ClinicManagementSystem.slnx # .NET solution file  
├── .gitignore  
└── README.md

---

### 📌 Project Roles

| Project            | Purpose                                                    |
| ------------------ | ---------------------------------------------------------- |
| **API**            | Web API layer exposing endpoints for clinic operations     |
| **Application**    | Contains use cases, services, DTOs, and orchestrates logic |
| **Domain**         | Business entities, enums, policies, and core interfaces    |
| **Infrastructure** | Persistence & external dependencies (databases, libraries) |
| **UnitTests**      | Ensures correctness via automated tests                    |

*This layered architecture separates concerns, enhances testability, and follows enterprise design principles.* :contentReference[oaicite:0]{index=0}

---

## 🚀 Key Features

> Because of the lack of an existing README or documentation, this is inferred from the standard capabilities of projects organized in this pattern.

✔️ CRUD for Patients, Doctors, Appointments  
✔ Business rules encapsulated in Application layer  
✔ Persistence via Infrastructure project  
✔ API exposed in a RESTful pattern  
✔ Automated tests in UnitTests project  

---

## ⚙️ Dependencies & Setup

ClinicManagementSystem is a **.NET (Core or Standard) solution** and requires:

- 🧰 **.NET SDK (6.0 / 7.0 / 8.0)** — compatible with the project’s target
- 🛠 Visual Studio or VS Code with C# tooling
- 📦 Entity Framework Core (if used) or another ORM
- 🔐 Database (SQL Server / SQLite / PostgreSQL — depending on configuration)

---

### Installation

1. Clone the repo:
   
   ```bash
   git clone https://github.com/AhmedEmadHassan/ClinicManagementSystem.git  
   cd ClinicManagementSystem
   ```

2. Restore .NET packages:   
   
   ```bash
   dotnet restore
   ```

3. Configure database settings in `appsettings.json` under the API or Infrastructure project.

---  

## 🏃 Running the Project

From the solution root:  

```bash
cd ClinicManagementSystem.API  
dotnet run
```

This starts the API server on the configured port (typically **http://localhost:5000**). You can then browse endpoints using Swagger, Postman, or a frontend application.

---

## 🧪 Testing

To execute the unit tests:

```bash
cd ClinicManagementSystem.UnitTests  
dotnet test
```

This runs all defined tests against application and domain logic.