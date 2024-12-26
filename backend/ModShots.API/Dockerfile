FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["backend/ModShots.API/ModShots.API.csproj", "backend/ModShots.API/"]
COPY ["backend/ModShots.Application/ModShots.Application.csproj", "backend/ModShots.Application/"]
COPY ["backend/ModShots.Domain/ModShots.Domain.csproj", "backend/ModShots.Domain/"]
RUN dotnet restore "backend/ModShots.API/ModShots.API.csproj"
COPY . .
WORKDIR "/src/backend/ModShots.API"
RUN dotnet build "ModShots.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ModShots.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ModShots.API.dll"]
