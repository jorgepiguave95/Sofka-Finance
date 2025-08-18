## Quick Start

### Environment Configuration

Copy the environment template and configure your settings:

```bash
cp .env.example .env
```

### Running with Docker

Start all microservices with Docker Compose:

```bash
# Navigate to the project root directory

# Start all services (builds images if needed)
docker compose up --build
```

This command will:

- **Build** all Docker images for the microservices
- **Start** the following containers:
  - `sofkafinance-gateway-1` - API Gateway (port 3000)
  - `sofkafinance-customers-1` - Customers Service
  - `sofkafinance-accounts-1` - Accounts Service
  - `sqlserver` - SQL Server Database (port 1433)
  - `rabbitmq` - RabbitMQ Message Broker (ports 5672, 15672)

#### Accessing the Application

Once all services are running, you can access:

- **Swagger UI (API Documentation)**: `http://localhost:3000` - Interactive API documentation in the root
- **API Gateway**: `http://localhost:3000/api/` - Main entry point for all API calls
- **RabbitMQ Management**: `http://localhost:15672` - Message broker monitoring
- **SQL Server**: `localhost,1433` - Database server

### Database Initialization

1. Ensure the SQL Server container is running:

   ```bash
   docker ps
   ```

2. Execute the initialization script:

   ```bash
   # Windows PowerShell
   sqlcmd -S localhost,1433 -U sa -P "a6Dd7GLHK2Zg4HVvSn" -i "database\queryInit.sql"

   # Linux/macOS
   sqlcmd -S localhost,1433 -U sa -P "a6Dd7GLHK2Zg4HVvSn" -i "database/queryInit.sql"
   ```

## Database Migrations

The system uses Entity Framework Core migrations. The connection strings are configured through environment variables defined in `.env`:

- **AccountSofka database**: `DB_NAME_ACCOUNT=AccountSofka`
- **CustomerSofka database**: `DB_NAME_CONSUMER=CustomerSofka`
- **SQL Server credentials**:
  - Host: `localhost,1433` (external) / `sqlserver` (internal)
  - User: `sa`
  - Password: `a6Dd7GLHK2Zg4HVvSn`

### Entity Framework Commands

To create and apply migrations for the Account service:

```bash
# Set connection string environment variable (if needed for local development)
$env:ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=AccountSofka;User Id=sa;Password=a6Dd7GLHK2Zg4HVvSn;TrustServerCertificate=True"

# Create migration
dotnet ef migrations add MigrationName --project src/Services/Account/Account.Infrastructure --startup-project src/Services/Account/Account.Api

# Apply migration to database
dotnet ef database update --project src/Services/Account/Account.Infrastructure --startup-project src/Services/Account/Account.Api
```

For the Customers service:

```bash
# Set connection string environment variable (if needed for local development)
$env:ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=CustomerSofka;User Id=sa;Password=a6Dd7GLHK2Zg4HVvSn;TrustServerCertificate=True"

# Create migration
dotnet ef migrations add MigrationName --project src/Services/Customers/Customers.Infrastructure --startup-project src/Services/Customers/Customers.Api

# Apply migration to database
dotnet ef database update --project src/Services/Customers/Customers.Infrastructure --startup-project src/Services/Customers/Customers.Api
```

## Monitoring

### RabbitMQ Management Interface

Access the RabbitMQ management console at `http://localhost:15672`

- Username: `sofka`
- Password: `sofka`

## Technology Stack

- **.NET 9**: Primary development framework
- **Entity Framework Core**: ORM and database migrations
- **RabbitMQ.Client**: Direct messaging implementation
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
