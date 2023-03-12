#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["test/SimpleTraveling.Test/SimpleTraveling.Test.csproj", "test/SimpleTraveling.Test/"]
COPY ["src/SimpleTraveling.Abstractions/SimpleTraveling.Abstractions.csproj", "src/SimpleTraveling.Abstractions/"]
COPY ["src/SimpleTraveling.CastService/SimpleTraveling.CostService.csproj", "src/SimpleTraveling.CastService/"]
COPY ["src/SimpleTraveling.DriverService.Remote/SimpleTraveling.DriverService.Remote.csproj", "src/SimpleTraveling.DriverService.Remote/"]
COPY ["src/SimpleTraveling.Remote/SimpleTraveling.Remote.csproj", "src/SimpleTraveling.Remote/"]
COPY ["src/SimpleTraveling.TravelService.Remote/SimpleTraveling.TravelService.Remote.csproj", "src/SimpleTraveling.TravelService.Remote/"]
COPY ["src/SimpleTraveling.DriverService/SimpleTraveling.DriverService.csproj", "src/SimpleTraveling.DriverService/"]
COPY ["src/SimpleTraveling.TravelService/SimpleTraveling.TravelService.csproj", "src/SimpleTraveling.TravelService/"]
COPY ["src/SimpleTraveling.CostService.Remote/SimpleTraveling.CostService.Remote.csproj", "src/SimpleTraveling.CostService.Remote/"]
RUN dotnet restore "test/SimpleTraveling.Test/SimpleTraveling.Test.csproj"
COPY . .
WORKDIR "/src/test/SimpleTraveling.Test"
RUN dotnet build "SimpleTraveling.Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleTraveling.Test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleTraveling.Test.dll"]