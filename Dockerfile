FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Cafe.Api/*.csproj Cafe.Api/
COPY Cafe.Application/*.csproj Cafe.Application/
COPY Cafe.Domain/*.csproj Cafe.Domain/
COPY Cafe.Infrastructure/*.csproj Cafe.Infrastructure/
RUN dotnet restore Cafe.Api/Cafe.Api.csproj
COPY . .
RUN dotnet publish Cafe.Api/Cafe.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cafe.Api.dll"]