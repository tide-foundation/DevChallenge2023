#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DevChallenge-Node/DevChallenge-Node.csproj", "DevChallenge-Node/"]
RUN dotnet restore "DevChallenge-Node/DevChallenge-Node.csproj"
COPY . .
WORKDIR "/src/DevChallenge-Node"
RUN dotnet build "DevChallenge-Node.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevChallenge-Node.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevChallenge-Node.dll"]