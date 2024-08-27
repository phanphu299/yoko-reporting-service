# SDK build image
FROM dxpprivate.azurecr.io/ahi-build:6.0 AS build
WORKDIR .
COPY NuGet.Config ./
COPY src/Reporting.Api/*.csproj         ./src/Reporting.Api/
COPY src/Reporting.Application/*.csproj ./src/Reporting.Application/
COPY src/Reporting.Domain/*.csproj      ./src/Reporting.Domain/
COPY src/Reporting.Persistence/*.csproj ./src/Reporting.Persistence/
RUN dotnet restore ./src/Reporting.Api/*.csproj /property:Configuration=Release -nowarn:msb3202,nu1503

COPY src/ ./src
RUN dotnet publish ./src/Reporting.Api/*.csproj --no-restore -c Release -o /app/out

# Run time image
FROM dxpprivate.azurecr.io/ahi-runtime:6.0-full as final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Reporting.Api.dll"]