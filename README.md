# 🛒 ECommerce Microservices (.NET)

![.NET](https://img.shields.io/badge/.NET-8-blue)
![Architecture](https://img.shields.io/badge/architecture-microservices-green)
![Database](https://img.shields.io/badge/database-sqlserver-red)
![Docker](https://img.shields.io/badge/container-docker-blue)
![Resilience](https://img.shields.io/badge/resilience-polly-orange)

Backend architecture for an **e-commerce platform built with .NET 8** using a **microservices-based design**.

This project demonstrates modern backend architecture practices including:

- Microservices architecture
- Domain modeling
- Service-to-service communication with resilience patterns
- Independent databases per service
- Clean domain entities
- Dockerized infrastructure with health checks
- REST APIs with ASP.NET Core
- Scalable distributed system design

The system models a simplified e-commerce flow where customers can place orders, inventory is validated, pricing rules are applied, and services collaborate to complete the order lifecycle.

---

# 🚀 Tech Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- C#

### Resilience
- Polly (via `Microsoft.Extensions.Http.Resilience`)
- Retry pattern
- Circuit Breaker pattern

### API
- REST
- OpenAPI / Swagger

### Database
- SQL Server
- EF Core Migrations (applied automatically on startup)

### Infrastructure
- Docker
- Docker Compose
- Health Checks (per service)

### Architecture
- Microservices
- Domain modeling
- Service clients (HTTP communication)
- Interface-based HTTP clients

---

# ✅ Implemented Features

### Product Service
- Create product
- Retrieve product by id
- Update product
- Delete product
- DTO mapping
- Entity validation

### Inventory Service
- Create inventory item (validates product exists in ProductService)
- Retrieve inventory by product
- Reserve stock
- Release stock
- Stock validation
- Inventory constraints via migrations
- Service layer abstraction
- Repository pattern

### Pricing Service
- Create pricing rules (validates product exists in ProductService)
- Retrieve pricing rules by product
- Rule evaluation engine
- Dynamic price calculation
- Support for pricing rule conditions:
  - Minimum quantity
  - Percentage discount
  - Fixed discount
  - Validity period (start / end date)

### Order Service
- Create order
- Integration with ProductService, InventoryService and PricingService
- Resilient HTTP clients with retry and circuit breaker (Polly)

### Infrastructure
- Health check endpoint (`/health`) on every service
- Docker Compose orchestration with `service_healthy` conditions
- Automatic EF Core migrations on startup
- Credentials managed via `.env` file (never committed)

---

# 🏗 System Architecture

The platform is composed of **multiple independent microservices**, each responsible for a specific business capability.

Each microservice has:
- Its own **database**
- Its own **REST API**
- Its own **health check endpoint**
- Independent **deployment**
- A clear **bounded context**

Services communicate through **resilient HTTP APIs** using Polly.

---

# 📊 Architecture Diagram
```mermaid
flowchart LR

Client --> APIGateway

APIGateway --> ProductService
APIGateway --> OrderService
APIGateway --> CustomerService

OrderService --> ProductService
OrderService --> PricingService
OrderService --> InventoryService

InventoryService --> ProductService
PricingService --> ProductService

InventoryService --> InventoryDB[(InventoryDB)]
PricingService --> PricingDB[(PricingDB)]
ProductService --> ProductDB[(ProductDB)]
OrderService --> OrderDB[(OrderDB)]
CustomerService --> CustomerDB[(CustomerDB)]
NotificationService --> NotificationDB[(NotificationDB)]
```

---

# ☁️ Microservices

| Service | Responsibility | Status |
|---|---|---|
| ProductService | Product catalog management | ✅ Implemented |
| OrderService | Order creation and lifecycle | 🚧 In progress |
| CustomerService | Customer management | 💤 Planned |
| NotificationService | Notifications and messaging | 💤 Planned |
| InventoryService | Product stock validation and reservation | ✅ Implemented |
| PricingService | Pricing rules and price calculation | ✅ Implemented |
| PaymentService | Payment processing | 💤 Planned |
| API Gateway | Single entry point for clients | 💤 Planned |

Each service is designed to evolve **independently**.

---

# 📦 Project Structure

```
services
 ├─ ProductService
 ├─ OrderService
 ├─ CustomerService
 ├─ NotificationService
 ├─ InventoryService
 ├─ PricingService
 └─ PaymentService (planned)
```

Typical structure inside each service:

```
Clients
Controllers
Models
DTOs
Repositories
Services
Data
Migrations
```

---

# 🔌 Service Ports

| Service | HTTP | Database |
|---|---|---|
| ProductService | 5100 | ProductDb |
| OrderService | 5200 | OrderDb |
| CustomerService | 5300 | CustomerDb |
| NotificationService | 5400 | NotificationDb |
| InventoryService | 5600 | InventoryDb |
| PricingService | 5700 | PricingDb |
| API Gateway | 5000 | — |

All services use **SQL Server running in Docker (port 1433)**.

---

# 🔐 Security & Configuration

Credentials and sensitive configuration are **never committed to the repository**.

All environment variables are managed via a `.env` file:

```bash
# Copy the example file and fill in your values
cp .env.example .env
```

The `.env.example` file is versioned as a reference template with placeholder values. The `.env` file is listed in `.gitignore`.

---

# 🐳 Running the Project

## Prerequisites
- Docker Desktop
- .NET 8 SDK (for local development without Docker)

## 1. Configure environment variables

```bash
cp .env.example .env
# Edit .env with your credentials
```

## 2. Start all services

```bash
docker compose up --build
```

Docker Compose will:
1. Start SQL Server and wait until it is **healthy**
2. Start ProductService and wait until it is **healthy**
3. Start InventoryService and PricingService (depend on ProductService being healthy)
4. Start OrderService once all dependencies are healthy

Swagger will be available at:
```
http://localhost:<port>/swagger
```

## 3. Health checks

Every service exposes a health endpoint:
```
GET http://localhost:<port>/health
```

## Useful commands

```bash
# View logs of a specific service
docker compose logs productservice

# Follow logs in real time
docker compose logs -f

# Check status of all containers
docker compose ps

# Stop all services
docker compose down

# Stop and remove volumes (resets databases)
docker compose down -v
```

---

# 🔄 Order Creation Sequence
```mermaid
sequenceDiagram

Client->>OrderService: POST /orders

OrderService->>ProductService: Get product information
ProductService-->>OrderService: Product details

OrderService->>PricingService: Calculate price
PricingService-->>OrderService: Final price

OrderService->>InventoryService: Validate stock
InventoryService-->>OrderService: Stock available

OrderService->>InventoryService: Reserve stock
InventoryService-->>OrderService: Reservation confirmed

OrderService->>OrderDB: Persist order

OrderService-->>Client: Order Created
```

---

# 🛡 Resilience Patterns

Inter-service HTTP communication is protected with **Polly** via `Microsoft.Extensions.Http.Resilience`.

Each HTTP client is configured with:

- **Retry** — up to 3 automatic retries with a 2-second delay between attempts
- **Circuit Breaker** — opens after 50% failure rate over a 30-second window (minimum 5 requests)

Example configuration:

```csharp
builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri("http://inventoryservice:8080");
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(2);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.MinimumThroughput = 5;
});
```

---

# 📡 Service Communication Contracts

### Product Service
```
GET /api/products/{id}
```

### Pricing Service
```
POST /api/pricing/calculate
```

### Inventory Service
```
GET  /api/inventory/{productId}
POST /api/inventory/reserve
POST /api/inventory/release
```

---

# 📚 Domain Concepts

## Order Lifecycle

```
Created → Confirmed → PaymentProcessing → Paid → Cancelled / Expired
```

Orders maintain a **status history** to track transitions between states.

---

# 🧠 Concepts Demonstrated

- Microservices architecture
- Domain-driven modeling
- Interface-based HTTP clients
- Resilient service communication (Polly — retry + circuit breaker)
- Health checks per service with Docker Compose orchestration
- Independent databases per service
- Clean entity design
- Repository and service layers
- Containerized infrastructure
- Credentials management with `.env`
- Automatic EF Core migrations on startup

---

# 🚧 Future Improvements

- API Gateway (YARP / Ocelot)
- Event-driven architecture
- Message broker (RabbitMQ / Kafka)
- Distributed transaction patterns (Saga)
- Observability with OpenTelemetry
- Structured logging (Serilog)
- Authentication with JWT / Identity
- Container orchestration with Kubernetes
- Centralized configuration

---

# 📈 Development Status

Implemented:
- Product catalog service
- Inventory reservation system
- Pricing rule engine
- Order integration with resilient HTTP clients
- Health checks and Docker Compose orchestration
- Secure credentials management

Next milestones:
- CustomerService and NotificationService
- Complete order lifecycle
- Structured logging
- API Gateway
- Event-driven communication with RabbitMQ

---

# 👨‍💻 Author

**Juan Sebastián Cárdenas Gómez**

Backend Engineer specialized in:
- .NET
- Java
- Microservices
- Cloud architecture
- Distributed systems

🔗 GitHub: https://github.com/sebastiancgomez  
🔗 LinkedIn: https://linkedin.com/in/sebastiancgomez
