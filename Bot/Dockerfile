FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RatingsBot/RatingsBot.csproj", "RatingsBot/"]
RUN dotnet restore "RatingsBot/RatingsBot.csproj"
COPY . .
WORKDIR "/src/RatingsBot"
RUN dotnet build "RatingsBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RatingsBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RatingsBot.dll"]
