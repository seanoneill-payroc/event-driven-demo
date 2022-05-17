#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebhookManager/src/WebhookManager.Web/WebhookManager.Web.csproj", "WebhookManager/src/WebhookManager.Web/"]
RUN dotnet restore "WebhookManager/src/WebhookManager.Web/WebhookManager.Web.csproj"
COPY . .
WORKDIR "/src/WebhookManager/src/WebhookManager.Web"
RUN dotnet build "WebhookManager.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebhookManager.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebhookManager.Web.dll"]