
# VNG Assignment

## Exercise 2 - Microservice

### Overview

This project demonstrates a microservices architecture using .NET technologies. It consists of several components including an API Gateway, multiple services, and a database, all deployed locally using Docker.

I am planning to build architect like this:

<img src="[https://api.nuget.org/v3-flatcontainer/ocelot/23.3.3/icon](https://i.ytimg.com/vi/0Mzft2Kcev0/maxresdefault.jpg)" alt="Ocelot API Gateway" width="500"/>

### Components

- **API Gateway**: Ocelot  
<img src="https://api.nuget.org/v3-flatcontainer/ocelot/23.3.3/icon" alt="Ocelot API Gateway" width="50"/>

- **Database**: Postgres  
  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/29/Postgresql_elephant.svg/640px-Postgresql_elephant.svg.png" alt="Ocelot API Gateway" width="50"/>

- **Communication**: gRPC  
  <img src="https://blog.kakaocdn.net/dn/bi6vYk/btqDSAPIWKU/AsFL9mx7ttSwBEqLX6Sgo0/img.png" alt="Ocelot API Gateway" width="50"/>

- **Architecture**: Clean Architecture  

- **Design Patterns**: CQRS, Mediator, Dependency Injection (DI)  

- **Unit Testing**: Moq, AutoFixture, xUnit  

- **Local Deployment**: Docker  
    <img src="https://cloud.z.com/vn/wp-content/uploads/2023/02/image1-15.png" alt="Ocelot API Gateway" width="50"/>

### Databases

This project utilizes two PostgreSQL databases:

1. **CartServiceDB**: Used by the Cart service, running on port `5433`.
2. **ProductDB**: Used by the Product service, running on port `5432`.

### Applying Migrations

Before running the project, apply migrations to set up the databases. Use the following commands in the Package Manager Console:

```shell
update-database -s CartService.API -p CartService.Persistence
update-database -s ProductService.API -p ProductService.Persistence
```

### Running the Project

#### Using IDE (HTTPS)

If you are running the project using an IDE, the services will be available on the following ports:

- **API Gateway**: 5000
- **Product API**: 5004
- **Cart API**: 5002

#### Using Docker (HTTP)

When deploying using Docker, the services will run on the following ports:

- **API Gateway**: 5432
- **Product API**: 5003
- **Cart API**: 5001

**Important**: When running on Docker, the services are configured to use HTTP only. Ensure to configure the appropriate ports in the Docker configuration to enable communication between services.

**Configuration Note**: The Docker configuration files will need to be updated to point each service to the correct ports.

![Docker Configuration Example](./images/docker-configuration-example.png)
