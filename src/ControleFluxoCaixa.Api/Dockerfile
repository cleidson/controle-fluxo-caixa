# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install mssql-tools in the base image
RUN apt-get update && apt-get install -y \
    curl \
    apt-transport-https \
    gnupg \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev \
    && echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Add mssql-tools to PATH for runtime use
ENV PATH="/opt/mssql-tools/bin:$PATH"

# Image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ControleFluxoCaixa.Api/ControleFluxoCaixa.Api.csproj", "src/ControleFluxoCaixa.Api/"]
RUN dotnet restore "./src/ControleFluxoCaixa.Api/ControleFluxoCaixa.Api.csproj"
COPY . .
WORKDIR "/src/src/ControleFluxoCaixa.Api"
RUN dotnet build "./ControleFluxoCaixa.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ControleFluxoCaixa.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControleFluxoCaixa.Api.dll"]
