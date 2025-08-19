# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy
COPY . .
RUN dotnet restore PeopleManager.Api/PeopleManager.Api.csproj
RUN dotnet publish PeopleManager.Api/PeopleManager.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /tmp && chmod -R 777 /tmp
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8081
EXPOSE 8081
ENTRYPOINT ["dotnet", "PeopleManager.Api.dll"]
