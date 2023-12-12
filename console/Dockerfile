FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["cipher-communcation/cipher-communcation.csproj", "cipher-communcation/"]
RUN dotnet restore "cipher-communcation/cipher-communcation.csproj"
COPY . .
WORKDIR "/src/cipher-communcation"
RUN dotnet build "cipher-communcation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "cipher-communcation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "cipher-communcation.dll"]
