# Devs Who Run API

This is the API for the Devs Who Run application. It provides endpoints for managing members and their activities.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Running the API using Docker

1. Build the Docker image:
   ```sh
   docker build -t devs-who-run-api -f devs-who-run-api/Dockerfile .
   ```

2. Run the Docker container:
   ```sh
   docker run -p 8080:80 devs-who-run-api
   ```

3. The API will be accessible at `http://localhost:8080`.

## Running the API using `dotnet` CLI

1. Restore the dependencies:
   ```sh
   dotnet restore
   ```

2. Build the project:
   ```sh
   dotnet build
   ```

3. Run the project:
   ```sh
   dotnet run --project devs-who-run-api
   ```

4. The API will be accessible at `http://localhost:5299`.

## Configuring the API

The API can be configured using the `appsettings.json` and `appsettings.Development.json` files located in the `devs-who-run-api` directory. These files contain settings for logging, connection strings, and other configuration options.

## Accessing the API Endpoints

The following endpoints are available in the API:

- `GET /getPartnerConference`: Retrieves the list of partner conferences.
- `POST /addMember`: Adds a new member.
- `GET /getMemberByEmail/{email}`: Retrieves a member by their email.
- `GET /member/{id}`: Retrieves a member by their ID.

## Running Entity Framework Migrations

To apply the latest Entity Framework migrations, use the following commands:

1. Add a new migration:
   ```sh
   dotnet ef migrations add <MigrationName> --project devs-who-run-api
   ```

2. Update the database:
   ```sh
   dotnet ef database update --project devs-who-run-api
   ```
