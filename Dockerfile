#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/SmartConfig.Api/SmartConfig.Api.csproj", "src/SmartConfig.Api/"]
COPY ["src/SmartConfig.Application/SmartConfig.Application.csproj", "src/SmartConfig.Application/"]
COPY ["src/SmartConfig.Common/SmartConfig.Common.csproj", "src/SmartConfig.Common/"]
COPY ["src/SmartConfig.Core/SmartConfig.Core.csproj", "src/SmartConfig.Core/"]
COPY ["src/SmartConfig.Data/SmartConfig.Data.csproj", "src/SmartConfig.Data/"]
RUN dotnet restore "src/SmartConfig.Api/SmartConfig.Api.csproj"
COPY . .
WORKDIR "/src/src/SmartConfig.Api"
RUN dotnet build "SmartConfig.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartConfig.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartConfig.Api.dll"]