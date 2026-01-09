# Gemini Code Assistant Context

This document provides context for the Gemini code assistant to understand the BookBazaar project.

## Project Overview

BookBazaar is a web application for an online bookstore. It consists of two main projects:

*   **BookBazaar**: An ASP.NET Core MVC web application that serves as the user-facing frontend. It handles user authentication, product display, cart management, and checkout.
*   **BookBazaarApi**: An ASP.NET Core Web API that provides the backend services for the BookBazaar web application. It manages the database, business logic, and data access for books, users, orders, and more.

The project uses a SQL Server database and Entity Framework Core for data access. The web application communicates with the API using a RESTful architecture.

## Building and Running

### Prerequisites

*   .NET 8 SDK
*   Visual Studio 2022 (or a compatible editor)
*   SQL Server (or SQL Server Express)

### Running the Project

1.  **Database Setup**:
    *   Open `BookBazaarApi/appsettings.json` and update the `dbconnection` connection string to point to your SQL Server instance.
    *   The application will automatically create the database and a default admin user on first run.

2.  **Running the API**:
    *   Open the solution in Visual Studio.
    *   Set `BookBazaarApi` as the startup project.
    *   Run the project (F5). The API will be available at the URL specified in `BookBazaarApi/Properties/launchSettings.json` (e.g., `https://localhost:44390`).

3.  **Running the Web App**:
    *   Open `BookBazaar/appsettings.json` and ensure the `BaseUrl` in the `ApiSettings` section points to the correct URL of the running API.
    *   Set `BookBazaar` as the startup project.
    *   Run the project (F5).

### TODO: Add Testing Information

There are no unit tests in the project. Information on running tests should be added here once they are available.

## Development Conventions

*   **Code Style**: The project follows standard C# and ASP.NET Core conventions.
*   **Authentication**: The web app uses cookie-based authentication. The API is stateless and authenticates requests based on information provided by the web app.
*   **Data Access**: The project uses the repository pattern for data access, with interfaces in `BookBazaarApi/Repos/Interfaces` and implementations in `BookBazaarApi/Repos/Classes`.
*   **Services**: Business logic is encapsulated in services, with interfaces in `BookBazaarApi/Services/Interfaces` and implementations in `BookBazaarApi/Services/Classes`.
*   **Database Migrations**: TODO: Add information on how to handle database migrations.