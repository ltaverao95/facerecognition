# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Customer.NotificationService/Notification.Customer.NotificationService.csproj", "Customer.NotificationService/"]
COPY ["Messaging.EventBus/Messaging.Bus.csproj", "Messaging.EventBus/"]
COPY ["Messaging.InterfacesConstants/Messaging.Core.csproj", "Messaging.InterfacesConstants/"]
COPY ["Notification.Customer.NotificationService.Core/Notification.Customer.NotificationService.Core.csproj", "Notification.Customer.NotificationService.Core/"]
COPY ["Notifications.Customer.Email/Notification.Customer.EmailService.csproj", "Notifications.Customer.Email/"]
RUN dotnet restore "./Customer.NotificationService/Notification.Customer.NotificationService.csproj"
COPY . .
WORKDIR "/src/Customer.NotificationService"
RUN dotnet build "./Notification.Customer.NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notification.Customer.NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Customer.NotificationService.dll"]