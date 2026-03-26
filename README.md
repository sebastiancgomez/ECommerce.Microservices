# 🛒 ECommerce Microservices (.NET)

![.NET](https://img.shields.io/badge/.NET-8-blue)
![Architecture](https://img.shields.io/badge/architecture-microservices-green)
![Database](https://img.shields.io/badge/database-sqlserver-red)
![Docker](https://img.shields.io/badge/container-docker-blue)

Backend architecture for an **e-commerce platform built with .NET 8** using a **microservices-based design**.

This project demonstrates modern backend architecture practices including:

* Microservices architecture
* Domain modeling
* Service-to-service communication
* Independent databases per service
* Clean domain entities
* Dockerized infrastructure
* REST APIs with ASP.NET Core
* Scalable distributed system design

The system models a simplified e-commerce flow where customers can place orders, inventory is validated, pricing rules are applied, and services collaborate to complete the order lifecycle.

---

# 🚀 Tech Stack

### Backend

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* C#

### API

* REST
* OpenAPI / Swagger

### Database

* SQL Server
* EF Core Migrations

### Infrastructure

* Docker
* Docker Compose *(planned)*

### Architecture

* Microservices
* Domain modeling
* Service clients (HTTP communication)

---

# ✅ Implemented Features

The following capabilities are currently implemented and functional.

### Product Service

* Create product
* Retrieve product by id
* Update product
* Delete product
* DTO mapping
* Entity validation

### Inventory Service

* Create inventory item
* Retrieve inventory by product
* Reserve stock
* Release stock
* Stock validation
* Inventory constraints via migrations
* Service layer abstraction
* Repository pattern

### Pricing Service

* Create pricing rules
* Retrieve pricing rules by product
* Rule evaluation engine
* Dynamic price calculation
* Support for pricing rule conditions:

  * Minimum quantity
  * Percentage discount
  * Fixed discount
  * Validity period (start / end date)

---

# 🏗 System Architecture

The platform is composed of **multiple independent microservices**, each responsible for a specific business capability.

Each microservice has:

* Its own **database**
* Its own **REST API**
* Independent **deployment**
* A clear **bounded context**

Services communicate through **HTTP APIs**.

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

InventoryService --> InventoryDB[(InventoryDB)]
PricingService --> PricingDB[(PricingDB)]
ProductService --> ProductDB[(ProductDB)]
OrderService --> OrderDB[(OrderDB)]
CustomerService --> CustomerDB[(CustomerDB)]
NotificationService --> NotificationDB[(NotificationDB)]
```

---

# ☁️ Microservices

| Service             | Responsibility                           | Status         |
| ------------------- | ---------------------------------------- | -------------- |
| ProductService      | Product catalog management               | ✅ Implemented  |
| OrderService        | Order creation and lifecycle             | 🚧 In progress |
| CustomerService     | Customer management                      | 💤 Planned     |
| NotificationService | Notifications and messaging              | 💤 Planned     |
| InventoryService    | Product stock validation and reservation | ✅ Implemented  |
| PricingService      | Pricing rules and price calculation      | ✅ Implemented  |
| PaymentService      | Payment processing                       | 💤 Planned     |
| API Gateway         | Single entry point for clients           | 💤 Planned     |

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

docker
 └─ sqlserver
```

Typical structure inside each service:

```
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

| Service             | HTTP | HTTPS | Database       |
| ------------------- | ---- | ----- | -------------- |
| ProductService      | 5100 | 7100  | ProductDb      |
| OrderService        | 5200 | 7200  | OrderDb        |
| CustomerService     | 5300 | 7300  | CustomerDb     |
| NotificationService | 5400 | 7400  | NotificationDb |
| PaymentService      | 5500 | 7500  | PaymentDb      |
| InventoryService    | 5600 | 7600  | InventoryDb    |
| PricingService      | 5700 | 7700  | PricingDb      |
| API Gateway         | 5000 | 7000  | —              |

All services use **SQL Server running in Docker (port 1433)**.

---

# 🐳 Running the Project

## 1. Start SQL Server with Docker

```
docker run -e "ACCEPT_EULA=Y" \
-e "SA_PASSWORD=YourPassword123!" \
-p 1433:1433 \
--name ecommerce-sql \
-d mcr.microsoft.com/mssql/server:2022-latest
```

---

## 2. Run database migrations

Inside each service:

```
dotnet ef database update
```

---

## 3. Run a service

Example:

```
cd services/ProductService
dotnet run
```

Swagger will be available at:

```
http://localhost:<port>/swagger
```

---

# 🔄 Order Creation Sequence

This sequence diagram illustrates how services collaborate during the order creation process.

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

# 📡 Service Communication Contracts

Inter-service communication is based on **REST APIs with DTO contracts**.

### Product Service

```
GET /products/{id}
```

Response example:

```json
{
  "id": 10,
  "name": "Laptop",
  "price": 1200.00
}
```

---

### Pricing Service

```
GET /pricing/{productId}
```

Response example:

```json
{
  "productId": 10,
  "basePrice": 1200.00,
  "finalPrice": 1150.00
}
```

---

### Inventory Service

```
POST /inventory/reserve
```

Request example:

```json
{
  "productId": 10,
  "quantity": 2
}
```

Response example:

```json
{
  "status": "Reserved"
}
```

---

# 📚 Domain Concepts

## Order Lifecycle

Orders follow a status workflow:

```
Created
Confirmed
PaymentProcessing
Paid
Cancelled
Expired
```

Orders maintain a **status history** to track transitions between states.

Example domain entities:

```
Order
OrderItem
OrderStatus
OrderStatusHistory
```

The domain model ensures business rules such as:

* Quantity validation
* Item aggregation
* Order total calculation
* Status transition tracking

---

# 🧠 Concepts Demonstrated

* Microservices architecture
* Domain-driven modeling
* Service communication via HTTP clients
* Independent databases per service
* Clean entity design
* Repository and service layers
* Containerized infrastructure
* Scalable backend architecture

---

# 📈 Development Status

Current implemented services:

* Product catalog service
* Inventory reservation system
* Pricing rule engine

Next milestones:

* Order orchestration service
* API Gateway
* Event-driven communication
* RabbitMQ integration
* Kubernetes deployment

---

# 🚧 Future Improvements

Planned improvements include:

* API Gateway (YARP / Ocelot)
* Event-driven architecture
* Message broker (RabbitMQ / Kafka)
* Distributed transaction patterns (Saga)
* Observability with OpenTelemetry
* Authentication with JWT / Identity
* Container orchestration with Kubernetes
* Centralized configuration
* Resilience patterns (retry / circuit breaker)

---

# 👨‍💻 Author

**Juan Sebastián Cárdenas Gómez**

Backend Engineer specialized in:

* .NET
* Java
* Microservices
* Cloud architecture
* Distributed systems

This project was built as part of **backend architecture practice and cloud-native experimentation**.

🔗 GitHub
https://github.com/sebastiancgomez

🔗 LinkedIn
https://linkedin.com/in/sebastiancgomez
