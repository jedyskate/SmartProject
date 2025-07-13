FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SmartConfig.Blazor/SmartConfig.Blazor/SmartConfig.Blazor.csproj", "src/SmartConfig.Blazor/SmartConfig.Blazor/"]
COPY ["src/SmartConfig.Blazor/SmartConfig.Blazor.Client/SmartConfig.Blazor.Client.csproj", "src/SmartConfig.Blazor/SmartConfig.Blazor.Client/"]
RUN dotnet restore "src/SmartConfig.Blazor/SmartConfig.Blazor/SmartConfig.Blazor.csproj"
COPY . .
WORKDIR "/src/src/SmartConfig.Blazor/SmartConfig.Blazor"
RUN dotnet build "SmartConfig.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SmartConfig.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartConfig.Blazor.dll"]
