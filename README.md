## Quick Start

### Prerequisites

- Docker and Docker Compose
- .NET 9 SDK (for local development)

### Environment Configuration

Copy the environment template and configure your settings:

```bash
cp .env.example .env
```

### Running with Docker

Start all services:

```bash
docker-compose up --build
```

## Database Migrations

The system uses Entity Framework Core migrations. To create and apply migrations:

```bash
# Create migration
dotnet ef migrations add MigrationName --project src/Services/Account/Account.Infrastructure --startup-project src/Services/Account/Account.Api

# Apply migration
dotnet ef database update --project src/Services/Account/Account.Infrastructure --startup-project src/Services/Account/Account.Api
```

## Monitoring

### RabbitMQ Management Interface

Access the RabbitMQ management console at `http://localhost:15672`

- Username: `sofka`
- Password: `sofka`

## Technology Stack

- **.NET 9**: Primary development framework
- **Entity Framework Core**: ORM and database migrations
- **MassTransit**: Messaging abstraction layer
- **RabbitMQ**: Message broker
- **SQL Server**: Database engine
- **Docker**: Containerization platform
- **Swagger/OpenAPI**: API documentation

### Core Components

- **API Gateway**: Entry point for HTTP requests, converts them to messages
- **Customers Service**: Customer management with complete CRUD operations
- **Accounts Service**: Account and transaction management
- **RabbitMQ**: Message broker implementing Topic Exchange pattern
- **SQL Server**: Relational database with database-per-service pattern

## Project Structure

```
src/
├── ApiGateway/           # HTTP entry point
├── Services/
│   ├── Account/          # Account management service
│   │   ├── Account.Api/
│   │   ├── Account.Application/
│   │   ├── Account.Domain/
│   │   └── Account.Infrastructure/
│   └── Customers/        # Customer management service
│       ├── Customers.Api/
│       ├── Customers.Application/
│       ├── Customers.Domain/
│       └── Customers.Infrastructure/
└── Shared/
    └── SofkaFinance.Contracts/  # Shared message contracts
```
