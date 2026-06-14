# LoanHub API

A loan management REST API built with **ASP.NET Core 8**, **Entity Framework Core**, and **AutoMapper**.
It handles customers, automated loan approval, repayment schedules, and payment tracking.

## Features

- Customer registration with validation (required names, unique personal number, 18+ check)
- Automated loan decisioning from credit score (`< 300` → Rejected, otherwise Approved)
- Automatic monthly payment (PMT) calculation and amortization schedule generation
- Payment recording with automatic loan status transitions (Closed / Overdue / Approved)
- JWT bearer authentication
- FluentValidation for request validation
- Global error-handling middleware
- AutoMapper for entity → DTO projection
- Soft delete with EF Core global query filters
- xUnit unit tests
- Swagger / OpenAPI documentation

## Tech & design

- **Single-project layered Web API**: `Controllers → Services → DbContext` (EF Core used directly in services).
- **SQLite** database, created automatically on first run (no DB server required).
- AutoMapper profiles for mapping, FluentValidation auto-validation, options-bound JWT config.

```
src/LoanHub.Api
├── Controllers      # API endpoints
├── Services         # Business logic (Customer, Loan, Payment, Auth, Token)
├── Data             # DbContext + database bootstrapper/seeder
├── Entities         # EF Core entities + enum
├── Contracts        # Request/response DTOs (records)
├── Mapping          # AutoMapper profile + loan view factory
├── Validation       # FluentValidation validators
├── Finance          # Amortization (PMT + schedule) calculator
├── Middleware       # Global error handler
└── Common           # Exceptions, clock, JWT options, helpers
tests/LoanHub.Tests  # xUnit tests
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — check with `dotnet --version`.

## Run it

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project src/LoanHub.Api
```

Open Swagger at **http://localhost:5099/swagger** (the root `/` redirects there).

## Authentication

Endpoints are protected with JWT. Get a token first:

`POST /api/auth/login`
```json
{ "username": "AniJabakhidze", "password": "Password1!" }
```

Copy the `token`, click **Authorize** in Swagger, and paste it. Demo credentials and the signing key
live in `appsettings.json` under `JwtConfig` (change them for real use).

> Demo credentials: **AniJabakhidze** / **Password1!**

## Endpoints

| Method | Route                                   | Description               | Auth |
|--------|-----------------------------------------|---------------------------|------|
| POST   | `/api/auth/login`                       | Get a JWT token           | No   |
| POST   | `/api/customers`                        | Create a customer         | Yes  |
| GET    | `/api/customers/{id}`                   | Get a customer            | Yes  |
| GET    | `/api/customers/loans?customerId={id}`  | Customer loan history     | Yes  |
| POST   | `/api/Loans/CreateApplication`          | Submit a loan application | Yes  |
| GET    | `/api/loans/{id}`                       | Get loan status/details   | Yes  |
| POST   | `/api/payments`                         | Make a payment            | Yes  |

Two sample customers are seeded on first run (ids `1` and `2`).

## Business rules

- **Customer:** names required, personal number unique, must be at least 18.
- **Loan:** amount 500–50,000, term 6–60 months, customer must exist; PMT is computed automatically;
  credit score `< 300` is rejected, otherwise approved (and a schedule is generated).
- **Payment:** amount `> 0`, loan must exist, not allowed on Closed/Rejected/Pending loans. After a
  payment the loan becomes Closed (fully paid), Overdue (behind the due amount), or stays Approved.

## Tests

```bash
dotnet test
```
