FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.sln ./
COPY */*.csproj ./
RUN for f in */*.csproj; do mkdir -p "$(dirname "$f")" && cp "$f" "$(dirname "$f")"; done
RUN dotnet restore

COPY . ./

RUN dotnet publish symbol_manager/symbol_manager.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "symbol_manager.dll"]
