FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./sample/ServiceA/ServiceA.csproj .
RUN dotnet restore "./sample/ServiceA/ServiceA.csproj"
COPY ./sample/ServiceA .
WORKDIR /src/sample/ServiceA
RUN dotnet build "ServiceA.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceA.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceA.dll"]
