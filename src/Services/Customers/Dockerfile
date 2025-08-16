# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Services/Customers/Customers.Api/Customers.Api.csproj
RUN dotnet publish Services/Customers/Customers.Api/Customers.Api.csproj -c Release -o /app /p:UseAppHost=false

# Runtime 
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet","Customers.Api.dll"]
