#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EventProducer/src/EventProducer.Web/EventProducer.Web.csproj", "EventProducer/src/EventProducer.Web/"]
COPY ["Common/src/Common.Events.Brokers.RabbitMq/Common.Events.Brokers.RabbitMq.csproj", "Common/src/Common.Events.Brokers.RabbitMq/"]
COPY ["Common/src/Common.Events/Common.Events.csproj", "Common/src/Common.Events/"]
RUN dotnet restore "EventProducer/src/EventProducer.Web/EventProducer.Web.csproj"
COPY . .
WORKDIR "/src/EventProducer/src/EventProducer.Web"
RUN dotnet build "EventProducer.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventProducer.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventProducer.Web.dll"]