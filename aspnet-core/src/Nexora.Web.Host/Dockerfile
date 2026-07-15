FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/Nexora.Web.Host/Nexora.Web.Host.csproj", "src/Nexora.Web.Host/"]
COPY ["src/Nexora.Web.Core/Nexora.Web.Core.csproj", "src/Nexora.Web.Core/"]
COPY ["src/Nexora.Application/Nexora.Application.csproj", "src/Nexora.Application/"]
COPY ["src/Nexora.Core/Nexora.Core.csproj", "src/Nexora.Core/"]
COPY ["src/Nexora.EntityFrameworkCore/Nexora.EntityFrameworkCore.csproj", "src/Nexora.EntityFrameworkCore/"]
WORKDIR "/src/src/Nexora.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/Nexora.Web.Host", "src/Nexora.Web.Host"]
COPY ["src/Nexora.Web.Core", "src/Nexora.Web.Core"]
COPY ["src/Nexora.Application", "src/Nexora.Application"]
COPY ["src/Nexora.Core", "src/Nexora.Core"]
COPY ["src/Nexora.EntityFrameworkCore", "src/Nexora.EntityFrameworkCore"]
WORKDIR "/src/src/Nexora.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Nexora.Web.Host.dll"]
