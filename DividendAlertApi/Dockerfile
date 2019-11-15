FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:3.0 AS build
WORKDIR /src
COPY ["DividendAlert/DividendAlert.csproj", "DividendAlert/"]
RUN dotnet restore "DividendAlert/DividendAlert.csproj"
COPY . .
WORKDIR "/src/DividendAlert"
RUN dotnet build "DividendAlert.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DividendAlert.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DividendAlert.dll"]