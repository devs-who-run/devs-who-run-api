FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["devs-who-run-api/devs-who-run-api.csproj", "devs-who-run-api/"]
RUN dotnet restore "devs-who-run-api/devs-who-run-api.csproj"
COPY . .
WORKDIR "/src/devs-who-run-api"
RUN dotnet build "devs-who-run-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "devs-who-run-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "devs-who-run-api.dll"]
