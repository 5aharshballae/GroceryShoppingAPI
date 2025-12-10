#!/bin/bash

echo "🛒 Setting up Grocery Shopping API..."

# Create solution
dotnet new sln -n GroceryShoppingAPI

# Add projects to solution
dotnet sln add src/Domain/Domain.csproj
dotnet sln add src/Application/Application.csproj
dotnet sln add src/Infrastructure/Infrastructure.csproj
dotnet sln add src/GroceryAPI/GroceryAPI.csproj
dotnet sln add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj

# Add project references
dotnet add src/Application/Application.csproj reference src/Domain/Domain.csproj
dotnet add src/Infrastructure/Infrastructure.csproj reference src/Application/Application.csproj
dotnet add src/Infrastructure/Infrastructure.csproj reference src/Domain/Domain.csproj
dotnet add src/GroceryAPI/GroceryAPI.csproj reference src/Application/Application.csproj
dotnet add src/GroceryAPI/GroceryAPI.csproj reference src/Domain/Domain.csproj
dotnet add src/GroceryAPI/GroceryAPI.csproj reference src/Infrastructure/Infrastructure.csproj
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj reference src/GroceryAPI/GroceryAPI.csproj
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj reference src/Application/Application.csproj
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj reference src/Domain/Domain.csproj
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj reference src/Infrastructure/Infrastructure.csproj

# Install packages for Application
dotnet add src/Application/Application.csproj package BCrypt.Net-Next --version 4.0.3

# Install packages for Infrastructure
dotnet add src/Infrastructure/Infrastructure.csproj package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add src/Infrastructure/Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add src/Infrastructure/Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add src/Infrastructure/Infrastructure.csproj package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add src/Infrastructure/Infrastructure.csproj package Microsoft.Extensions.Logging --version 8.0.0
dotnet add src/Infrastructure/Infrastructure.csproj package Microsoft.IdentityModel.Tokens --version 7.0.0
dotnet add src/Infrastructure/Infrastructure.csproj package System.IdentityModel.Tokens.Jwt --version 7.0.0

# Install packages for API
dotnet add src/GroceryAPI/GroceryAPI.csproj package BCrypt.Net-Next --version 4.0.3
dotnet add src/GroceryAPI/GroceryAPI.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add src/GroceryAPI/GroceryAPI.csproj package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add src/GroceryAPI/GroceryAPI.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add src/GroceryAPI/GroceryAPI.csproj package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add src/GroceryAPI/GroceryAPI.csproj package Swashbuckle.AspNetCore --version 6.5.0
dotnet add src/GroceryAPI/GroceryAPI.csproj package System.IdentityModel.Tokens.Jwt --version 7.0.0

# Install packages for Tests
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj package Microsoft.NET.Test.Sdk --version 17.8.0
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj package xunit --version 2.6.2
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj package xunit.runner.visualstudio --version 2.5.4
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj package Moq --version 4.20.70
dotnet add tests/GroceryAPI.Tests/GroceryAPI.Tests.csproj package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0

echo "✅ Project structure created!"
echo "📦 All packages installed!"
echo ""
echo "Next steps:"
echo "1. Copy all the code files from the artifacts into their respective locations"
echo "2. Run: docker-compose up -d postgres"
echo "3. Run: cd src/GroceryAPI && dotnet run"
echo "4. Open: http://localhost:5000"
