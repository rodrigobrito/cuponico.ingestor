FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR .
COPY Ingestor.sln .
COPY ["src//Ingestor.ConsoleHost//Ingestor.ConsoleHost.csproj", ".//src//Ingestor.ConsoleHost//"]

RUN dotnet restore -s https://api.nuget.org/v3/index.json
COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Ingestor.ConsoleHost.dll"]