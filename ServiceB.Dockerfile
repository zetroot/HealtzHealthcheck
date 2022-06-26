FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./sample/ServiceB/ServiceB.csproj ./sample/ServiceB/
COPY ./src/HealthHealthcheck/HealthHealthcheck.csproj ./src/HealthHealthcheck/
RUN dotnet restore "./sample/ServiceB/ServiceB.csproj"
COPY ./sample/ServiceB/ ./sample/ServiceB/
COPY ./src/HealthHealthcheck/ ./src/HealthHealthcheck/
WORKDIR /src/sample/ServiceB
RUN dotnet build "ServiceB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceB.dll"]
