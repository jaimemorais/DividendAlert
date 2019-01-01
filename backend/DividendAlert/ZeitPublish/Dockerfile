FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
LABEL name="zeitdividendalertjaime"
WORKDIR /app
EXPOSE 80
ARG source=.

COPY $source .
ENTRYPOINT ["dotnet", "DividendAlert.dll"]