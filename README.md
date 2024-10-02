# Overview

This project demonstrates a microservices architecture using .NET technologies. It consists of several components including an API Gateway, multiple services, and a database, all deployed locally using Docker.

I am planning to build architect like this:

<img src="https://i.ytimg.com/vi/0Mzft2Kcev0/maxresdefault.jpg" alt="Ocelot API Gateway"/>

# Components:

## Structure:
#### API Gateway:
- [x] Ocelot
- [ ] Load Balancing
- [ ] Rate limiting
- [ ] Service Discovery (Consul)

#### Authentication, Authorization:
- [x] JWT

#### Communication:
- ##### Synchronous:
    - [x] gRPC
    - [ ] HTTP
- ##### Asynchronous:
    - [ ] Event Message (RabbitMQ/Kafka)

#### Architecture (each services): 
- [x] Clean Architecture  

#### Validations: 
- [x] FluentValidation

#### Design Patterns: 
- [x] CQRS
- [x] Mediator
- [x] Dependency Injection (DI)
- [x] Generic Repository, Unit of Work
- [x] Options

#### Unit Testing: 
- [x] Moq
- [x] AutoFixture
- [x] xUnit

#### Integration Testing: 
- **Planning ...**

#### Caching:
- [x] Redis

#### Monitoring:
- [ ] OpenTelemetry
- ##### Logging:
    - [ ] ElasitcSearch, Kibana, Serilog
    - [ ] Grafana, Loki
    - [ ] Log collectors (FluentD/(Logstash, FileBeat))

- ##### Tracing:
    - [ ] Jeager/Zipkin

- ##### Metrics:
    - [ ] Prometheus (Alert, ...)

#### Local Deployment:
- [x] Docker
- [ ] Docker Swarm / k8s

#### Database & ORM:
- ##### Auth Service:
    - [x] Postgres (EF Core)
- ##### Cart Service:
    - [x] Postgres (EF Core)
- ##### Product Service:
    - [x] Postgres (EF Core)
    - [ ] MongoDB (Dapper)

## Services:
- [x] AuthService (Identity Server 4)
- [x] CartService
- [x] ProductService
- [ ] NotificationService

## Technique:
- ##### Distributed Lock:
    - [x] RedLock (Redis)
- ##### Distributed transaction:
    - [ ] 2PC/3PC
    - **Saga Pattern**:
        - [ ] Orchestration
        - [ ] Choreography
    - [ ] Outbox Pattern
    - [ ] Inbox Pattern
- ##### Resilience (Polly):
    - [ ] Circuit Breaker
    - [ ] Bulkhead Pattern
    - [ ] Retry Strategy
# Usage
### Applying Migrations

Before running the project, apply migrations to set up the databases. Use the following commands in the Package Manager Console:

```shell
update-database -s CartService.API -p CartService.Persistence
update-database -s ProductService.API -p ProductService.Persistence
```
