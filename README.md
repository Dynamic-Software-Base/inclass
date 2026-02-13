[![Board Status](https://dev.azure.com/narjis-lola/6dc47091-22e3-4e00-b118-e1f3aff4e30a/16b421fc-ee7f-4b1c-8c10-fa4b84333a5a/_apis/work/boardbadge/403e8b81-ef57-4fb0-ae4e-05dcedaa7a8f)](https://dev.azure.com/narjis-lola/6dc47091-22e3-4e00-b118-e1f3aff4e30a/_boards/board/t/16b421fc-ee7f-4b1c-8c10-fa4b84333a5a/Microsoft.RequirementCategory)
# Clean Architecture Template

What's included in the template?

- SharedKernel project with common Domain-Driven Design abstractions.
- Domain layer with sample entities.
- Application layer with abstractions for:
  - CQRS
  - Example use cases
  - Cross-cutting concerns (logging, validation)
- Infrastructure layer with:
  - Authentication
  - Permission authorization
  - EF Core, PostgreSQL
  - Serilog
- Seq for searching and analyzing structured logs
  - Seq is available at http://localhost:8081 by default
- Testing projects
  - Architecture testing

# INCLASS

Brief description of what this does.

## Prerequisites
- .NET 10 SDK
- Docker Desktop
- (we should add any other requirements here)

## Getting Started

git clone https://github.com/Dynamic-Software-Base/inclass.git \
cd your-app \
dotnet restore\
dotnet run --project src/YourApp.API

## Project Structure

src/
YourApp.API/          → Web API entry point
YourApp.Application/  → Use cases and business logic
YourApp.Domain/       → Entities and domain rules
YourApp.Infrastructure/ → DB, external services

tests/
YourApp.Unit.Tests/
YourApp.Integration.Tests/

## Branching
- main        → production
- staging     → pre-production
- develop     → active development