FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["V.Panel.Backend/V.Panel.Backend.csproj", "V.Panel.Backend/"]
COPY ["V.Panel.Models/V.Panel.Models.csproj", "V.Panel.Models/"]
RUN dotnet restore "V.Panel.Backend/V.Panel.Backend.csproj"
COPY . .
WORKDIR "/src/V.Panel.Backend"
RUN dotnet build "V.Panel.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "V.Panel.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "V.Panel.Backend.dll"]
