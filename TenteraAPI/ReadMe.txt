# TenteraAPI - Clean Architecture Implementation

## Overview
TenteraAPI is a .NET 9.0 Web API project that follows Clean Architecture principles. This architecture promotes separation of concerns, maintainability, and testability by organizing the codebase into distinct layers.

## Project Structure
The solution is organized into the following layers:

### 1. Domain Layer
- Contains enterprise business rules and entities
- Independent of other layers
- Defines interfaces that other layers must implement
- Houses core business logic and domain models

### 2. Application Layer
- Contains business use cases
- Orchestrates the flow of data to and from entities
- Implements interfaces defined in the Domain layer
- Contains business rules and validation logic

### 3. Infrastructure Layer
- Implements interfaces defined in the Application layer
- Contains external concerns like:
  - Database access (Entity Framework Core)
  - External services integration
  - File system operations
  - Third-party services

### 4. Presentation Layer
- Contains API controllers and endpoints
- Handles HTTP requests and responses
- Implements API versioning and documentation
- Manages authentication and authorization

## Technology Stack
- .NET 9.0
- Entity Framework Core 9.0.5
- SQL Server
- BCrypt.Net-Next 4.0.3 (for password hashing)
- Twilio 7.11.1 (for SMS/communication services)
- OpenAPI/Swagger for API documentation

## Key Features
- Clean Architecture implementation
- RESTful API design
- Entity Framework Core for data access
- Secure password hashing with BCrypt
- SMS integration with Twilio
- API documentation with Swagger/OpenAPI

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server
- Visual Studio 2022 or later (recommended)

### Configuration
1. Update the connection string in `appsettings.json`
2. Configure any required API keys or secrets
3. Run database migrations (if applicable)

### Running the Application
1. Clone the repository
2. Restore NuGet packages
3. Build the solution
4. Run the application

## Architecture Benefits
- **Independence of Frameworks**: The core business logic is independent of any external frameworks
- **Testability**: Business rules can be tested without UI, database, or external elements
- **Independence of UI**: UI can be changed without changing the rest of the system
- **Independence of Database**: Business rules are not bound to the database
- **Independence of External Agency**: Business rules don't know about the outside world

## Best Practices
- Follow SOLID principles
- Implement proper exception handling
- Use dependency injection
- Write unit tests for business logic
- Document API endpoints
- Implement proper logging
- Follow security best practices

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
 MIT License.