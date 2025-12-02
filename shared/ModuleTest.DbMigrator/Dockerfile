FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mycompanyname/moduletest-base:1.0 AS build
WORKDIR /src

COPY . .
WORKDIR "/src/shared/ModuleTest.DbMigrator"
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ModuleTest.DbMigrator.dll"]