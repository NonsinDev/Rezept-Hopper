# Build Stage (Nutzt das stabile .NET 9 SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["Rezept Hopper/Rezept Hopper.csproj", "Rezept Hopper/"]
RUN dotnet restore "Rezept Hopper/Rezept Hopper.csproj"

# Copy everything else and build
COPY . .

# Wir bleiben im Root (/src) und bauen mit absoluten Pfaden
RUN dotnet publish "Rezept Hopper/Rezept Hopper.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage (Nutzt die stabile .NET 9 Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create directory for SQLite database
RUN mkdir -p /app/data

COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/rezepthopper.db"

EXPOSE 8080

ENTRYPOINT ["dotnet", "Rezept_Hopper.dll"]
