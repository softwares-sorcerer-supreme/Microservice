# Overview

This project demonstrates a microservices architecture using .NET technologies. It consists of several components including an API Gateway, multiple services, and a database, all deployed locally using Docker.
This project is mainly focused on showcasing my technical skills, with less emphasis on the business side.

I am planning to build architecture like this, this image just illustration my project, not 100% exactly what I am building:

<img src="https://i.ytimg.com/vi/0Mzft2Kcev0/maxresdefault.jpg" alt="Ocelot API Gateway"/>

# Components:

## Structure:
#### API Gateway:
- [x] Ocelot
- [ ] YARP
    - [ ] Load Balancing
    - [x] Rate limiting
    - [ ] Service Discovery (Consul)

#### Authentication, Authorization:
- [x] JWT

#### Communication:
- ##### Synchronous:
    - [x] gRPC
    - [x] HTTP
- ##### Asynchronous:
    - [ ] Event Message (MassTransit)

#### Architecture (each service): 
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
    - [ ] Jaeger/Zipkin

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
    - [x] MongoDB (2 approach: EF Core and IMongoCollection)
- ##### Product Service:
    - [x] Postgres
    - [X] EF Core
    - [X] Dapper

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
    - [x] Circuit Breaker
    - [ ] Bulkhead Pattern
    - [x] Timeout/Retry Strategy
# Usage
### Applying Migrations

Before running the project, apply migrations to set up the databases. Use the following commands in the Package Manager Console:

```shell
update-database -s CartService.API -p CartService.Persistence
update-database -s ProductService.API -p ProductService.Persistence
```
