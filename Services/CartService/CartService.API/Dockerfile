#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5001
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/CartService/CartService.API/CartService.API.csproj", "Services/CartService/CartService.API/"]
COPY ["Services/CartService/CartService.Application/CartService.Application.csproj", "Services/CartService/CartService.Application/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]
COPY ["Services/CartService/CartService.Domain/CartService.Domain.csproj", "Services/CartService/CartService.Domain/"]
COPY ["Services/CartService/CartService.Infrastructure/CartService.Infrastructure.csproj", "Services/CartService/CartService.Infrastructure/"]
COPY ["Services/CartService/CartService.Persistence/CartService.Persistence.csproj", "Services/CartService/CartService.Persistence/"]
RUN dotnet restore "./Services/CartService/CartService.API/./CartService.API.csproj"
COPY . .
WORKDIR "/src/Services/CartService/CartService.API"
RUN dotnet build "./CartService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CartService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartService.API.dll"]