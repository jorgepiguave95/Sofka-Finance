# 🏦 SofkaFinance - Microservicios con Comunicación Bidireccional

Sistema financiero distribuido implementando **microservicios con arquitectura de mensajería bidireccional**, donde cada servicio puede actuar como **Producer** y **Consumer** para comunicación inter-servicios.

## 🏗️ Arquitectura

### Patrón de Comunicación Bidireccional

```
┌─────────────┐    HTTP     ┌─────────────┐
│   Cliente   │────────────▶│  Gateway    │
└─────────────┘             │ (Producer)  │
                            └─────┬───────┘
                                  │ Messages
                                  ▼
                            ┌─────────────┐
                            │  RabbitMQ   │
                            │   Topics    │
                            └─────┬───────┘
                                  │ Messages
                     ┌────────────┼────────────┐
                     ▼            ▼            ▼
              ┌─────────────┐ ┌─────────────┐ ┌─────────────┐
              │ Customers   │ │  Accounts   │ │   Other     │
              │(P+C)        │ │   (P+C)     │ │ Services    │
              │             │ │             │ │   (P+C)     │
              └─────────────┘ └─────────────┘ └─────────────┘
                     ▲            │
                     │            ▼
                     └── Notifications ──┘
                    (Inter-service messaging)
```

**P+C = Producer + Consumer**: Cada microservicio puede enviar Y recibir mensajes.

### Servicios

- **🚪 API Gateway**: Punto de entrada HTTP, convierte requests a mensajes (Producer)
- **👥 Customers.Api**: Gestión de clientes (Producer + Consumer)
- **💰 Accounts.Api**: Gestión de cuentas y movimientos (Producer + Consumer)
- **🐰 RabbitMQ**: Broker de mensajes con Topic Exchanges
- **🗄️ SQL Server**: Base de datos relacional

## 🔄 Comunicación Inter-Servicios

### Ejemplo: Creación de Cuenta → Notificación a Cliente

1. **Gateway** recibe HTTP POST `/accounts`
2. **Gateway** envía `CreateAccountCommand` → **Accounts.Api**
3. **Accounts.Api** procesa y crea la cuenta
4. **🆕 Accounts.Api** envía `AccountCreatedNotification` → **Customers.Api**
5. **Customers.Api** recibe notificación y actualiza estado del cliente

### Routing Keys

```
# Gateway → Microservicios
account.create → Accounts.Api
customer.create → Customers.Api

# Inter-servicios (bidireccional)
customer.accountCreated → Customers.Api (desde Accounts.Api)
```

## 🚀 Ejecutar el Proyecto

### 1. Configurar Variables de Entorno

```bash
# Copiar archivo de ejemplo
cp .env.example .env

# Editar .env con tus configuraciones
```

### 2. Docker Compose

```bash
# Construir y ejecutar todos los servicios
docker-compose up --build

# Solo RabbitMQ y SQL Server (para desarrollo local)
docker-compose up rabbitmq sqlserver
```

### 3. Desarrollo Local

```bash
# Terminal 1: RabbitMQ
docker-compose up rabbitmq

# Terminal 2: Accounts.Api (solo para debugging)
cd src/Services/Account/Account.Api
dotnet run --urls "http://localhost:5001"

# Terminal 3: Customers.Api (solo para debugging)
cd src/Services/Customers/Customers.Api
dotnet run --urls "http://localhost:5002"

# Terminal 4: Gateway (único endpoint público)
cd src/ApiGateway
dotnet run --urls "http://localhost:3000"
```

**Nota**: En producción, solo el Gateway expone puerto HTTP. Los microservicios se comunican únicamente por mensajería.

## 🔧 Variables de Entorno

| Variable                 | Descripción                    | Valor por defecto |
| ------------------------ | ------------------------------ | ----------------- |
| `RABBITMQ_HOST`          | Host de RabbitMQ               | `localhost`       |
| `RABBITMQ_USER`          | Usuario RabbitMQ               | `sofka`           |
| `RABBITMQ_PASSWORD`      | Contraseña RabbitMQ            | `sofka`           |
| `GATEWAY_PORT`           | Puerto Gateway (único público) | `3000`            |
| `DATABASE_HOST`          | Host SQL Server                | `sqlserver`       |
| `DATABASE_NAME_ACCOUNT`  | Base de datos Account.Api      | `SofkaAccount`    |
| `DATABASE_NAME_CONSUMER` | Base de datos Customers.Api    | `SofkaConsumer`   |
| `MSSQL_SA_PASSWORD`      | Contraseña SA                  | Ver .env          |

**Arquitectura**:

- Solo el Gateway expone puerto HTTP público
- Microservicios se comunican únicamente por RabbitMQ
- **Database per Service**: Cada microservicio tiene su propia base de datos

## 📊 Endpoints

### Gateway (Puerto 3000 - Único Endpoint Público)

```http
# Customers
POST /customers - Crear cliente
GET /customers/{id} - Obtener cliente
PUT /customers/{id} - Actualizar cliente

# Accounts
POST /accounts - Crear cuenta
GET /accounts/{id} - Obtener cuenta
POST /accounts/{id}/deposit - Depositar
POST /accounts/{id}/withdraw - Retirar
```

### RabbitMQ Management (Puerto 15672)

- Usuario: `sofka`
- Contraseña: `sofka`
- URL: `http://localhost:15672`

## 🔍 Monitoreo

### Logs de Comunicación Bidireccional

```bash
# Ver logs de Account.Api enviando notificaciones
docker-compose logs accounts.api

# Ver logs de Customers.Api recibiendo notificaciones
docker-compose logs customers.api

# Ver mensajes en RabbitMQ
# Ir a http://localhost:15672 → Queues → Ver mensajes
```

## 🛠️ Tecnologías

- **.NET 9**: Framework principal
- **MassTransit**: Abstraction layer para mensajería
- **RabbitMQ**: Message broker con Topic Exchanges
- **SQL Server**: Base de datos
- **Docker & Docker Compose**: Containerización
- **Swagger/OpenAPI**: Documentación de APIs

## ✨ Beneficios de la Arquitectura Bidireccional

✅ **Desacoplamiento total** entre servicios  
✅ **Comunicación asíncrona** y resiliente  
✅ **Escalabilidad independiente** de cada servicio  
✅ **Notificaciones inter-servicios** para consistencia eventual  
✅ **Fácil extensión** para nuevos tipos de comunicación  
✅ **Observabilidad** completa del flujo de mensajes
