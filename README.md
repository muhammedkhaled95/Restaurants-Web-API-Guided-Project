# ASP.NET Core 8 Web API - Clean Architecture & Azure
Guided Course https://www.udemy.com/course/aspnet-core-web-api-clean-architecture-azure By Jakub Kozera
## 📌 Overview
This project demonstrates a **scalable, maintainable, and high-performance RESTful API** built with **ASP.NET Core 8**, **Clean Architecture**, and **Azure services**. It follows best practices in **code organization, security, logging, and deployment** to deliver a robust API solution.

## 🔥 Key Features
- **Clean Architecture** for modular and maintainable code
- **Entity Framework Core & MS SQL** for database interaction
- **CQRS with MediatR** for command-query separation
- **Fluent Validation & DTO Mapping** for data integrity
- **Global Exception Handling** for better error management
- **Serilog Logging** for structured API event tracking
- **Automated API Documentation** for seamless client integration
- **User Authentication & Authorization** with ASP.NET Identity
- **Pagination & Sorting** for optimized performance
- **Unit & Integration Testing** for reliability
- **Azure Deployment & CI/CD Pipelines** for cloud scalability

## 🛠️ Tech Stack
- **Framework:** ASP.NET Core 8
- **Database:** MS SQL Server, Entity Framework Core
- **Architecture:** Clean Architecture, CQRS
- **Security:** ASP.NET Identity, JWT Authentication
- **Logging:** Serilog
- **Testing:** xUnit, Moq, Integration Testing
- **Deployment:** Azure App Service, Azure SQL, CI/CD Pipelines

## 🚀 Getting Started
### Prerequisites
- .NET 8 SDK
- MS SQL Server
- Azure Account (for cloud deployment)

### Setup
1. Clone the repository:
   ```sh
   git clone https://github.com/muhammedkhaled95/Restaurants-Web-API-Guided-Project.git
   ```
2. Update the `appsettings.json` with your database connection string.
3. Run database migrations:
   ```sh
   dotnet ef database update
   ```
4. Start the API:
   ```sh
   dotnet run
   ```
5. Access Swagger API documentation at `https://localhost:<port>/swagger`

## 🧪 Testing
Run unit and integration tests:
```sh
dotnet test
```

## ☁️ Deployment
### Azure Deployment Steps:
1. **Set up Azure App Service** and **Azure SQL**.
2. Configure **CI/CD pipelines** using GitHub Actions or Azure DevOps.
3. Deploy the application.

---
### 💡 Connect & Contribute
If you're passionate about **.NET, Clean Architecture, or Cloud Development**, feel free to connect, contribute, or discuss improvements!
