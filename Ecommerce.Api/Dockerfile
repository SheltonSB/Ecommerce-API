# Stage 1: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-builder
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=backend-builder /app/publish .

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
