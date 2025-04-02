# Appointment Scheduling System
## Overview
This is a secure, role-based appointment scheduling API built with .NET. It features JWT authentication, role-based authorization, and comprehensive appointment management capabilities. 
The system is designed for healthcare providers, salons, or any business requiring appointment scheduling with different access levels for clients and admins.
## Table of contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [API Documentation](#api-documentation)
  - [Authentication](#authentication)
  - [Appointments](#appointments)
  - [Payments](#payments)
  - [Reviews](#reviews)
- [Usage Examples](#usage-examples)
- [Security](#security)

## Features
✅ **Role-Based Access Control**  
- Client, and Admin roles with granular permissions  
- Separate controllers for read vs. write operations  

🔒 **Security**  
- JWT authentication with 1-hour expiration  
- BCrypt password hashing  
- Secure admin registration flow  

📅 **Appointment Management**  
- Conflict detection for double bookings  
- Flexible scheduling system  

💳 **Payment Integration**  
- Payment record creation  
- Transaction history  

⭐ **Review System**  
- Client feedback collection  
- Rating system (1-5 stars)  
- Role-based access control (Client, Admin)
- JWT authentication with 1-hour token expiration
- Secure password handling with BCrypt hashing
- Appointment management with conflict detection
- Payment processing integration
- Review system for client feedback
- Swagger documentation for easy API exploration

## Tech Stack
- .NET 7
- Entity Framework Core
- JWT Bearer Authentication
- BCrypt.Net-Next
- Swagger/OpenAPI
- SQL Server (or your preferred database)

## Getting Started
### Prerequisites
- [.NET 7 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) 
- [Git](https://git-scm.com/)

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/NMHelmy/Appointment-Scheduling-System.git
   cd appointment-scheduling
   ```
2. Confiure the database
   ```
   // Update connection string in appsettings.json
   "ConnectionStrings": {
    "DefaultConnection": "Server=localhost; Database=YourDatabaseName; Trusted_Connection=true;TrustServerCertificate=true"
    },
    ```
3. Apply migrations
   ```
   dotnet ef migrations add MigrationName
   dotnet ef database update
   ```
4. Run the API
   ```
   dotnet run
   ```
5. Access Swagger UI at:
   ```
   https://localhost:5001/swagger
   ```

## API Documentation

### Authentication

| Endpoint                | Method | Description                          | Access  | Request Body Example                          | Success Response Example                     |
|-------------------------|--------|--------------------------------------|---------|-----------------------------------------------|----------------------------------------------|
| `/api/auth/register`    | POST   | Register new client account          | Client/Admin   | `{"firstName":"John","lastName":"Doe","email":"john@example.com","password":"Pass@1234"}` | `{"userId":1,"email":"john@example.com","role":"Client","token":"eyJhb..."}` |
| `/api/auth/register-admin` | POST | Register new admin account (protected) | Admin   | Same as register                              | Same as register with `"role":"Admin"`       |
| `/api/auth/login`       | POST   | Authenticate user                    | Client/Admin   | `{"email":"john@example.com","password":"Pass@1234"}` | Same as register response                   |

### Appointments

| Endpoint                     | Method | Description                          | Access      | Request Body Example                          | Success Response                          |
|------------------------------|--------|--------------------------------------|-------------|-----------------------------------------------|-------------------------------------------|
| `/api/AppointmentReadOnly`   | GET    | Get user's appointments              | Client/Admin    | -                                             | `[{"id":1,"startTime":"2023-07-20T09:00:00","service":"Haircut"}]` |
| `/api/AppointmentAdmin`      | POST   | Create new appointment               | Admin | `{"clientId":1,"serviceId":3,"startTime":"2023-07-20T09:00:00","notes":"Regular checkup"}` | `201 Created` with location header       |

### Payments

| Endpoint               | Method | Description                          | Access      | Request Body Example                          | Success Response                          |
|------------------------|--------|--------------------------------------|-------------|-----------------------------------------------|-------------------------------------------|
| `/api/PaymentReadOnly` | GET    | Get user's payment history           | Client/Admin       | -                                             | `[{"id":1,"amount":50.00,"status":"Paid","date":"2023-07-15"}]` |
| `/api/PaymentAdmin`    | POST   | Create payment record                | Admin | `{"appointmentId":1,"amount":50.00,"method":"CreditCard","transactionId":"txn_12345"}` | `201 Created` with payment details       |

### Reviews

| Endpoint             | Method | Description                          | Access  | Request Body Example                          | Success Response                          |
|----------------------|--------|--------------------------------------|---------|-----------------------------------------------|-------------------------------------------|
| `/api/ReviewReadOnly`| GET    | Get all reviews                      | Client/Admin   | -                                             | `[{"id":1,"rating":5,"comment":"Great service!","user":"John D."}]` |
| `/api/ReviewAdmin`   | POST   | Create new review                    | Admin  | `{"appointmentId":1,"rating":5,"comment":"Excellent service!"}` | `201 Created` with review details        |

## Usage Ecamples
### Client Registration
```
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "password": "SecurePass123!"
}
```
### Create Appointment (Admin)
```
POST /api/AppointmentAdmin
Authorization: Bearer your.jwt.token
Content-Type: application/json

{
  "clientId": 1,
  "serviceId": 2,
  "startTime": "2023-07-20T14:00:00",
  "notes": "Regular checkup"
}
```
## Security
```
// appsettings.json
"JwtSettings": {
  "SecretKey": "minimum-32-character-secret-key-here",
  "ValidIssuer": "yourdomain.com",
  "ValidAudience": "yourdomain.com",
  "ExpiryInHours": 1
}
```
