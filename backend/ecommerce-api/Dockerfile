# syntax=docker/dockerfile:1.7

# -------------------------------------
# Build stage
# -------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy ONLY solution and project files first for layer caching
COPY ["Ecommerce.slnx", "./"]

# Copy all .csproj fiels in their respective folders to allow cached restore
COPY ["src/Ecommerce.Api/Ecommerce.Api.csproj", "src/Ecommerce.Api/"]
COPY ["src/Ecommerce.Application/Ecommerce.Application.csproj", "src/Ecommerce.Application/"]
COPY ["src/Ecommerce.Domain/Ecommerce.Domain.csproj", "src/Ecommerce.Domain/"]
COPY ["src/Ecommerce.Infrastructure/Ecommerce.Infrastructure.csproj", "src/Ecommerce.Infrastructure/"]

# Test projects
COPY ["tests/Ecommerce.Api.Tests/Ecommerce.Api.Tests.csproj", "tests/Ecommerce.Api.Tests/"]
COPY ["tests/Ecommerce.Application.Tests/Ecommerce.Application.Tests.csproj", "tests/Ecommerce.Application.Tests/"]
COPY ["tests/Ecommerce.Domain.Tests/Ecommerce.Domain.Tests.csproj", "tests/Ecommerce.Domain.Tests/"]
COPY ["tests/Ecommerce.TestUtils/Ecommerce.TestUtils.csproj", "tests/Ecommerce.TestUtils/"]

# Restore using the solution file
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet restore Ecommerce.slnx --nologo

# Copy the rest of the source code
COPY . .

# Publish using the same cache
RUN --mount=type=cache,target=/root/.nuget/packages \
	dotnet publish src/Ecommerce.Api/Ecommerce.Api.csproj -c Release -o /app/publish \
	/p:UseAppHost=false

# -------------------------------------
# Runtime stage
# -------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
	libgssapi-krb5-2 \
	libkrb5-3 \
	&& rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Standardize port for Railway/Azure
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
