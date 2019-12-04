FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY blazorTeddy/TeddyBlazor.csproj blazorTeddy/
RUN dotnet restore blazorTeddy/TeddyBlazor.csproj
COPY . .
WORKDIR /src/blazorTeddy
RUN dotnet build TeddyBlazor.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TeddyBlazor.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TeddyBlazor.dll"]