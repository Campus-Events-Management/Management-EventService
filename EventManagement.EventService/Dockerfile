FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5075

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EventManagement.EventService.csproj", "./"]
RUN dotnet restore "EventManagement.EventService.csproj"
COPY . .
RUN dotnet build "EventManagement.EventService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventManagement.EventService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/wwwroot/event-images && chmod 777 /app/wwwroot/event-images

ENTRYPOINT ["dotnet", "EventManagement.EventService.dll"] 