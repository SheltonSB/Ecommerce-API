Of course. Here is the revised version with the bold formatting removed from the text.

-----

# Enterprise E-commerce API

A production-grade RESTful API for an e-commerce platform, built with ASP.NET Core 9. This project is designed following Clean Architecture and Domain-Driven Design (DDD) principles to ensure a scalable, maintainable, and thoroughly testable codebase. The solution demonstrates advanced software engineering practices suitable for enterprise-level applications, focusing on separation of concerns, data integrity, and robust performance.

## Architectural Highlights

  * Clean Architecture: A decoupled and layered design (Domain, Application, Infrastructure, Presentation) promotes high cohesion, low coupling, and a clear separation of concerns.
  * Domain-Driven Design (DDD): Employs a rich domain model with encapsulated business logic, ensuring the core business rules are central to the application's design.
  * Comprehensive Data Management: Implements full CRUD (Create, Read, Update, Delete) functionality for core e-commerce entities, including products, categories, and sales.
  * Advanced Query Capabilities: Supports robust pagination, multi-field sorting, and dynamic filtering to efficiently handle large and complex datasets.
  * Data Integrity and Auditing: Utilizes soft deletes for data preservation and tracks price history for full audit compliance. Sales records are designed to be immutable once finalized to ensure financial accuracy.
  * Extensive Test Coverage: Includes a comprehensive suite of unit and integration tests to verify code reliability, correctness, and behavior.
  * API Documentation: Automatically generates OpenAPI (Swagger) documentation for clear, interactive API exploration and client generation.

## Technology Stack

| Component     | Technology               | Purpose |
|-----------    |------------              |---------|
| Framework     | ASP.NET Core 9           | Core web API and application framework |
| Database      |  Entity Framework Core 9 | ORM for database access (SQLite & SQL Server providers) |
| Logging       | Serilog                  | High-performance structured logging |
| Validation    | FluentValidation         | Declarative and robust input validation |
| Testing       | xUnit, FluentAssertions  | Unit and integration testing frameworks |
| Documentation | Swagger/OpenAPI          | Interactive API documentation and specification |

## Getting Started

### Prerequisites

  * .NET 9 SDK
  * Visual Studio 2022 or another compatible IDE/editor

### Installation and Execution

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/SheltonSB/ecommerce-api.git
    cd ecommerce-api
    ```

2.  **Restore NuGet packages:**

    ```bash
    dotnet restore
    ```

3.  **Run the application:**

    ```bash
    dotnet run --project Ecommerce.Api
    ```

4.  **Access the API Documentation:**
    Navigate to `https://localhost:7000` in a web browser. The application is configured to use an in-memory or SQLite database by default for ease of setup and testing.

## API Endpoints Overview

The API exposes a set of RESTful endpoints for managing e-commerce resources. The following is a summary of the primary endpoints.

#### Categories

  * `GET /api/categories` - Retrieve a paginated list of categories.
  * `POST /api/categories` - Create a new category.
  * `PUT /api/categories/{id}` - Update an existing category.

#### Products

  * `GET /api/products` - Retrieve a paginated list of products with filtering and sorting.
  * `GET /api/products/{id}` - Retrieve a single product by its ID.
  * `POST /api/products` - Create a new product.

#### Sales

  * `GET /api/sales` - Retrieve a paginated list of sales records.
  * `POST /api/sales` - Create a new sale record.
  * `POST /api/sales/{id}/complete` - Mark a sale as complete.

## Testing Strategy

The project maintains a high standard of quality through a multi-layered testing strategy. The test suite can be executed from the root directory with the following command:

```bash
dotnet test
```

The strategy includes:

  * Unit Tests: Validate individual components, services, and business logic in isolation.
  * Integration Tests: Verify the interactions between different layers of the application, including API controllers and database persistence.

## Contributing

Contributions to the project are welcome. Please fork the repository, create a dedicated feature branch, and submit a pull request for review.

## License

This project is licensed under the MIT License. See the `LICENSE` file for full details.