# build
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /source
COPY . .
RUN dotnet restore "./OutOfTimePrototype/OutOfTimePrototype.csproj" --disable-parallel && dotnet publish "./OutOfTimePrototype/OutOfTimePrototype.csproj" -c debug -o /app --no-restore

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 
WORKDIR /app
COPY --from=build /app ./
USER 1000
ENV ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_HTTP_PORT=https://+:5001

EXPOSE 5000

#specify environment to configure swagger availability
ENTRYPOINT ["dotnet", "OutOfTimePrototype.dll", "--environment=Development"]