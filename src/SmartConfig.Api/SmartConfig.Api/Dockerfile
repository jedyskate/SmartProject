FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../../nuget.config", "."]
COPY ["src/SmartConfig.Api/SmartConfig.Api/SmartConfig.Api.csproj", "src/SmartConfig.Api/SmartConfig.Api/"]
COPY ["tools/SmartConfig.ServiceDefaults/SmartConfig.ServiceDefaults.csproj", "tools/SmartConfig.ServiceDefaults/"]
COPY ["src/SmartConfig.Api/SmartConfig.Application/SmartConfig.Application.csproj", "src/SmartConfig.Api/SmartConfig.Application/"]
COPY ["src/SmartConfig.Api/SmartConfig.Common/SmartConfig.Common.csproj", "src/SmartConfig.Api/SmartConfig.Common/"]
COPY ["src/SmartConfig.Api/SmartConfig.Core/SmartConfig.Core.csproj", "src/SmartConfig.Api/SmartConfig.Core/"]
COPY ["src/SmartConfig.Api/SmartConfig.Data/SmartConfig.Data.csproj", "src/SmartConfig.Api/SmartConfig.Data/"]
COPY ["src/SmartConfig.Api/SmartConfig.Orleans.Silo/SmartConfig.Orleans.Silo.csproj", "src/SmartConfig.Api/SmartConfig.Orleans.Silo/"]
RUN dotnet restore "src/SmartConfig.Api/SmartConfig.Api/SmartConfig.Api.csproj"
COPY . .
WORKDIR "/src/src/SmartConfig.Api/SmartConfig.Api"
RUN dotnet build "./SmartConfig.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SmartConfig.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartConfig.Api.dll"]
