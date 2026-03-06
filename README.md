```markdown
# ECommerce Microservices (.NET)

A microservices-based backend architecture for an e-commerce platform built with **.NET 8**, **ASP.NET Core**, **Entity Framework Core**, **Docker**, and **SQL Server**.

This project was created as part of a **backend architecture practice**, focusing on **microservices design, service communication, and scalable system structure**.

The system models a simplified e-commerce flow where customers can place orders, inventory is validated, pricing rules are applied, and services communicate to complete the order lifecycle.

---

# Architecture Overview

The platform is composed of multiple independent services, each responsible for a specific business capability.

Each microservice has:

- Its own **database**
- Its own **REST API**
- Independent **deployment**
- A clear **bounded context**

Services communicate using **HTTP APIs**.

Architecture (simplified):

Client  
│  
▼  
API Gateway (planned)  
│  
├───────────────┬───────────────┬───────────────  
│               │               │  
Product       Order         Customer  
Service       Service       Service  
                 │  
                 │  
           ┌───────────────┐  
           │               │  
       Inventory        Pricing  
        Service         Service  

---

# Microservices

| Service | Responsibility |
|------|------|
| **ProductService** | Product catalog management |
| **OrderService** | Order creation and lifecycle |
| **CustomerService** | Customer management |
| **NotificationService** | Notifications and messaging |
| **InventoryService** | Product stock management and reservations |
| **PricingService** | Product pricing rules and price calculation |
| **PaymentService** | Payment processing *(planned)* |
| **API Gateway** | Single entry point for clients *(planned)* |

---

# Technology Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **Docker**
- **Swagger / OpenAPI**

Architecture practices included:

- Domain modeling
- Microservices separation
- Independent databases
- Service-to-service communication
- DTO-based APIs
- Containerized infrastructure

---

# Project Structure

services/

- ProductService/
- OrderService/
- CustomerService/
- NotificationService/
- InventoryService/
- PricingService/
- PaymentService (planned)

docker/

- sqlserver/

Each service follows a typical layered structure:

- Controllers
- Models
- DTOs
- Data
- Services / Clients
- Migrations

---

# Service Ports

| Service | HTTP | HTTPS | Database |
|------|------|------|------|
| ProductService | 5100 | 7100 | ProductDb |
| OrderService | 5200 | 7200 | OrderDb |
| CustomerService | 5300 | 7300 | CustomerDb |
| NotificationService | 5400 | 7400 | NotificationDb |
| PaymentService | 5500 | 7500 | PaymentDb |
| InventoryService | 5600 | 7600 | InventoryDb |
| PricingService | 5700 | 7700 | PricingDb |
| API Gateway (planned) | 5000 | 7000 | — |

All services use **SQL Server (1433)** running in Docker.

---

# Running the Project

## 1. Start SQL Server with Docker

Example:

docker run -e "ACCEPT_EULA=Y" \
-e "SA_PASSWORD=YourPassword123!" \
-p 1433:1433 \
--name ecommerce-sql \
-d mcr.microsoft.com/mssql/server:2022-latest

---

## 2. Run Migrations

Inside each service:

dotnet ef database update

---

## 3. Run Services

Example:

cd ProductService  
dotnet run

Swagger will be available at:

http://localhost:<port>/swagger

---

# Example Flow: Create Order

Order creation coordinates multiple services.

1. Client sends a **CreateOrder** request to **OrderService**

2. OrderService calls:

- **ProductService** → retrieve product information
- **PricingService** → calculate final price
- **InventoryService** → validate stock and reserve items

3. Order is created and persisted.

4. Order status moves to **Confirmed**.

---

# Domain Concepts

## Order Lifecycle

Orders follow a status workflow:

- Created
- Confirmed
- PaymentProcessing
- Paid
- Cancelled
- Expired

Orders maintain a **status history** to track transitions between states.

---

# Goals of This Project

This repository is intended to demonstrate:

- Backend architecture design
- Microservices structure
- Service boundaries
- Domain modeling
- Infrastructure setup

It is **not intended to be a full production system**, but a **technical architecture showcase**.

---

# Future Improvements

Planned improvements include:

- API Gateway (YARP / Ocelot)
- Event-driven communication
- Message broker (RabbitMQ / Kafka)
- Distributed transaction patterns
- Observability (OpenTelemetry)
- Authentication (JWT / Identity)
- Container orchestration (Kubernetes)

---

# Author

Backend engineer specialized in:

- .NET
- Java
- Microservices
- Cloud architecture
- Distributed systems

This project was built as part of **backend architecture practice and cloud-native deployment experimentation**.
```
