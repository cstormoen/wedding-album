# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /App
COPY --from=build-env /App/out .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose ports
EXPOSE 8080

# Set entrypoint
ENTRYPOINT ["dotnet", "WeddingAlbum.dll"]
