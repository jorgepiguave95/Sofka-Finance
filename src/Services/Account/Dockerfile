# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Services/Account/Account.Api/Account.Api.csproj
RUN dotnet publish Services/Account/Account.Api/Account.Api.csproj -c Release -o /app /p:UseAppHost=false

# Runtime 
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet","Account.Api.dll"]
