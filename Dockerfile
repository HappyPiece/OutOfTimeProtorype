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
ENV ROOT_EMAIL=root@root.net
ENV ROOT_PASS=aboba

EXPOSE 5000

ENTRYPOINT ROOT_EMAIL=${ROOT_EMAIL} ROOT_EMAIL=${ROOT_EMAIL} dotnet OutOfTimePrototype.dll --environment=Development