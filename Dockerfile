FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/GroceryAPI/GroceryAPI.csproj", "GroceryAPI/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies
RUN dotnet restore "GroceryAPI/GroceryAPI.csproj"

# Copy all source code
COPY src/ .

# Build
WORKDIR "/src/GroceryAPI"
RUN dotnet build "GroceryAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroceryAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroceryAPI.dll"]