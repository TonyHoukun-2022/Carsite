### Build Stage ###
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
# This sets the working directory inside the container to /app. All subsequent commands will be run from this directory.
WORKDIR /app
# This exposes port 80 on the container, allowing access to the application from this port.
EXPOSE 80

#  copy the solution file and the .csproj files for all projects into the container. This allows Docker to take advantage of caching, as it only needs to rebuild the layers that have changed.
COPY Carsites.sln Carsites.sln
COPY src/AuctionService/AuctionService.csproj  src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj  src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj  src/GatewayService/GatewayService.csproj
COPY src/Contracts/Contracts.csproj  src/Contracts/Contracts.csproj
COPY src/IdentityService/IdentityService.csproj  src/IdentityService/IdentityService.csproj

# Restore package dependencies
# restore all the package dependencies for the projects.
RUN dotnet restore Carsites.sln

# These lines copy the source code for the AuctionService and Contracts projects into the container.
COPY src/IdentityService src/IdentityService
WORKDIR /app/src/IdentityService
# publishes the application in Release configuration and outputs the published files to the /app/src/out directory.
RUN dotnet publish -c Release -o /app/src/out

### Runtime Stage ###
# build runtime img
FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app
# copies the published files from the build stage to the /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "IdentityService.dll" ]


#### Build Stage: Uses the .NET SDK image to build the application. It restores dependencies, copies the source code, and publishes the application.
#### Runtime Stage: Uses the ASP.NET Core runtime image to run the application. It copies the published files from the build stage and sets the entry point.