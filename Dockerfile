# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project file and restore
COPY ["Rezept Hopper/Rezept Hopper.csproj", "Rezept Hopper/"]
RUN dotnet restore "Rezept Hopper/Rezept Hopper.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Rezept Hopper"
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
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
