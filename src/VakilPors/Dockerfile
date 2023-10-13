FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["VakilPors.Web/VakilPors.Api.csproj", "VakilPors.Web/"]
COPY ["VakilPors.Core/VakilPors.Core.csproj", "VakilPors.Core/"]
COPY ["VakilPors.Shared/VakilPors.Shared.csproj", "VakilPors.Shared/"]
COPY ["VakilPors.Data/VakilPors.Data.csproj", "VakilPors.Data/"]
RUN dotnet restore "VakilPors.Web/VakilPors.Api.csproj"
COPY . .
WORKDIR "/src/VakilPors.Web"
RUN dotnet build "VakilPors.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VakilPors.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VakilPors.Api.dll"]
