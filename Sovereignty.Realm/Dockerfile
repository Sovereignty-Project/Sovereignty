FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sovereignty.Realm/Sovereignty.Realm.csproj", "Sovereignty.Realm/"]
COPY ["Sovereignty.Models/Sovereignty.Models.csproj", "Sovereignty.Models/"]
RUN dotnet restore "Sovereignty.Realm/Sovereignty.Realm.csproj"
COPY . .
WORKDIR "/src/Sovereignty.Realm/"
RUN dotnet build "Sovereignty.Realm.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Sovereignty.Realm.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sovereignty.Realm.dll"]
