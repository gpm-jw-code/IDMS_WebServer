#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IDMSWebServer/IDMSWebServer.csproj", "IDMSWebServer/"]
RUN dotnet restore "IDMSWebServer/IDMSWebServer.csproj"
COPY . .
WORKDIR "/src/IDMSWebServer"
RUN dotnet build "IDMSWebServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IDMSWebServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IDMSWebServer.dll"]