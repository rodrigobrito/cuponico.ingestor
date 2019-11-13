FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR .
COPY Ingestor.sln .
COPY ["src//Cuponico.Ingestor.Host//Cuponico.Ingestor.Host.csproj", ".//src//Cuponico.Ingestor.Host//"]

RUN dotnet restore -s https://api.nuget.org/v3/index.json
COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cuponico.Ingestor.Host.dll"]