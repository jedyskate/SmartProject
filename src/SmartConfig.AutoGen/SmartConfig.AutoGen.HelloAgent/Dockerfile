FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SmartConfig.AutoGen/SmartConfig.AutoGen.csproj", "src/SmartConfig.AutoGen/"]
RUN dotnet restore "src/SmartConfig.AutoGen/SmartConfig.AutoGen.csproj"
COPY . .
WORKDIR "/src/src/SmartConfig.AutoGen"
RUN dotnet build "./SmartConfig.AutoGen.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SmartConfig.AutoGen.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartConfig.AutoGen.dll"]
