#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebhookManager/src/WebhookManager.Web/WebhookManager.Web.csproj", "WebhookManager/src/WebhookManager.Web/"]
COPY ["WebhookManager/src/WebhookManager.Infrastructure/WebhookManager.Infrastructure.csproj", "WebhookManager/src/WebhookManager.Infrastructure/"]
COPY ["Common/src/Common.Events.Brokers.RabbitMq/Common.Events.Brokers.RabbitMq.csproj", "Common/src/Common.Events.Brokers.RabbitMq/"]
COPY ["Common/src/Common.Events/Common.Events.csproj", "Common/src/Common.Events/"]
COPY ["WebhookManager/src/WebhookManager.Application/WebhookManager.Application.csproj", "WebhookManager/src/WebhookManager.Application/"]
COPY ["WebhookManager/src/WebhookManager.Domain/WebhookManager.Domain.csproj", "WebhookManager/src/WebhookManager.Domain/"]
RUN dotnet restore "WebhookManager/src/WebhookManager.Web/WebhookManager.Web.csproj"
COPY . .
WORKDIR "/src/WebhookManager/src/WebhookManager.Web"
RUN dotnet build "WebhookManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebhookManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebhookManager.Web.dll"]