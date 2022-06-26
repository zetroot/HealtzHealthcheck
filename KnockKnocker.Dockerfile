FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["sample/KnockKnocker/KnockKnocker.csproj", "."]
RUN dotnet restore "KnockKnocker.csproj"
COPY ./sample/KnockKnocker .
RUN dotnet build "KnockKnocker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KnockKnocker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KnockKnocker.dll"]
