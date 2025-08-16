# SofkaFinance API Documentation

## Quick Setup

1. Import collection: Open Postman → Import → Select `SofkaFinance-Collection.json`
2. Start services: `docker-compose up --build -d`
3. Base URL: `http://localhost:3000`

**💡 Commands explained:**

- `docker-compose up --build -d` = Rebuild images + run in background
- `docker-compose up -d` = Start existing containers in background
- `docker-compose logs -f` = View real-time logs

**⚠️ Important:** All JSON property names must start with **uppercase** letters to match the C# DTOs.

## Authentication

### POST /api/auth/login

**Example 1:**

```json
{
  "email": "admin@test.com",
  "password": "123456"
}
```

**Example 2:**

```json
{
  "email": "usuario@sofka.com",
  "password": "mipassword123"
}
```

## Customer Management

### GET /api/clientes

**Example 1:**

```http
GET http://localhost:3000/api/clientes
```

**Example 2:**

```http
GET http://localhost:3000/api/clientes
Accept: application/json
```

### POST /api/clientes

**Example 1:**

```json
{
  "Name": "Test User",
  "Gender": "M",
  "Age": 30,
  "Identification": "12345678",
  "Address": "Test Address",
  "Phone": "123-456-7890",
  "Email": "test@test.com",
  "Password": "test123"
}
```

**Example 2:**

```json
{
  "Name": "Carlos Rodriguez",
  "Gender": "M",
  "Age": 35,
  "Identification": "87654321",
  "Address": "Calle 100 #11-22",
  "Phone": "300-987-6543",
  "Email": "carlos@empresa.com",
  "Password": "secure123"
}
```

## Account Management

### GET /api/cuentas

**Example 1:**

```http
GET http://localhost:3000/api/cuentas
```

**Example 2:**

```http
GET http://localhost:3000/api/cuentas
Accept: application/json
```

### GET /api/cuentas/{id}

**Example 1:**

```http
GET http://localhost:3000/api/cuentas/550e8400-e29b-41d4-a716-446655440000
```

**Example 2:**

```http
GET http://localhost:3000/api/cuentas/6ba7b810-9dad-11d1-80b4-00c04fd430c8
```

### POST /api/cuentas

**Example 1:**

```json
{
  "CustomerId": "550e8400-e29b-41d4-a716-446655440000",
  "AccountType": "Savings",
  "InitialBalance": 500000
}
```

**Example 2:**

```json
{
  "CustomerId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "AccountType": "Checking",
  "InitialBalance": 1000000
}
```

### DELETE /api/cuentas/{id}

**Example 1:**

```http
DELETE http://localhost:3000/api/cuentas/550e8400-e29b-41d4-a716-446655440000
```

**Example 2:**

```http
DELETE http://localhost:3000/api/cuentas/6ba7b810-9dad-11d1-80b4-00c04fd430c8
```

## Transaction Management

### GET /api/movimientos/cuenta/{id}

**Example 1:**

```http
GET http://localhost:3000/api/movimientos/cuenta/550e8400-e29b-41d4-a716-446655440000
```

**Example 2:**

```http
GET http://localhost:3000/api/movimientos/cuenta/6ba7b810-9dad-11d1-80b4-00c04fd430c8
```

### POST /api/movimientos/{id}/deposito

**Example 1:**

```json
{
  "Amount": 500000,
  "Concept": "Salary deposit"
}
```

**Example 2:**

```json
{
  "Amount": 150000,
  "Concept": "Cash deposit"
}
```

### POST /api/movimientos/{id}/retiro

**Example 1:**

```json
{
  "Amount": 200000,
  "Concept": "ATM withdrawal"
}
```

**Example 2:**

```json
{
  "Amount": 300000,
  "Concept": "Branch withdrawal"
}
```

### POST /api/movimientos/transferencia

**Example 1:**

```json
{
  "FromAccountId": "550e8400-e29b-41d4-a716-446655440000",
  "ToAccountId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "Amount": 250000,
  "Concept": "Transfer between accounts"
}
```

**Example 2:**

```json
{
  "FromAccountId": "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
  "ToAccountId": "123e4567-e89b-12d3-a456-426614174000",
  "Amount": 450000,
  "Concept": "Payment to third party"
}
```

### GET /api/movimientos/reportes

**⚠️ Note:** This endpoint has unusual design (GET with required body). Consider refactoring to POST.

**Example 1:**

```http
GET http://localhost:3000/api/movimientos/reportes?fechaInicio=2025-02-01&fechaFin=2025-02-28
Content-Type: application/json

{
  "IdCliente": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Example 2:**

```http
GET http://localhost:3000/api/movimientos/reportes?fechaInicio=2025-01-01&fechaFin=2025-03-31
Content-Type: application/json

{
  "IdCliente": "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
}
```

## Standard Response Format

All endpoints return **202 Accepted** with an asynchronous processing pattern:

```json
{
  "operation": "OperationName",
  "operationId": "generated-guid",
  "message": "Request accepted and will be processed asynchronously",
  "statusCode": 202
}
```
