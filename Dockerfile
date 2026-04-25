FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Ecommerce.sln ./
COPY Ecommerce.Api/Ecommerce.Api.csproj Ecommerce.Api/
COPY Ecommerce.Tests/Ecommerce.Tests.csproj Ecommerce.Tests/
RUN dotnet restore Ecommerce.sln

COPY . .
RUN dotnet publish Ecommerce.Api/Ecommerce.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DatabaseProvider=Sqlite
ENV ConnectionStrings__DefaultConnection=Data Source=/app/data/Ecommerce.db

RUN mkdir -p /app/data

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
