# Multi-stage Dockerfile with Redis

# Use the .NET 8.0 runtime image as the base for the final application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5006

# Use the .NET 8.0 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EPLSquadBackend/EPLSquadBackend.csproj", "EPLSquadBackend/"]
RUN dotnet restore "EPLSquadBackend/EPLSquadBackend.csproj"
COPY . .
WORKDIR "/src/EPLSquadBackend"
RUN dotnet build "EPLSquadBackend.csproj" -c Release -o /app/build

# Publish the application to a folder
FROM build AS publish
RUN dotnet publish "EPLSquadBackend.csproj" -c Release -o /app/publish

# Final image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install Redis in the final image
RUN apt-get update && apt-get install -y redis-server \
    && rm -rf /var/lib/apt/lists/*

# Copy Redis configuration (optional, if custom config is needed)
# COPY redis.conf /etc/redis/redis.conf

# Set ASP.NET Core URLs environment variable
ENV ASPNETCORE_URLS=http://+:5006

# Start Redis and the application when the container starts
CMD redis-server /etc/redis/redis.conf & dotnet EPLSquadBackend.dll