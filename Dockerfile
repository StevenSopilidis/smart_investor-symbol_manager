FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY *.sln .

RUN dotnet sln symbol_manager.sln remove Tests/Application.Tests/Application.Tests.csproj
RUN dotnet sln symbol_manager.sln remove Tests/Infrastructure.Tests/Infrastructure.Tests.csproj
RUN dotnet sln symbol_manager.sln remove Tests/Api.Tests/Api.Tests.csproj

COPY API/*.csproj ./API/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/

RUN dotnet restore

COPY . .

WORKDIR /src/API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]
